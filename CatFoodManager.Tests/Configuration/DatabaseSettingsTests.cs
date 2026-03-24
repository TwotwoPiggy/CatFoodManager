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
        Assert.Equal(string.Empty, settings.DatabasePath);
        Assert.True(settings.EnableForeignKeys);
        Assert.Equal(30, settings.CommandTimeout);
    }

    [Fact]
    public void DatabaseSettings_ShouldAllowSettingProperties()
    {
        var settings = new DatabaseSettings
        {
            ConnectionString = "Data Source=test.db",
            DatabasePath = "/data/test.db",
            EnableForeignKeys = false,
            CommandTimeout = 60
        };

        Assert.Equal("Data Source=test.db", settings.ConnectionString);
        Assert.Equal("/data/test.db", settings.DatabasePath);
        Assert.False(settings.EnableForeignKeys);
        Assert.Equal(60, settings.CommandTimeout);
    }
}
