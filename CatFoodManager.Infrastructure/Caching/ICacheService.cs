namespace CatFoodManager.Infrastructure.Caching;

/// <summary>
/// 缓存服务接口，提供缓存操作的标准定义。
/// Cache service interface, providing standard definitions for cache operations.
/// </summary>
public interface ICacheService
{
    /// <summary>
    /// 获取缓存值。
    /// Gets a cached value.
    /// </summary>
    /// <typeparam name="T">值类型 / Value type</typeparam>
    /// <param name="key">缓存键 / Cache key</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    /// <returns>缓存值或默认值 / Cached value or default</returns>
    Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default);

    /// <summary>
    /// 设置缓存值。
    /// Sets a cached value.
    /// </summary>
    /// <typeparam name="T">值类型 / Value type</typeparam>
    /// <param name="key">缓存键 / Cache key</param>
    /// <param name="value">缓存值 / Cached value</param>
    /// <param name="expiration">过期时间 / Expiration time</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// 移除缓存值。
    /// Removes a cached value.
    /// </summary>
    /// <param name="key">缓存键 / Cache key</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    Task RemoveAsync(string key, CancellationToken cancellationToken = default);

    /// <summary>
    /// 检查缓存键是否存在。
    /// Checks if a cache key exists.
    /// </summary>
    /// <param name="key">缓存键 / Cache key</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    /// <returns>是否存在 / Whether exists</returns>
    Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default);

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
    Task<T> GetOrCreateAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiration = null, CancellationToken cancellationToken = default);
}
