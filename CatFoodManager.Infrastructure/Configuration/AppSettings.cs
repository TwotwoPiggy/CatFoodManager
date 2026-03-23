namespace CatFoodManager.Infrastructure.Configuration;

/// <summary>
/// 应用程序设置类，包含应用程序的全局配置。
/// Application settings class, containing global application configuration.
/// </summary>
public class AppSettings
{
    /// <summary>
    /// 配置节名称。
    /// Configuration section name.
    /// </summary>
    public const string SectionName = "AppSettings";

    /// <summary>
    /// 数据库设置。
    /// Database settings.
    /// </summary>
    public DatabaseSettings Database { get; set; } = new();

    /// <summary>
    /// Gemini AI设置。
    /// Gemini AI settings.
    /// </summary>
    public GeminiSettings Gemini { get; set; } = new();
}
