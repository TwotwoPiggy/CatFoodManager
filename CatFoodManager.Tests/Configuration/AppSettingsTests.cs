using CatFoodManager.Infrastructure.Configuration;
using Xunit;

namespace CatFoodManager.Tests.Configuration;

public class AppSettingsTests
{
    [Fact]
    public void AppSettings_ShouldHaveCorrectSectionName()
    {
        Assert.Equal("AppSettings", AppSettings.SectionName);
    }

    [Fact]
    public void AppSettings_ShouldInitializeWithDefaultValues()
    {
        var settings = new AppSettings();

        Assert.NotNull(settings.Database);
        Assert.NotNull(settings.Gemini);
    }

    [Fact]
    public void AppSettings_ShouldAllowSettingProperties()
    {
        var settings = new AppSettings
        {
            Database = new DatabaseSettings
            {
                ConnectionString = "Data Source=test.db"
            },
            Gemini = new GeminiSettings
            {
                ApiKey = "test-key"
            }
        };

        Assert.Equal("Data Source=test.db", settings.Database.ConnectionString);
        Assert.Equal("test-key", settings.Gemini.ApiKey);
    }
}
