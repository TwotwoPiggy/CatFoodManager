using CefSharp;
using CefSharp.WinForms;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CatfoodManagement.Services
{
    /// <summary>
    /// JavaScript 桥接器，用于 C# 后端与前端 JavaScript 之间的通信
    /// 通过 CefSharp 将 C# API 对象暴露给前端调用
    /// </summary>
    public class JavaScriptBridge
    {
        /// <summary>
        /// 服务提供者，用于获取依赖注入的服务
        /// </summary>
        private readonly IServiceProvider _serviceProvider;
        
        /// <summary>
        /// 猫粮管理 API
        /// </summary>
        private readonly Bridge.CatFoodApi _catFoodApi;
        
        /// <summary>
        /// 品牌管理 API
        /// </summary>
        private readonly Bridge.BrandApi _brandApi;
        
        /// <summary>
        /// 最佳价格管理 API
        /// </summary>
        private readonly Bridge.BestPriceApi _bestPriceApi;
        
        /// <summary>
        /// OCR 识别 API
        /// </summary>
        private readonly Bridge.OcrApi _ocrApi;
        
        private readonly Bridge.OcrPromptApi _ocrPromptApi;
        
        /// <summary>
        /// 设置管理 API
        /// </summary>
        private readonly Bridge.SettingsApi _settingsApi;

        /// <summary>
        /// 任务管理 API
        /// </summary>
        private readonly Bridge.TaskApi _taskApi;

        /// <summary>
        /// 构造函数，初始化所有 API 实例
        /// </summary>
        /// <param name="serviceProvider">服务提供者</param>
        public JavaScriptBridge(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _catFoodApi = new Bridge.CatFoodApi(serviceProvider);
            _brandApi = new Bridge.BrandApi(serviceProvider);
            _bestPriceApi = new Bridge.BestPriceApi(serviceProvider);
            _ocrApi = new Bridge.OcrApi(serviceProvider);
            _ocrPromptApi = new Bridge.OcrPromptApi(serviceProvider);
            _settingsApi = new Bridge.SettingsApi(serviceProvider.GetRequiredService<IConfiguration>());
            _taskApi = new Bridge.TaskApi(serviceProvider);
        }

        /// <summary>
        /// 向浏览器注册所有 API 对象
        /// </summary>
        /// <param name="browser">Chromium 浏览器实例</param>
        public void RegisterApis(ChromiumWebBrowser browser)
        {
            browser.JavascriptObjectRepository.Register("catFoodApi", _catFoodApi, BindingOptions.DefaultBinder);
            browser.JavascriptObjectRepository.Register("brandApi", _brandApi, BindingOptions.DefaultBinder);
            browser.JavascriptObjectRepository.Register("bestPriceApi", _bestPriceApi, BindingOptions.DefaultBinder);
            browser.JavascriptObjectRepository.Register("ocrApi", _ocrApi, BindingOptions.DefaultBinder);
            browser.JavascriptObjectRepository.Register("ocrPromptApi", _ocrPromptApi, BindingOptions.DefaultBinder);
            browser.JavascriptObjectRepository.Register("settingsApi", _settingsApi, BindingOptions.DefaultBinder);
            browser.JavascriptObjectRepository.Register("taskApi", _taskApi, BindingOptions.DefaultBinder);
        }

        /// <summary>
        /// 初始化前端 JavaScript 环境，绑定 API 对象
        /// </summary>
        /// <param name="browser">Chromium 浏览器实例</param>
        public async Task InitializeAsync(ChromiumWebBrowser browser)
        {
            await browser.EvaluateScriptAsync(@"
                (async function() {
                    await CefSharp.BindObjectAsync('catFoodApi');
                    await CefSharp.BindObjectAsync('brandApi');
                    await CefSharp.BindObjectAsync('bestPriceApi');
                    await CefSharp.BindObjectAsync('ocrApi');
                    await CefSharp.BindObjectAsync('ocrPromptApi');
                    await CefSharp.BindObjectAsync('settingsApi');
                    await CefSharp.BindObjectAsync('taskApi');
                    
                    if (typeof window.setCefSharpReady === 'function') {
                        window.setCefSharpReady();
                    }
                    
                    console.log('CefSharp APIs initialized and ready');
                })();
            ");
        }
    }
}
