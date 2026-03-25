using CefSharp;
using CefSharp.WinForms;
using CatfoodManagement.Services;
using CatFoodManager.Application.Interfaces;
using CatFoodManager.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;

namespace CatfoodManagement
{
    public partial class MainForm : Form
    {
        private readonly JavaScriptBridge _javaScriptBridge;
        private readonly IServiceProvider _serviceProvider;
        private ChromiumWebBrowser _browser = null!;

        public MainForm(
            JavaScriptBridge javaScriptBridge,
            IServiceProvider serviceProvider)
        {
            _javaScriptBridge = javaScriptBridge;
            _serviceProvider = serviceProvider;
            InitializeComponent();
            InitBrowser();
        }

        private void InitBrowser()
        {
            try
            {
                var settings = new CefSettings
                {
                    CachePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "CatFoodManager\\Cache"),
                    LogFile = Path.Combine(AppContext.BaseDirectory, "logs\\cef.log"),
                    LogSeverity = LogSeverity.Info
                };

                settings.CefCommandLineArgs.Add("disable-gpu", "1");
                settings.CefCommandLineArgs.Add("disable-gpu-compositing", "1");
                settings.CefCommandLineArgs.Add("enable-media-stream", "1");

                var cefPath = Path.Combine(AppContext.BaseDirectory, "logs");
                Directory.CreateDirectory(cefPath);

                if (!Cef.Initialize(settings, performDependencyCheck: true, browserProcessHandler: null))
                {
                    throw new InvalidOperationException("CefSharp 初始化失败，请确保所有依赖项已正确安装。");
                }

                var htmlPath = Path.Combine(AppContext.BaseDirectory, "wwwroot", "index.html");
                var url = File.Exists(htmlPath) ? $"file:///{htmlPath.Replace("\\", "/")}" : "about:blank";

                _browser = new ChromiumWebBrowser(url)
                {
                    Dock = DockStyle.Fill
                };

                _javaScriptBridge.RegisterApis(_browser);
                
                _browser.RenderProcessMessageHandler = new BridgeInitRenderProcessHandler(_browser, _javaScriptBridge);

                _browser.FrameLoadEnd += async (s, e) =>
                {
                    if (e.Frame.IsMain)
                    {
                        try
                        {
                            using var scope = _serviceProvider.CreateScope();
                            var databaseInitializer = scope.ServiceProvider.GetRequiredService<IDatabaseInitializer>();
                            var ocrPromptService = scope.ServiceProvider.GetRequiredService<IOcrPromptService>();
                            
                            await databaseInitializer.InitializeAsync();
                            await ocrPromptService.InitializeDefaultPromptAsync();
                        }
                        catch (Exception initEx)
                        {
                            _browser.BeginInvoke(new Action(() =>
                            {
                                MessageBox.Show(
                                    $"后台服务初始化失败: {initEx.Message}\n请检查配置或数据库路径。",
                                    "初始化失败",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Warning);
                            }));
                        }
                    }
                };

#if DEBUG
                _browser.MenuHandler = new DevToolsContextMenuHandler(_browser);
#endif

                Controls.Add(_browser);
            }
            catch (Exception ex)
            {
                var logPath = Path.Combine(AppContext.BaseDirectory, "logs");
                Directory.CreateDirectory(logPath);
                var logFile = Path.Combine(logPath, $"browser_init_error_{DateTime.Now:yyyyMMdd_HHmmss}.log");
                File.WriteAllText(logFile, $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] 浏览器初始化失败:\n{ex}\n\nStackTrace:\n{ex.StackTrace}");
                
                MessageBox.Show(
                    $"浏览器初始化失败:\n{ex.Message}\n\n详细信息已保存到:\n{logFile}",
                    "初始化错误",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                
                throw;
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            Cef.Shutdown();
            base.OnFormClosing(e);
        }

        public void LoadUrl(string url)
        {
            _browser.Load(url);
        }

        public async Task<string> ExecuteScriptAsync(string script)
        {
            var response = await _browser.EvaluateScriptAsync(script);
            return response.Success ? response.Result?.ToString() ?? string.Empty : string.Empty;
        }
    }

    internal class BridgeInitRenderProcessHandler : IRenderProcessMessageHandler
    {
        private readonly ChromiumWebBrowser _browser;
        private readonly JavaScriptBridge _bridge;
        private bool _initialized = false;

        public BridgeInitRenderProcessHandler(ChromiumWebBrowser browser, JavaScriptBridge bridge)
        {
            _browser = browser;
            _bridge = bridge;
        }

        public void OnContextCreated(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame)
        {
            if (!_initialized && frame.IsMain)
            {
                _initialized = true;
                _bridge.InitializeAsync(_browser).ConfigureAwait(false);
            }
        }

        public void OnContextReleased(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame) { }
        public void OnFocusedNodeChanged(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IDomNode node) { }
        public void OnUncaughtException(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, JavascriptException exception) { }
    }

#if DEBUG
    internal class DevToolsContextMenuHandler : IContextMenuHandler
    {
        private const int ShowDevTools = 26501;
        private readonly ChromiumWebBrowser _browser;

        public DevToolsContextMenuHandler(ChromiumWebBrowser browser)
        {
            _browser = browser;
        }

        public void OnBeforeContextMenu(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IContextMenuParams parameters, IMenuModel model)
        {
            model.AddItem(CefMenuCommand.CustomFirst, "开发者工具(&D)");
            model.SetAcceleratorAt(model.Count - 1, (int)Keys.F12, false, false, false);
        }

        public bool OnContextMenuCommand(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IContextMenuParams parameters, CefMenuCommand commandId, CefEventFlags eventFlags)
        {
            if (commandId == CefMenuCommand.CustomFirst)
            {
                _browser.ShowDevTools();
                return true;
            }
            return false;
        }

        public void OnContextMenuDismissed(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame) { }

        public bool RunContextMenu(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IContextMenuParams parameters, IMenuModel model, IRunContextMenuCallback callback)
        {
            return false;
        }
    }
#endif
}
