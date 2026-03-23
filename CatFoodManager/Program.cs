using CatFoodManager.Application.Extensions;
using CatFoodManager.Application.Interfaces;
using CatFoodManager.Core.Interfaces;
using CatFoodManager.Core.Models;
using CatFoodManager.Core.Repositories;
using CatFoodManager.Core.Services;
using CatFoodManager.Infrastructure.Configuration;
using CatFoodManager.Infrastructure.Extensions;
using CatFoodManager.ViewModels;
using CommonTools;
using CommonTools.Database;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OcrApi;
using Twotwo.Agent.Extensions;
using Twotwo.Agent.Interfaces;
using IApplicationGeminiOcrService = CatFoodManager.Application.Interfaces.IGeminiOcrService;
using ICoreGeminiOcrService = CatFoodManager.Core.Interfaces.IGeminiOcrService;
using ICorePlatformRegExpService = CatFoodManager.Core.Interfaces.IPlatformRegExpService;

namespace CatFoodManager
{
    internal static class Program
    {
        public static IServiceProvider ServiceProvider { get; set; }

        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            ConfigureServicesAsync(needMigrate: true).GetAwaiter().GetResult();

#pragma warning disable CS8604
            System.Windows.Forms.Application.Run(ServiceProvider.GetService<Main>());
#pragma warning restore CS8604
        }

        private static async Task ConfigureServicesAsync(bool needMigrate)
        {
            var services = new ServiceCollection();

            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();

            services.AddSingleton<IConfiguration>(configuration);

            services.AddLogging(builder =>
            {
                builder.AddConsole();
                builder.AddDebug();
                builder.SetMinimumLevel(LogLevel.Information);
            });

            var databaseSettings = configuration.GetSection(DatabaseSettings.SectionName).Get<DatabaseSettings>();
            var databasePath = databaseSettings?.ConnectionString ?? "./data/catfood.db";

            services.AddInfrastructureSettings(configuration);
            services.AddInfrastructure(databasePath);
            services.AddApplicationServices();

            var appSettings = configuration.GetSection(AppSettings.SectionName).Get<AppSettings>();
            var tessdataPath = appSettings?.TessdataPath ?? Path.Combine(AppContext.BaseDirectory, "tessdata");

            services.AddGeminiAgent(configuration, "AppSettings:AI");

            services.AddSingleton<Main>()
                    .AddSingleton<MainViewModel>()
                    .AddScoped<BrandManager>()
                    .AddScoped<LowestPrice>()
                    .AddSingleton<SQLiteHelper>()
                    .AddScoped(serviceProvider => new OCRHelper(tessdataPath))
                    .AddScoped<IRepository, CommonRepository>()
                    .AddScoped<PictureContentService>()
                    .AddScoped<IService<Brand>, BrandService>(serviceProvider => new BrandService(serviceProvider.GetRequiredService<IRepository>(), needMigrate))
                    .AddScoped<IService<CatFood>, CatFoodService>(serviceProvider => new CatFoodService(serviceProvider.GetRequiredService<IRepository>(), needMigrate))
                    .AddScoped<IService<Factory>, FactoryService>(serviceProvider => new FactoryService(serviceProvider.GetRequiredService<IRepository>(), needMigrate))
                    .AddScoped<IService<BestPrice>, LowestPriceService>(serviceProvider => new LowestPriceService(serviceProvider.GetRequiredService<IRepository>(), needMigrate))
                    .AddScoped<ICorePlatformRegExpService, PlatformRegExpService>(serviceProvider => new PlatformRegExpService(serviceProvider.GetRequiredService<IRepository>(), needMigrate))
                    .AddScoped<ICoreGeminiOcrService, CatFoodManager.Core.Services.GeminiOcrService>(sp =>
                    {
                        var repo = sp.GetRequiredService<IRepository>();
                        var agentService = sp.GetRequiredService<IGeminiAgentService>();
                        return new CatFoodManager.Core.Services.GeminiOcrService(repo, agentService, needMigrate);
                    })
                    .AddSingleton<IMemoryCache, MemoryCache>()
                    .AddScoped<IApplicationGeminiOcrService, CatFoodManager.Application.Services.GeminiOcrService>(sp =>
                    {
                        var agentService = sp.GetRequiredService<IGeminiAgentService>();
                        var repository = sp.GetRequiredService<IRepository>();
                        var logger = sp.GetRequiredService<ILogger<CatFoodManager.Application.Services.GeminiOcrService>>();
                        var cache = sp.GetRequiredService<IMemoryCache>();
                        return new CatFoodManager.Application.Services.GeminiOcrService(agentService, repository, logger, cache);
                    });

            ServiceProvider = services.BuildServiceProvider();
        }

        public static T? GetService<T>() where T : class
        {
            return (T?)ServiceProvider.GetService(typeof(T));
        }
    }
}