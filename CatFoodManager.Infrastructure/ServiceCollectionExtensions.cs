using CatFoodManager.Infrastructure.Exceptions;
using Microsoft.Extensions.DependencyInjection;

namespace CatFoodManager.Infrastructure;

/// <summary>
/// 服务集合扩展方法类，提供基础设施层服务的注册。
/// Service collection extension methods class, providing infrastructure layer service registration.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// 注册基础设施层服务。
    /// Registers infrastructure layer services.
    /// </summary>
    /// <param name="services">服务集合 / Service collection</param>
    /// <returns>服务集合 / Service collection</returns>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<ExceptionHandler>();

        return services;
    }
}
