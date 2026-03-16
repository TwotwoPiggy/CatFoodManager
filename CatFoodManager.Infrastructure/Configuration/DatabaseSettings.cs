namespace CatFoodManager.Infrastructure.Configuration;

public class DatabaseSettings
{
    public const string SectionName = "Database";
    public string ConnectionString { get; set; } = string.Empty;
    public string DatabaseType { get; set; } = "SQLite";
}
