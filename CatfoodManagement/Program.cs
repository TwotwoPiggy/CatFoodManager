using CatFoodManager.Application.Extensions;
using CatFoodManager.Core.Interfaces;
using CatFoodManager.Core.Models;
using CatFoodManager.Core.Repositories;
using CatFoodManager.Core.Services;
using CatFoodManager.Infrastructure.Configuration;
using CatFoodManager.Infrastructure.Extensions;
using CatfoodManagement.Services;
using CefSharp;
using CommonTools;
using CommonTools.Database;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CatfoodManagement
{
    internal static class Program
    {
        public static IServiceProvider ServiceProvider { get; set; } = null!;
        public static IConfiguration Configuration { get; set; } = null!;

        [STAThread]
        static void Main()
        {
            CefSharpSettings.ConcurrentTaskExecution = true;
            ApplicationConfiguration.Initialize();
            ConfigureServices();

            var mainForm = ServiceProvider.GetRequiredService<MainForm>();
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

            services.AddSingleton<MainForm>();
            services.AddSingleton<JavaScriptBridge>();
            services.AddScoped<SQLiteHelper>();
            services.AddScoped<IRepository, CommonRepository>();
            services.AddScoped<PictureContentService>();
            services.AddScoped<IService<Brand>, BrandService>(serviceProvider => new BrandService(serviceProvider.GetRequiredService<IRepository>(), true));
            services.AddScoped<IService<CatFood>, CatFoodService>(serviceProvider => new CatFoodService(serviceProvider.GetRequiredService<IRepository>(), true));
            services.AddScoped<IService<Factory>, FactoryService>(serviceProvider => new FactoryService(serviceProvider.GetRequiredService<IRepository>(), true));
            services.AddScoped<IService<BestPrice>, LowestPriceService>(serviceProvider => new LowestPriceService(serviceProvider.GetRequiredService<IRepository>(), true));

            ServiceProvider = services.BuildServiceProvider();
        }

        public static T? GetService<T>() where T : class
        {
            return (T?)ServiceProvider.GetService(typeof(T));
        }
    }
}
