using CefSharp;
using CefSharp.WinForms;
using CatfoodManagement.Services;

namespace CatfoodManagement
{
    public partial class MainForm : Form
    {
        private readonly JavaScriptBridge _javaScriptBridge;
        private ChromiumWebBrowser _browser = null!;

        public MainForm(JavaScriptBridge javaScriptBridge)
        {
            _javaScriptBridge = javaScriptBridge;
            InitializeComponent();
#if DEBUG

            InitMenu();
#endif
            InitBrowser();
        }

        private void InitMenu()
        {
            var menuStrip = new MenuStrip();
            var toolsMenu = new ToolStripMenuItem("工具(&T)");
            var devToolsItem = new ToolStripMenuItem("开发者工具(&D)", null, (s, e) => _browser.ShowDevTools())
            {
                ShortcutKeys = Keys.F12
            };
            toolsMenu.DropDownItems.Add(devToolsItem);
            menuStrip.Items.Add(toolsMenu);
            Controls.Add(menuStrip);
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
                    await _javaScriptBridge.InitializeAsync(_browser);
                }
            };

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
}
