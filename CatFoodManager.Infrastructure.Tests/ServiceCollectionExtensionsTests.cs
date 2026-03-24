using CatFoodManager.Infrastructure;
using CatFoodManager.Infrastructure.Exceptions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit;

namespace CatFoodManager.Infrastructure.Tests;

public class ServiceCollectionExtensionsTests
{
    [Fact]
    public void AddInfrastructure_ShouldRegisterExceptionHandler()
    {
        var services = new ServiceCollection();
        services.AddLogging();

        services.AddInfrastructure();

        var serviceProvider = services.BuildServiceProvider();
        var handler = serviceProvider.GetService<ExceptionHandler>();

        Assert.NotNull(handler);
    }

    [Fact]
    public void AddInfrastructure_ShouldRegisterExceptionHandlerAsSingleton()
    {
        var services = new ServiceCollection();
        services.AddLogging();

        services.AddInfrastructure();

        var serviceProvider = services.BuildServiceProvider();
        var handler1 = serviceProvider.GetService<ExceptionHandler>();
        var handler2 = serviceProvider.GetService<ExceptionHandler>();

        Assert.Same(handler1, handler2);
    }

    [Fact]
    public void AddInfrastructure_ShouldReturnSameServiceCollection()
    {
        var services = new ServiceCollection();

        var result = services.AddInfrastructure();

        Assert.Same(services, result);
    }
}
