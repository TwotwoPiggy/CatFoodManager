using CatFoodManager.Domain.Entities;
using CatFoodManager.Domain.Interfaces;
using CatFoodManager.Infrastructure.Caching;
using CatFoodManager.Infrastructure.Configuration;
using CatFoodManager.Infrastructure.Extensions;
using CatFoodManager.Infrastructure.Persistence;
using CatFoodManager.Infrastructure.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Xunit;

namespace CatFoodManager.Infrastructure.Tests.Extensions;

public class ServiceCollectionExtensionsTests : IDisposable
{
    private readonly string _testDbPath;
    private readonly IServiceCollection _services;

    public ServiceCollectionExtensionsTests()
    {
        _testDbPath = Path.Combine(Path.GetTempPath(), $"test_{Guid.NewGuid()}.db");
        _services = new ServiceCollection();
    }

    [Fact]
    public void AddInfrastructure_ShouldRegisterIDbContext()
    {
        _services.AddInfrastructure(_testDbPath);

        var serviceProvider = _services.BuildServiceProvider();
        var dbContext = serviceProvider.GetService<IDbContext>();

        Assert.NotNull(dbContext);
        Assert.IsType<DbContext>(dbContext);
    }

    [Fact]
    public void AddInfrastructure_ShouldRegisterIUnitOfWork()
    {
        _services.AddInfrastructure(_testDbPath);

        var serviceProvider = _services.BuildServiceProvider();
        var unitOfWork = serviceProvider.GetService<IUnitOfWork>();

        Assert.NotNull(unitOfWork);
        Assert.IsType<UnitOfWork>(unitOfWork);
    }

    [Fact]
    public void AddInfrastructure_ShouldRegisterGenericRepository()
    {
        _services.AddInfrastructure(_testDbPath);

        var serviceProvider = _services.BuildServiceProvider();
        var repository = serviceProvider.GetService<IRepository<CatFood>>();

        Assert.NotNull(repository);
        Assert.IsType<SQLiteRepository<CatFood>>(repository);
    }

    [Fact]
    public void AddInfrastructure_ShouldReturnServiceCollection()
    {
        var result = _services.AddInfrastructure(_testDbPath);

        Assert.Same(_services, result);
    }

    [Fact]
    public void AddInfrastructure_IDbContextShouldBeSingleton()
    {
        _services.AddInfrastructure(_testDbPath);

        var serviceProvider = _services.BuildServiceProvider();
        var dbContext1 = serviceProvider.GetService<IDbContext>();
        var dbContext2 = serviceProvider.GetService<IDbContext>();

        Assert.Same(dbContext1, dbContext2);
    }

    [Fact]
    public void AddInfrastructure_IUnitOfWorkShouldBeScoped()
    {
        _services.AddInfrastructure(_testDbPath);

        using var scope1 = _services.BuildServiceProvider().CreateScope();
        using var scope2 = _services.BuildServiceProvider().CreateScope();

        var unitOfWork1 = scope1.ServiceProvider.GetService<IUnitOfWork>();
        var unitOfWork2 = scope2.ServiceProvider.GetService<IUnitOfWork>();

        Assert.NotSame(unitOfWork1, unitOfWork2);
    }

    [Fact]
    public void AddInfrastructure_RepositoryShouldBeScoped()
    {
        _services.AddInfrastructure(_testDbPath);

        using var scope1 = _services.BuildServiceProvider().CreateScope();
        using var scope2 = _services.BuildServiceProvider().CreateScope();

        var repo1 = scope1.ServiceProvider.GetService<IRepository<CatFood>>();
        var repo2 = scope2.ServiceProvider.GetService<IRepository<CatFood>>();

        Assert.NotSame(repo1, repo2);
    }

    [Fact]
    public void AddInfrastructureSettings_ShouldRegisterDatabaseSettings()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Database:ConnectionString"] = "Data Source=test.db",
                ["Database:DatabaseType"] = "SQLite"
            })
            .Build();

        _services.AddInfrastructureSettings(configuration);

        var serviceProvider = _services.BuildServiceProvider();
        var options = serviceProvider.GetService<IOptions<DatabaseSettings>>();

        Assert.NotNull(options);
        Assert.Equal("Data Source=test.db", options.Value.ConnectionString);
        Assert.Equal("SQLite", options.Value.DatabaseType);
    }

    [Fact]
    public void AddInfrastructureSettings_ShouldRegisterGeminiSettings()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Gemini:ApiKey"] = "test-key",
                ["Gemini:Model"] = "gemini-2.0-flash",
                ["Gemini:MaxRetries"] = "3",
                ["Gemini:TimeoutSeconds"] = "30"
            })
            .Build();

        _services.AddInfrastructureSettings(configuration);

        var serviceProvider = _services.BuildServiceProvider();
        var options = serviceProvider.GetService<IOptions<GeminiSettings>>();

        Assert.NotNull(options);
        Assert.Equal("test-key", options.Value.ApiKey);
        Assert.Equal("gemini-2.0-flash", options.Value.Model);
        Assert.Equal(3, options.Value.MaxRetries);
        Assert.Equal(30, options.Value.TimeoutSeconds);
    }

    [Fact]
    public void AddInfrastructureSettings_ShouldRegisterAppSettings()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["AppSettings:PlatformFolders:京东"] = "/path/to/jd",
                ["AppSettings:PlatformFolders:淘宝"] = "/path/to/taobao",
                ["AppSettings:TessdataPath"] = "/path/to/tessdata"
            })
            .Build();

        _services.AddInfrastructureSettings(configuration);

        var serviceProvider = _services.BuildServiceProvider();
        var options = serviceProvider.GetService<IOptions<AppSettings>>();

        Assert.NotNull(options);
        Assert.Equal("/path/to/jd", options.Value.PlatformFolders["京东"]);
        Assert.Equal("/path/to/taobao", options.Value.PlatformFolders["淘宝"]);
        Assert.Equal("/path/to/tessdata", options.Value.TessdataPath);
    }

    [Fact]
    public void AddInfrastructureSettings_ShouldReturnServiceCollection()
    {
        var configuration = new ConfigurationBuilder().Build();

        var result = _services.AddInfrastructureSettings(configuration);

        Assert.Same(_services, result);
    }

    [Fact]
    public void AddInfrastructure_ShouldRegisterICacheService()
    {
        _services.AddInfrastructure(_testDbPath);

        var serviceProvider = _services.BuildServiceProvider();
        var cacheService = serviceProvider.GetService<ICacheService>();

        Assert.NotNull(cacheService);
        Assert.IsType<MemoryCacheService>(cacheService);
    }

    [Fact]
    public void AddInfrastructure_ICacheServiceShouldBeSingleton()
    {
        _services.AddInfrastructure(_testDbPath);

        var serviceProvider = _services.BuildServiceProvider();
        var cacheService1 = serviceProvider.GetService<ICacheService>();
        var cacheService2 = serviceProvider.GetService<ICacheService>();

        Assert.Same(cacheService1, cacheService2);
    }

    public void Dispose()
    {
        if (File.Exists(_testDbPath))
        {
            File.Delete(_testDbPath);
        }
    }
}
