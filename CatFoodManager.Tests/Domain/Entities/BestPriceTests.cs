using CatFoodManager.Domain.Entities;
using CatFoodManager.Domain.Enums;
using Xunit;

namespace CatFoodManager.Tests.Domain.Entities;

public class BestPriceTests
{
    [Fact]
    public void Constructor_ShouldInitializeWithDefaultValues()
    {
        var bestPrice = new BestPrice();

        Assert.Equal(0, bestPrice.Id);
        Assert.Null(bestPrice.Name);
        Assert.Equal(default, bestPrice.CreatedAt);
        Assert.Null(bestPrice.UpdatedAt);
        Assert.Null(bestPrice.PurchasedAt);
        Assert.Equal(ProductType.CatFood, bestPrice.Type);
        Assert.Equal(PlatformType.None, bestPrice.Platform);
        Assert.Equal(0, bestPrice.LowestPrice);
        Assert.False(bestPrice.HasPurchased);
        Assert.Null(bestPrice.FinalPrice);
        Assert.Null(bestPrice.PicturePath);
        Assert.Null(bestPrice.FactoryName);
        Assert.False(bestPrice.HasTestReport);
        Assert.False(bestPrice.IsWorthRepurchasing);
    }

    [Fact]
    public void Properties_ShouldBeSettable()
    {
        var now = DateTimeOffset.Now;
        var bestPrice = new BestPrice
        {
            Id = 1,
            Name = "Test Best Price",
            CreatedAt = now,
            UpdatedAt = now.AddDays(1),
            PurchasedAt = now.AddDays(2),
            Type = ProductType.CannedFood,
            Platform = PlatformType.JD,
            LowestPrice = 99.99m,
            HasPurchased = true,
            FinalPrice = 89.99m,
            PicturePath = "/path/to/picture.jpg",
            FactoryName = "Test Factory",
            HasTestReport = true,
            IsWorthRepurchasing = true
        };

        Assert.Equal(1, bestPrice.Id);
        Assert.Equal("Test Best Price", bestPrice.Name);
        Assert.Equal(now, bestPrice.CreatedAt);
        Assert.Equal(now.AddDays(1), bestPrice.UpdatedAt);
        Assert.Equal(now.AddDays(2), bestPrice.PurchasedAt);
        Assert.Equal(ProductType.CannedFood, bestPrice.Type);
        Assert.Equal(PlatformType.JD, bestPrice.Platform);
        Assert.Equal(99.99m, bestPrice.LowestPrice);
        Assert.True(bestPrice.HasPurchased);
        Assert.Equal(89.99m, bestPrice.FinalPrice);
        Assert.Equal("/path/to/picture.jpg", bestPrice.PicturePath);
        Assert.Equal("Test Factory", bestPrice.FactoryName);
        Assert.True(bestPrice.HasTestReport);
        Assert.True(bestPrice.IsWorthRepurchasing);
    }
}
