using CatFoodManager.Infrastructure.Caching;
using Microsoft.Extensions.Caching.Memory;
using Xunit;

namespace CatFoodManager.Infrastructure.Tests.Caching;

public class MemoryCacheServiceTests : IDisposable
{
    private readonly MemoryCache _memoryCache;
    private readonly MemoryCacheService _cacheService;

    public MemoryCacheServiceTests()
    {
        _memoryCache = new MemoryCache(new MemoryCacheOptions());
        _cacheService = new MemoryCacheService(_memoryCache);
    }

    [Fact]
    public async Task GetAsync_WhenKeyNotExists_ShouldReturnDefault()
    {
        var result = await _cacheService.GetAsync<string>("nonexistent");

        Assert.Null(result);
    }

    [Fact]
    public async Task SetAsync_ThenGetAsync_ShouldReturnCachedValue()
    {
        var key = "test-key";
        var value = "test-value";

        await _cacheService.SetAsync(key, value);
        var result = await _cacheService.GetAsync<string>(key);

        Assert.Equal(value, result);
    }

    [Fact]
    public async Task SetAsync_WithExpiration_ShouldExpireAfterTime()
    {
        var key = "expiring-key";
        var value = "expiring-value";
        var expiration = TimeSpan.FromMilliseconds(100);

        await _cacheService.SetAsync(key, value, expiration);
        await Task.Delay(150);
        var result = await _cacheService.GetAsync<string>(key);

        Assert.Null(result);
    }

    [Fact]
    public async Task RemoveAsync_ShouldRemoveCachedValue()
    {
        var key = "remove-key";
        var value = "remove-value";

        await _cacheService.SetAsync(key, value);
        await _cacheService.RemoveAsync(key);
        var result = await _cacheService.GetAsync<string>(key);

        Assert.Null(result);
    }

    [Fact]
    public async Task ExistsAsync_WhenKeyExists_ShouldReturnTrue()
    {
        var key = "exists-key";
        var value = "exists-value";

        await _cacheService.SetAsync(key, value);
        var result = await _cacheService.ExistsAsync(key);

        Assert.True(result);
    }

    [Fact]
    public async Task ExistsAsync_WhenKeyNotExists_ShouldReturnFalse()
    {
        var result = await _cacheService.ExistsAsync("nonexistent");

        Assert.False(result);
    }

    [Fact]
    public async Task GetOrCreateAsync_WhenKeyExists_ShouldReturnCachedValue()
    {
        var key = "cached-key";
        var cachedValue = "cached-value";
        var factoryCalled = false;

        await _cacheService.SetAsync(key, cachedValue);
        var result = await _cacheService.GetOrCreateAsync(key, () =>
        {
            factoryCalled = true;
            return Task.FromResult("factory-value");
        });

        Assert.Equal(cachedValue, result);
        Assert.False(factoryCalled);
    }

    [Fact]
    public async Task GetOrCreateAsync_WhenKeyNotExists_ShouldCallFactoryAndCache()
    {
        var key = "factory-key";
        var factoryValue = "factory-value";
        var factoryCalled = false;

        var result = await _cacheService.GetOrCreateAsync(key, () =>
        {
            factoryCalled = true;
            return Task.FromResult(factoryValue);
        });

        Assert.Equal(factoryValue, result);
        Assert.True(factoryCalled);

        var cachedResult = await _cacheService.GetAsync<string>(key);
        Assert.Equal(factoryValue, cachedResult);
    }

    [Fact]
    public async Task GetOrCreateAsync_WithExpiration_ShouldCacheWithExpiration()
    {
        var key = "expiring-factory-key";
        var factoryValue = "expiring-factory-value";
        var expiration = TimeSpan.FromMilliseconds(100);

        await _cacheService.GetOrCreateAsync(key, () => Task.FromResult(factoryValue), expiration);
        await Task.Delay(150);
        var result = await _cacheService.GetAsync<string>(key);

        Assert.Null(result);
    }

    [Fact]
    public async Task SetAsync_WithComplexObject_ShouldCacheSuccessfully()
    {
        var key = "complex-key";
        var value = new TestObject { Id = 1, Name = "Test" };

        await _cacheService.SetAsync(key, value);
        var result = await _cacheService.GetAsync<TestObject>(key);

        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Test", result.Name);
    }

    public void Dispose()
    {
        _memoryCache?.Dispose();
    }

    private class TestObject
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
