using Microsoft.Extensions.Caching.Memory;

namespace CatFoodManager.Infrastructure.Caching;

/// <summary>
/// 内存缓存服务实现，提供基于内存的缓存操作。
/// Memory cache service implementation, providing memory-based cache operations.
/// </summary>
public class MemoryCacheService : ICacheService
{
    private readonly IMemoryCache _cache;

    /// <summary>
    /// 默认缓存过期时间。
    /// Default cache expiration time.
    /// </summary>
    private static readonly TimeSpan DefaultExpiration = TimeSpan.FromMinutes(30);

    /// <summary>
    /// 构造函数。
    /// Constructor.
    /// </summary>
    /// <param name="cache">内存缓存实例 / Memory cache instance</param>
    public MemoryCacheService(IMemoryCache cache)
    {
        _cache = cache;
    }

    /// <summary>
    /// 获取缓存值。
    /// Gets a cached value.
    /// </summary>
    /// <typeparam name="T">值类型 / Value type</typeparam>
    /// <param name="key">缓存键 / Cache key</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    /// <returns>缓存值或默认值 / Cached value or default</returns>
    public Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        _cache.TryGetValue(key, out T? value);
        return Task.FromResult(value);
    }

    /// <summary>
    /// 设置缓存值。
    /// Sets a cached value.
    /// </summary>
    /// <typeparam name="T">值类型 / Value type</typeparam>
    /// <param name="key">缓存键 / Cache key</param>
    /// <param name="value">缓存值 / Cached value</param>
    /// <param name="expiration">过期时间 / Expiration time</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    public Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default)
    {
        var options = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = expiration ?? DefaultExpiration
        };

        _cache.Set(key, value, options);
        return Task.CompletedTask;
    }

    /// <summary>
    /// 移除缓存值。
    /// Removes a cached value.
    /// </summary>
    /// <param name="key">缓存键 / Cache key</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    public Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        _cache.Remove(key);
        return Task.CompletedTask;
    }

    /// <summary>
    /// 检查缓存键是否存在。
    /// Checks if a cache key exists.
    /// </summary>
    /// <param name="key">缓存键 / Cache key</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    /// <returns>是否存在 / Whether exists</returns>
    public Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_cache.TryGetValue(key, out _));
    }

    /// <summary>
    /// 获取或创建缓存值。
    /// Gets or creates a cached value.
    /// </summary>
    /// <typeparam name="T">值类型 / Value type</typeparam>
    /// <param name="key">缓存键 / Cache key</param>
    /// <param name="factory">值工厂函数 / Value factory function</param>
    /// <param name="expiration">过期时间 / Expiration time</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    /// <returns>缓存值 / Cached value</returns>
    public async Task<T> GetOrCreateAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiration = null, CancellationToken cancellationToken = default)
    {
        if (_cache.TryGetValue(key, out T? cachedValue) && cachedValue != null)
        {
            return cachedValue;
        }

        var value = await factory().ConfigureAwait(false);

        var options = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = expiration ?? DefaultExpiration
        };

        _cache.Set(key, value, options);

        return value;
    }
}
