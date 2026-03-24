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
            var settings = new CefSettings
            {
                CachePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "CatFoodManager\\Cache"),
                LogFile = Path.Combine(AppContext.BaseDirectory, "logs\\cef.log"),
                LogSeverity = LogSeverity.Info
            };

            settings.CefCommandLineArgs.Add("disable-gpu", "1");
            settings.CefCommandLineArgs.Add("disable-gpu-compositing", "1");
            settings.CefCommandLineArgs.Add("enable-media-stream", "1");

            Cef.Initialize(settings, performDependencyCheck: true, browserProcessHandler: null);

            var htmlPath = Path.Combine(AppContext.BaseDirectory, "wwwroot", "index.html");
            var url = File.Exists(htmlPath) ? $"file:///{htmlPath.Replace("\\", "/")}" : "about:blank";

            _browser = new ChromiumWebBrowser(url)
            {
                Dock = DockStyle.Fill
            };

            _javaScriptBridge.RegisterApis(_browser);

            _browser.FrameLoadEnd += async (s, e) =>
            {
                if (e.Frame.IsMain)
                {
                    using var scope = _serviceProvider.CreateScope();
                    var databaseInitializer = scope.ServiceProvider.GetRequiredService<IDatabaseInitializer>();
                    var ocrPromptService = scope.ServiceProvider.GetRequiredService<IOcrPromptService>();
                    
                    await databaseInitializer.InitializeAsync();
                    await ocrPromptService.InitializeDefaultPromptAsync();
                    await _javaScriptBridge.InitializeAsync(_browser);
                }
            };

#if DEBUG
            _browser.MenuHandler = new DevToolsContextMenuHandler(_browser);
#endif

            Controls.Add(_browser);
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
