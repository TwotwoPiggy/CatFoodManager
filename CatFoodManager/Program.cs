using CatFoodManager.Core.Interfaces;
using CatFoodManager.Core.Models;
using CatFoodManager.Core.Repositories;
using CatFoodManager.Core.Services;
using CommonTools;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OcrApi;

namespace CatFoodManager
{
    internal static class Program
    {
        public static IServiceProvider ServiceProvider { get; set; }

        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {

            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            ConfigureServices(needMigrate: true);


#pragma warning disable CS8604 // 引用类型参数可能为 null。
            Application.Run(ServiceProvider.GetService<Main>());
#pragma warning restore CS8604 // 引用类型参数可能为 null。
        }

        private static void ConfigureServices(bool needMigrate)
        {
            var services = new ServiceCollection();
            // Load configuration from appsettings.json
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();

            // read tessdataPath and sqlite connection string from configuration
            var tessdataPath = configuration.GetSection("AppSettings")?["TessdataPath"] ?? Path.Combine(AppContext.BaseDirectory, "tessdata");

            services.AddSingleton<IConfiguration>(configuration);
            services.AddSingleton(typeof(Main))
                    .AddScoped(typeof(BrandManager))
                    .AddScoped(typeof(LowestPrice))
                    .AddScoped<SQLiteHelper>()
                    .AddScoped(serviceProvider => new OCRHelper(tessdataPath))
                    .AddScoped<IRepository, CommonRepository>()
                    .AddScoped<PictureContentService>()
                    .AddScoped<IService<Brand>, BrandService>(serviceProvider => new BrandService(serviceProvider.GetRequiredService<IRepository>(), needMigrate))
                    .AddScoped<IService<CatFood>, CatFoodService>(serviceProvider => new CatFoodService(serviceProvider.GetRequiredService<IRepository>(), needMigrate))
                    .AddScoped<IService<Factory>, FactoryService>(serviceProvider => new FactoryService(serviceProvider.GetRequiredService<IRepository>(), needMigrate))
                    .AddScoped<IService<BestPrice>, LowestPriceService>(serviceProvider => new LowestPriceService(serviceProvider.GetRequiredService<IRepository>(), needMigrate))
                    .AddScoped<IPlatformRegExpService, PlatformRegExpService>(serviceProvider => new PlatformRegExpService(serviceProvider.GetRequiredService<IRepository>(), needMigrate));
            ServiceProvider = services.BuildServiceProvider();

        }

        public static T? GetService<T>() where T : class
        {
            return (T?)ServiceProvider.GetService(typeof(T));
        }

    }
}