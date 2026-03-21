using CatFoodManager.Domain.Entities;
using CatFoodManager.Domain.Interfaces;
using CatFoodManager.Infrastructure.Caching;
using CatFoodManager.Infrastructure.Configuration;
using CatFoodManager.Infrastructure.Persistence;
using CatFoodManager.Infrastructure.Repositories;
using CatFoodManager.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace CatFoodManager.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        string databasePath)
    {
        services.AddSingleton<IDbContext>(sp => new DbContext(databasePath));
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddMemoryCache();
        services.AddSingleton<ICacheService, MemoryCacheService>();

        services.AddScoped(typeof(IRepository<>), typeof(SQLiteRepository<>));
        services.AddScoped<IDatabaseInitializer, DatabaseInitializer>();

        services.AddHostedService<TaskBackgroundService>();

        return services;
    }

    public static IServiceCollection AddInfrastructureSettings(
        this IServiceCollection services,
        Microsoft.Extensions.Configuration.IConfiguration configuration)
    {
        services.AddOptions<DatabaseSettings>()
            .Bind(configuration.GetSection(DatabaseSettings.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddOptions<GeminiSettings>()
            .Bind(configuration.GetSection(GeminiSettings.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddOptions<AppSettings>()
            .Bind(configuration.GetSection(AppSettings.SectionName));

        return services;
    }
}
