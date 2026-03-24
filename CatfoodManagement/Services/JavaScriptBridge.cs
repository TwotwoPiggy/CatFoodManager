using CefSharp;
using CefSharp.WinForms;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CatfoodManagement.Services
{
    public class JavaScriptBridge
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly Bridge.CatFoodApi _catFoodApi;
        private readonly Bridge.BrandApi _brandApi;
        private readonly Bridge.BestPriceApi _bestPriceApi;
        private readonly Bridge.OcrApi _ocrApi;
        private readonly Bridge.OcrPromptApi _ocrPromptApi;
        private readonly Bridge.SettingsApi _settingsApi;
        private readonly Bridge.TaskApi _taskApi;
        private readonly Bridge.BackgroundServiceApi _backgroundServiceApi;

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
            _backgroundServiceApi = new Bridge.BackgroundServiceApi(serviceProvider);
        }

        public void RegisterApis(ChromiumWebBrowser browser)
        {
            browser.JavascriptObjectRepository.Register("catFoodApi", _catFoodApi, BindingOptions.DefaultBinder);
            browser.JavascriptObjectRepository.Register("brandApi", _brandApi, BindingOptions.DefaultBinder);
            browser.JavascriptObjectRepository.Register("bestPriceApi", _bestPriceApi, BindingOptions.DefaultBinder);
            browser.JavascriptObjectRepository.Register("ocrApi", _ocrApi, BindingOptions.DefaultBinder);
            browser.JavascriptObjectRepository.Register("ocrPromptApi", _ocrPromptApi, BindingOptions.DefaultBinder);
            browser.JavascriptObjectRepository.Register("settingsApi", _settingsApi, BindingOptions.DefaultBinder);
            browser.JavascriptObjectRepository.Register("taskApi", _taskApi, BindingOptions.DefaultBinder);
            browser.JavascriptObjectRepository.Register("backgroundServiceApi", _backgroundServiceApi, BindingOptions.DefaultBinder);
        }

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
                    await CefSharp.BindObjectAsync('backgroundServiceApi');
                    
                    if (typeof window.setCefSharpReady === 'function') {
                        window.setCefSharpReady();
                    }
                    
                    console.log('CefSharp APIs initialized and ready');
                })();
            ");
        }
    }
}
