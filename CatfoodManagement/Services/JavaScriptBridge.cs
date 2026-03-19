using CefSharp;
using CefSharp.WinForms;

namespace CatfoodManagement.Services
{
    public class JavaScriptBridge
    {
        private readonly IServiceProvider _serviceProvider;
        private Bridge.CatFoodApi? _catFoodApi;
        private Bridge.BrandApi? _brandApi;
        private Bridge.BestPriceApi? _bestPriceApi;
        private bool _registered = false;

        public JavaScriptBridge(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public void RegisterApis(ChromiumWebBrowser browser)
        {
            if (_registered) return;
            
            _catFoodApi = new Bridge.CatFoodApi(_serviceProvider);
            _brandApi = new Bridge.BrandApi(_serviceProvider);
            _bestPriceApi = new Bridge.BestPriceApi(_serviceProvider);
            
            browser.JavascriptObjectRepository.Register("catFoodApi", _catFoodApi, BindingOptions.DefaultBinder);
            browser.JavascriptObjectRepository.Register("brandApi", _brandApi, BindingOptions.DefaultBinder);
            browser.JavascriptObjectRepository.Register("bestPriceApi", _bestPriceApi, BindingOptions.DefaultBinder);
            
            _registered = true;
        }

        public async Task InitializeAsync(ChromiumWebBrowser browser)
        {
            var script = @"
                (async function() {
                    try {
                        await CefSharp.BindObjectAsync('catFoodApi');
                        await CefSharp.BindObjectAsync('brandApi');
                        await CefSharp.BindObjectAsync('bestPriceApi');
                        console.log('CefSharp APIs bound successfully');
                        if (typeof window.setCefSharpReady === 'function') {
                            window.setCefSharpReady();
                            console.log('CefSharp ready signal sent to Vue app');
                        }
                    } catch (e) {
                        console.error('CefSharp bind error:', e);
                    }
                })();
            ";
            await browser.EvaluateScriptAsync(script);
        }
    }
}
