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

        Assert.Empty(settings.PlatformFolders);
        Assert.Equal(string.Empty, settings.TessdataPath);
    }

    [Fact]
    public void AppSettings_ShouldAllowSettingProperties()
    {
        var settings = new AppSettings
        {
            PlatformFolders = new Dictionary<string, string>
            {
                { "京东", "C:\\JDPictures" },
                { "淘宝", "C:\\TaobaoPictures" }
            },
            TessdataPath = "C:\\Tessdata"
        };

        Assert.Equal("C:\\JDPictures", settings.PlatformFolders["京东"]);
        Assert.Equal("C:\\TaobaoPictures", settings.PlatformFolders["淘宝"]);
        Assert.Equal("C:\\Tessdata", settings.TessdataPath);
    }
}
