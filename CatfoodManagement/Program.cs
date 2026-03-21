using CatFoodManager.Application.Extensions;
using CatFoodManager.Application.Interfaces;
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
    /// <summary>
    /// 应用程序入口类，负责配置依赖注入和启动应用程序
    /// </summary>
    internal static class Program
    {
        /// <summary>
        /// 全局服务提供者，用于获取已注册的服务
        /// </summary>
        public static IServiceProvider ServiceProvider { get; set; } = null!;
        
        /// <summary>
        /// 应用程序配置，用于读取 appsettings.json
        /// </summary>
        public static IConfiguration Configuration { get; set; } = null!;

        /// <summary>
        /// 应用程序入口点
        /// </summary>
        [STAThread]
        static void Main()
        {
            // 启用 CefSharp 并发任务执行
            CefSharpSettings.ConcurrentTaskExecution = true;
            // 初始化 Windows Forms 应用程序配置
            ApplicationConfiguration.Initialize();
            // 配置依赖注入服务
            ConfigureServices();

            // 获取主窗体实例并运行应用程序
            var mainForm = ServiceProvider.GetRequiredService<MainForm>();
            Application.Run(mainForm);
        }

        /// <summary>
        /// 配置依赖注入服务
        /// </summary>
        private static void ConfigureServices()
        {
            var services = new ServiceCollection();

            // 加载配置文件
            Configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();

            // 注册配置服务
            services.AddSingleton<IConfiguration>(Configuration);

            // 配置日志服务
            services.AddLogging(builder =>
            {
                builder.AddConsole();
                builder.AddDebug();
                builder.SetMinimumLevel(LogLevel.Information);
            });

            // 获取数据库连接字符串
            var databaseSettings = Configuration.GetSection(DatabaseSettings.SectionName).Get<DatabaseSettings>();
            var databasePath = databaseSettings?.ConnectionString ?? "./data/catfood.db";

            // 注册基础设施服务
            services.AddInfrastructureSettings(Configuration);
            services.AddInfrastructure(databasePath);
            services.AddApplicationServices();

            // 注册 Gemini AI Agent 服务
            services.AddGeminiAgent(Configuration, "AppSettings:AI");

            // 注册主窗体和桥接器（单例）
            services.AddSingleton<MainForm>();
            services.AddSingleton<JavaScriptBridge>();
            
            // 注册数据库相关服务（作用域）
            services.AddScoped<SQLiteHelper>();
            services.AddScoped<IRepository, CommonRepository>();
            services.AddScoped<PictureContentService>();
            
            // 注册业务服务（作用域）
            services.AddScoped<IService<Brand>, BrandService>(serviceProvider => new BrandService(serviceProvider.GetRequiredService<IRepository>(), true));
            services.AddScoped<IService<CatFood>, CatFoodService>(serviceProvider => new CatFoodService(serviceProvider.GetRequiredService<IRepository>(), true));
            services.AddScoped<IService<Factory>, FactoryService>(serviceProvider => new FactoryService(serviceProvider.GetRequiredService<IRepository>(), true));
            services.AddScoped<IService<BestPrice>, LowestPriceService>(serviceProvider => new LowestPriceService(serviceProvider.GetRequiredService<IRepository>(), true));
            
            // 注册 OCR 服务
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

            // 构建服务提供者
            ServiceProvider = services.BuildServiceProvider();
        }

        /// <summary>
        /// 获取指定类型的服务实例
        /// </summary>
        /// <typeparam name="T">服务类型</typeparam>
        /// <returns>服务实例，如果未注册则返回 null</returns>
        public static T? GetService<T>() where T : class
        {
            return (T?)ServiceProvider.GetService(typeof(T));
        }
    }
}
