using CatFoodManager.Domain.Entities;
using Xunit;

namespace CatFoodManager.Tests.Domain.Entities;

public class PlatformRegExpTests
{
    [Fact]
    public void Constructor_ShouldInitializeWithDefaultValues()
    {
        var platformRegExp = new PlatformRegExp();

        Assert.Equal(0, platformRegExp.Id);
        Assert.Equal(string.Empty, platformRegExp.Name);
        Assert.Equal(string.Empty, platformRegExp.Platform);
        Assert.Equal(string.Empty, platformRegExp.RegularExpression);
        Assert.Equal(string.Empty, platformRegExp.FieldInfos);
    }

    [Fact]
    public void Properties_ShouldBeSettable()
    {
        var platformRegExp = new PlatformRegExp
        {
            Id = 1,
            Name = "Taobao",
            Platform = "Taobao",
            RegularExpression = @"\d+",
            FieldInfos = "{\"price\":1,\"name\":2}"
        };

        Assert.Equal(1, platformRegExp.Id);
        Assert.Equal("Taobao", platformRegExp.Name);
        Assert.Equal("Taobao", platformRegExp.Platform);
        Assert.Equal(@"\d+", platformRegExp.RegularExpression);
        Assert.Equal("{\"price\":1,\"name\":2}", platformRegExp.FieldInfos);
    }
}
