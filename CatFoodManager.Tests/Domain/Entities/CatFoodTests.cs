using CatFoodManager.Domain.Entities;
using CatFoodManager.Domain.Enums;
using Xunit;

namespace CatFoodManager.Tests.Domain.Entities;

public class CatFoodTests
{
    [Fact]
    public void Constructor_ShouldInitializeWithDefaultValues()
    {
        var catFood = new CatFood();

        Assert.Equal(0, catFood.Id);
        Assert.Null(catFood.Name);
        Assert.Equal(default, catFood.CreatedAt);
        Assert.Null(catFood.UpdatedAt);
        Assert.Null(catFood.PurchasedAt);
        Assert.Null(catFood.OrderId);
        Assert.Equal(ProductType.CatFood, catFood.FoodType);
        Assert.Equal(0, catFood.Count);
        Assert.Equal(0, catFood.Price);
        Assert.Equal(0, catFood.Weights);
        Assert.Null(catFood.PicturePath);
        Assert.Equal(default, catFood.ProductionDate);
        Assert.Equal(0, catFood.FeededCount);
        Assert.Equal(0, catFood.BrandId);
        Assert.Equal(0, catFood.FactoryId);
        Assert.True(catFood.Feeded);
    }

    [Fact]
    public void Properties_ShouldBeSettable()
    {
        var now = DateTimeOffset.Now;
        var catFood = new CatFood
        {
            Id = 1,
            Name = "Test Cat Food",
            CreatedAt = now,
            UpdatedAt = now.AddDays(1),
            PurchasedAt = now.AddDays(2),
            OrderId = "ORDER123",
            FoodType = ProductType.CannedFood,
            Count = 10,
            Price = 99.99,
            Weights = 500,
            PicturePath = "/path/to/picture.jpg",
            ProductionDate = now,
            FeededCount = 5,
            BrandId = 1,
            FactoryId = 2
        };

        Assert.Equal(1, catFood.Id);
        Assert.Equal("Test Cat Food", catFood.Name);
        Assert.Equal(now, catFood.CreatedAt);
        Assert.Equal(now.AddDays(1), catFood.UpdatedAt);
        Assert.Equal(now.AddDays(2), catFood.PurchasedAt);
        Assert.Equal("ORDER123", catFood.OrderId);
        Assert.Equal(ProductType.CannedFood, catFood.FoodType);
        Assert.Equal(10, catFood.Count);
        Assert.Equal(99.99, catFood.Price);
        Assert.Equal(500, catFood.Weights);
        Assert.Equal("/path/to/picture.jpg", catFood.PicturePath);
        Assert.Equal(now, catFood.ProductionDate);
        Assert.Equal(5, catFood.FeededCount);
        Assert.Equal(1, catFood.BrandId);
        Assert.Equal(2, catFood.FactoryId);
    }

    [Fact]
    public void Feeded_WhenCountEqualsFeededCount_ShouldReturnTrue()
    {
        var catFood = new CatFood
        {
            Count = 10,
            FeededCount = 10
        };

        Assert.True(catFood.Feeded);
    }

    [Fact]
    public void Feeded_WhenCountNotEqualsFeededCount_ShouldReturnFalse()
    {
        var catFood = new CatFood
        {
            Count = 10,
            FeededCount = 5
        };

        Assert.False(catFood.Feeded);
    }

    [Fact]
    public void Feeded_WhenSetToTrue_ShouldSetFeededCountToCount()
    {
        var catFood = new CatFood
        {
            Count = 10,
            FeededCount = 5
        };

        catFood.Feeded = true;

        Assert.Equal(10, catFood.FeededCount);
    }

    [Fact]
    public void Feeded_WhenSetToFalse_ShouldNotChangeFeededCount()
    {
        var catFood = new CatFood
        {
            Count = 10,
            FeededCount = 5
        };

        catFood.Feeded = false;

        Assert.Equal(5, catFood.FeededCount);
    }
}
