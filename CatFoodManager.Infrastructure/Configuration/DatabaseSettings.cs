namespace CatFoodManager.Infrastructure.Configuration;

/// <summary>
/// 数据库设置类，包含数据库连接相关配置。
/// Database settings class, containing database connection related configuration.
/// </summary>
public class DatabaseSettings
{
    /// <summary>
    /// 配置节名称。
    /// Configuration section name.
    /// </summary>
    public const string SectionName = "Database";

    /// <summary>
    /// 数据库连接字符串。
    /// Database connection string.
    /// </summary>
    public string ConnectionString { get; set; } = string.Empty;

    /// <summary>
    /// 数据库文件路径。
    /// Database file path.
    /// </summary>
    public string DatabasePath { get; set; } = string.Empty;

    /// <summary>
    /// 是否启用外键约束。
    /// Whether to enable foreign key constraints.
    /// </summary>
    public bool EnableForeignKeys { get; set; } = true;

    /// <summary>
    /// 命令超时时间（秒）。
    /// Command timeout in seconds.
    /// </summary>
    public int CommandTimeout { get; set; } = 30;
}
