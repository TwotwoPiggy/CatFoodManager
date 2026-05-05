using CatFoodManager.Infrastructure.Configuration;
using Xunit;

namespace CatFoodManager.Tests.Configuration;

public class GeminiSettingsTests
{
    [Fact]
    public void GeminiSettings_ShouldHaveCorrectSectionName()
    {
        Assert.Equal("Gemini", GeminiSettings.SectionName);
    }

    [Fact]
    public void GeminiSettings_ShouldInitializeWithDefaultValues()
    {
        var settings = new GeminiSettings();

        Assert.Equal(string.Empty, settings.ApiKey);
        Assert.Equal("gemini-2.0-flash", settings.Model);
        Assert.Equal(3, settings.MaxRetries);
        Assert.Equal(120, settings.TimeoutSeconds);
        Assert.Equal("https://generativelanguage.googleapis.com/v1beta", settings.BaseUrl);
        Assert.True(settings.EnableCache);
        Assert.Equal(60, settings.CacheExpirationMinutes);
    }

    [Fact]
    public void GeminiSettings_ShouldAllowSettingProperties()
    {
        var settings = new GeminiSettings
        {
            ApiKey = "test-api-key",
            Model = "gemini-1.5-pro",
            MaxRetries = 5,
            TimeoutSeconds = 60
        };

        Assert.Equal("test-api-key", settings.ApiKey);
        Assert.Equal("gemini-1.5-pro", settings.Model);
        Assert.Equal(5, settings.MaxRetries);
        Assert.Equal(60, settings.TimeoutSeconds);
    }
}
