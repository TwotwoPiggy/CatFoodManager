using CatFoodManager.Application.Interfaces;
using CatFoodManager.Application.Services;
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
        services.AddScoped<IGeminiOcrService, GeminiOcrService>();
        services.AddScoped<IPlatformRegExpService, PlatformRegExpService>();

        return services;
    }
}
