using CatFoodManager.Application.Interfaces;
using CatFoodManager.Domain.Interfaces;
using CatFoodManager.Infrastructure.Caching;
using CatFoodManager.Infrastructure.Configuration;
using CatFoodManager.Infrastructure.Persistence;
using CatFoodManager.Infrastructure.Repositories;
using CatFoodManager.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CatFoodManager.Infrastructure.Extensions;

/// <summary>
/// 服务集合扩展方法类，提供基础设施层服务的注册�?
/// Service collection extension methods class, providing infrastructure layer service registration.
/// </summary>
public static class ServiceCollectionExtensions
{/// <summary>
 /// 注册基础设施层服务�?
 /// Registers infrastructure layer services.
 /// </summary>
 /// <param name="services">服务集合 / Service collection</par
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

        services.AddSingleton<TaskBackgroundService>();
        services.AddSingleton<IBackgroundServiceControl>(sp => sp.GetRequiredService<TaskBackgroundService>());

        return services;
    }

    /// <summary>
    /// 注册基础设施层配置设置�?
    /// Registers infrastructure layer configuration settings.
    /// </summary>
    /// <param name="services">服务集合 / Service collection</param>
    /// <param name="configuration">配置对象 / Configuration object</param>
    /// <returns>服务集合 / Service collection</returns>
    public static IServiceCollection AddInfrastructureSettings(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<DatabaseSettings>()
            .Bind(configuration.GetSection(DatabaseSettings.SectionName));

        services.AddOptions<GeminiSettings>()
            .Bind(configuration.GetSection(GeminiSettings.SectionName));

        services.AddOptions<AppSettings>()
            .Bind(configuration.GetSection(AppSettings.SectionName));

        return services;
    }
}
