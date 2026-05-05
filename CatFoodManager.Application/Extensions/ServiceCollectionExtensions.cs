using CatFoodManager.Application.Interfaces;
using CatFoodManager.Application.Services;
using CatFoodManager.Application.Services.Handlers;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CatFoodManager.Application.Extensions;

/// <summary>
/// 服务集合扩展方法类。
/// Service collection extension methods class.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// 注册应用层服务。
    /// Registers application layer services.
    /// </summary>
    /// <param name="services">服务集合 / Service collection</param>
    /// <returns>服务集合 / Service collection</returns>
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<ICatFoodService, CatFoodService>();
        services.AddScoped<IBrandService, BrandService>();
        services.AddScoped<IFactoryService, FactoryService>();
        services.AddScoped<IBestPriceService, BestPriceService>();
        services.AddScoped<IOcrPromptService, OcrPromptService>();
        services.AddScoped<IPlatformRegExpService, PlatformRegExpService>();

        services.AddScoped<ITaskService, TaskService>();
        services.AddSingleton<ITaskScheduler, Services.TaskScheduler>();
        services.AddSingleton<ITaskExecutor, TaskExecutor>();

        services.AddScoped<ITaskHandler, SyncTaskHandler>();
        services.AddScoped<ITaskHandler, ImageTaskHandler>();
        services.AddScoped<ITaskHandler, ImageProcessHandler>();

        return services;
    }

    /// <summary>
    /// 注册 Gemini OCR 服务。
    /// Registers Gemini OCR service.
    /// </summary>
    /// <param name="services">服务集合 / Service collection</param>
    /// <returns>服务集合 / Service collection</returns>
    public static IServiceCollection AddGeminiOcrService(this IServiceCollection services)
    {
        services.AddSingleton<IMemoryCache, MemoryCache>();
        services.AddScoped<IGeminiOcrService, GeminiOcrService>();

        return services;
    }
}
