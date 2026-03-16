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

        Assert.Equal(string.Empty, settings.PictureFolders);
        Assert.Equal(string.Empty, settings.TessdataPath);
    }

    [Fact]
    public void AppSettings_ShouldAllowSettingProperties()
    {
        var settings = new AppSettings
        {
            PictureFolders = "C:\\Pictures",
            TessdataPath = "C:\\Tessdata"
        };

        Assert.Equal("C:\\Pictures", settings.PictureFolders);
        Assert.Equal("C:\\Tessdata", settings.TessdataPath);
    }
}
