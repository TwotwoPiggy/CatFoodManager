namespace CatFoodManager.Infrastructure.Caching;

/// <summary>
/// 缓存键管理器，用于生成和管理缓存键。
/// Cache key manager, used to generate and manage cache keys.
/// </summary>
public static class CacheKeyManager
{
    /// <summary>
    /// 生成品牌缓存键。
    /// Generates a brand cache key.
    /// </summary>
    /// <param name="id">品牌ID / Brand ID</param>
    /// <returns>缓存键 / Cache key</returns>
    public static string BrandKey(long id) => $"brand_{id}";

    /// <summary>
    /// 生成品牌列表缓存键。
    /// Generates a brand list cache key.
    /// </summary>
    /// <returns>缓存键 / Cache key</returns>
    public static string BrandListKey() => "brand_list";

    /// <summary>
    /// 生成猫粮缓存键。
    /// Generates a cat food cache key.
    /// </summary>
    /// <param name="id">猫粮ID / Cat food ID</param>
    /// <returns>缓存键 / Cache key</returns>
    public static string CatFoodKey(long id) => $"catfood_{id}";

    /// <summary>
    /// 生成猫粮列表缓存键。
    /// Generates a cat food list cache key.
    /// </summary>
    /// <param name="page">页码 / Page number</param>
    /// <param name="pageSize">每页大小 / Page size</param>
    /// <returns>缓存键 / Cache key</returns>
    public static string CatFoodListKey(int page, int pageSize) => $"catfood_list_{page}_{pageSize}";

    /// <summary>
    /// 生成最低价格缓存键。
    /// Generates a best price cache key.
    /// </summary>
    /// <param name="id">最低价格ID / Best price ID</param>
    /// <returns>缓存键 / Cache key</returns>
    public static string BestPriceKey(long id) => $"bestprice_{id}";

    /// <summary>
    /// 生成最低价格列表缓存键。
    /// Generates a best price list cache key.
    /// </summary>
    /// <param name="page">页码 / Page number</param>
    /// <param name="pageSize">每页大小 / Page size</param>
    /// <returns>缓存键 / Cache key</returns>
    public static string BestPriceListKey(int page, int pageSize) => $"bestprice_list_{page}_{pageSize}";
}
