using CatFoodManager.Infrastructure.Exceptions;
using Microsoft.Extensions.DependencyInjection;

namespace CatFoodManager.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<ExceptionHandler>();

        return services;
    }
}
