namespace CatFoodManager.Infrastructure.Caching;

public static class CacheKeyManager
{
    private const string Separator = ":";
    
    public static string ForEntity<T>(long id) => $"entity{Separator}{typeof(T).Name.ToLower()}{Separator}{id}";
    
    public static string ForList<T>(string? suffix = null) 
    {
        var baseKey = $"list{Separator}{typeof(T).Name.ToLower()}";
        return string.IsNullOrEmpty(suffix) ? baseKey : $"{baseKey}{Separator}{suffix}";
    }
    
    public static string ForSearch<T>(string keyword) 
        => $"search{Separator}{typeof(T).Name.ToLower()}{Separator}{keyword.ToLower()}";
    
    public static string ForPaged<T>(int page, int pageSize) 
        => $"paged{Separator}{typeof(T).Name.ToLower()}{Separator}{page}{Separator}{pageSize}";
}
