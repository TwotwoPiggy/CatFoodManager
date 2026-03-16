using CatFoodManager.Infrastructure.Configuration;
using Xunit;

namespace CatFoodManager.Tests.Configuration;

public class DatabaseSettingsTests
{
    [Fact]
    public void DatabaseSettings_ShouldHaveCorrectSectionName()
    {
        Assert.Equal("Database", DatabaseSettings.SectionName);
    }

    [Fact]
    public void DatabaseSettings_ShouldInitializeWithDefaultValues()
    {
        var settings = new DatabaseSettings();

        Assert.Equal(string.Empty, settings.ConnectionString);
        Assert.Equal("SQLite", settings.DatabaseType);
    }

    [Fact]
    public void DatabaseSettings_ShouldAllowSettingProperties()
    {
        var settings = new DatabaseSettings
        {
            ConnectionString = "Data Source=test.db",
            DatabaseType = "PostgreSQL"
        };

        Assert.Equal("Data Source=test.db", settings.ConnectionString);
        Assert.Equal("PostgreSQL", settings.DatabaseType);
    }
}
