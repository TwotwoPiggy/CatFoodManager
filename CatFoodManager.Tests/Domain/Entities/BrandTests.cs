using CatFoodManager.Domain.Entities;
using Xunit;

namespace CatFoodManager.Tests.Domain.Entities;

public class BrandTests
{
    [Fact]
    public void Constructor_ShouldInitializeWithDefaultValues()
    {
        var brand = new Brand();

        Assert.Equal(0, brand.Id);
        Assert.Null(brand.Name);
    }

    [Fact]
    public void Properties_ShouldBeSettable()
    {
        var brand = new Brand
        {
            Id = 1,
            Name = "Test Brand"
        };

        Assert.Equal(1, brand.Id);
        Assert.Equal("Test Brand", brand.Name);
    }
}
