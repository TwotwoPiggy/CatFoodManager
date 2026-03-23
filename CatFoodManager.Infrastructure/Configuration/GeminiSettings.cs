namespace CatFoodManager.Infrastructure.Configuration;

/// <summary>
/// Gemini AI设置类，包含Gemini API相关配置。
/// Gemini AI settings class, containing Gemini API related configuration.
/// </summary>
public class GeminiSettings
{
    /// <summary>
    /// 配置节名称。
    /// Configuration section name.
    /// </summary>
    public const string SectionName = "Gemini";

    /// <summary>
    /// Gemini API密钥。
    /// Gemini API key.
    /// </summary>
    public string ApiKey { get; set; } = string.Empty;

    /// <summary>
    /// 使用的模型名称。
    /// Model name to use.
    /// </summary>
    public string Model { get; set; } = "gemini-2.0-flash";

    /// <summary>
    /// API基础URL。
    /// API base URL.
    /// </summary>
    public string BaseUrl { get; set; } = "https://generativelanguage.googleapis.com/v1beta";

    /// <summary>
    /// 请求超时时间（秒）。
    /// Request timeout in seconds.
    /// </summary>
    public int TimeoutSeconds { get; set; } = 120;

    /// <summary>
    /// 最大重试次数。
    /// Maximum retry count.
    /// </summary>
    public int MaxRetries { get; set; } = 3;

    /// <summary>
    /// 是否启用缓存。
    /// Whether to enable caching.
    /// </summary>
    public bool EnableCache { get; set; } = true;

    /// <summary>
    /// 缓存过期时间（分钟）。
    /// Cache expiration time in minutes.
    /// </summary>
    public int CacheExpirationMinutes { get; set; } = 60;
}
