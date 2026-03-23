using CatFoodManager.Application.Extensions;
using CatFoodManager.Application.Interfaces;
using CatFoodManager.Core.Interfaces;
using CatFoodManager.Core.Models;
using CatFoodManager.Core.Repositories;
using CatFoodManager.Core.Services;
using CatFoodManager.Infrastructure.Configuration;
using CatFoodManager.Infrastructure.Extensions;
using CatFoodManager.Infrastructure.Services;
using CatfoodManagement.Services;
using CefSharp;
using CommonTools;
using CommonTools.Database;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Twotwo.Agent.Extensions;
using Twotwo.Agent.Interfaces;
using IApplicationGeminiOcrService = CatFoodManager.Application.Interfaces.IGeminiOcrService;
using OcrApiService = CatfoodManagement.Services.Bridge.OcrApi;

namespace CatfoodManagement
{
    internal static class Program
    {
        public static IServiceProvider ServiceProvider { get; set; } = null!;
        
        public static IConfiguration Configuration { get; set; } = null!;

        private static IBackgroundServiceControl? _backgroundServiceControl;
        private static CancellationTokenSource? _backgroundServiceCts;

        [STAThread]
        static void Main()
        {
            CefSharpSettings.ConcurrentTaskExecution = true;
            ApplicationConfiguration.Initialize();
            ConfigureServices();

            StartBackgroundService();

            var mainForm = ServiceProvider.GetRequiredService<MainForm>();
            Application.ApplicationExit += OnApplicationExit;
            Application.Run(mainForm);
        }

        private static void ConfigureServices()
        {
            var services = new ServiceCollection();

            Configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();

            services.AddSingleton<IConfiguration>(Configuration);

            services.AddLogging(builder =>
            {
                builder.AddConsole();
                builder.AddDebug();
                builder.SetMinimumLevel(LogLevel.Information);
            });

            var databaseSettings = Configuration.GetSection(DatabaseSettings.SectionName).Get<DatabaseSettings>();
            var databasePath = databaseSettings?.ConnectionString ?? "./data/catfood.db";

            services.AddInfrastructureSettings(Configuration);
            services.AddInfrastructure(databasePath);
            services.AddApplicationServices();

            services.AddGeminiAgent(Configuration, "AppSettings:AI");

            services.AddSingleton<MainForm>();
            services.AddSingleton<JavaScriptBridge>();
            services.AddSingleton<INotificationService, NotificationService>();
            
            services.AddSingleton<SQLiteHelper>();
            services.AddScoped<IRepository, CommonRepository>();
            services.AddScoped<PictureContentService>();
            
            services.AddScoped<IService<Brand>, BrandService>(serviceProvider => new BrandService(serviceProvider.GetRequiredService<IRepository>(), true));
            services.AddScoped<IService<CatFood>, CatFoodService>(serviceProvider => new CatFoodService(serviceProvider.GetRequiredService<IRepository>(), true));
            services.AddScoped<IService<Factory>, FactoryService>(serviceProvider => new FactoryService(serviceProvider.GetRequiredService<IRepository>(), true));
            services.AddScoped<IService<BestPrice>, LowestPriceService>(serviceProvider => new LowestPriceService(serviceProvider.GetRequiredService<IRepository>(), true));
            
            services.AddSingleton<IMemoryCache, MemoryCache>();
            services.AddScoped<IApplicationGeminiOcrService, CatFoodManager.Application.Services.GeminiOcrService>(sp =>
            {
                var agentService = sp.GetRequiredService<IGeminiAgentService>();
                var repository = sp.GetRequiredService<IRepository>();
                var logger = sp.GetRequiredService<ILogger<CatFoodManager.Application.Services.GeminiOcrService>>();
                var cache = sp.GetRequiredService<IMemoryCache>();
                return new CatFoodManager.Application.Services.GeminiOcrService(agentService, repository, logger, cache);
            });
            services.AddScoped<OcrApiService>();

            ServiceProvider = services.BuildServiceProvider();
        }

        private static void StartBackgroundService()
        {
            _backgroundServiceControl = ServiceProvider.GetRequiredService<IBackgroundServiceControl>();
            _backgroundServiceCts = new CancellationTokenSource();
            
            _ = _backgroundServiceControl.StartServiceAsync(_backgroundServiceCts.Token);
        }

        private static async void OnApplicationExit(object? sender, EventArgs e)
        {
            if (_backgroundServiceControl != null)
            {
                await _backgroundServiceControl.StopServiceAsync();
            }
            
            if (_backgroundServiceCts != null)
            {
                _backgroundServiceCts.Cancel();
                _backgroundServiceCts.Dispose();
            }
        }

        public static T? GetService<T>() where T : class
        {
            return (T?)ServiceProvider.GetService(typeof(T));
        }
    }
}
