using CatFoodManager.Application.Interfaces;
using CatFoodManager.Application.Services;
using CatFoodManager.Application.Services.Handlers;
using Microsoft.Extensions.DependencyInjection;

namespace CatFoodManager.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<ICatFoodService, CatFoodService>();
        services.AddScoped<IBrandService, BrandService>();
        services.AddScoped<IFactoryService, FactoryService>();
        services.AddScoped<IBestPriceService, BestPriceService>();
        services.AddScoped<IOcrPromptService, OcrPromptService>();

        services.AddScoped<ITaskService, TaskService>();
        services.AddScoped<ITaskScheduler, Services.TaskScheduler>();
        services.AddScoped<ITaskExecutor, TaskExecutor>();

        services.AddScoped<ITaskHandler, SyncTaskHandler>();
        services.AddScoped<ITaskHandler, ImageTaskHandler>();
        services.AddScoped<ITaskHandler, ImageProcessHandler>();

        return services;
    }
}
