using CatFoodManager.Domain.Entities;
using Xunit;

namespace CatFoodManager.Tests.Domain.Entities;

public class FactoryTests
{
    [Fact]
    public void Constructor_ShouldInitializeWithDefaultValues()
    {
        var factory = new Factory();

        Assert.Equal(0, factory.Id);
        Assert.Null(factory.Name);
        Assert.Equal(default, factory.CreatedAt);
        Assert.Null(factory.UpdatedAt);
        Assert.Null(factory.PurchasedAt);
    }

    [Fact]
    public void Properties_ShouldBeSettable()
    {
        var now = DateTimeOffset.Now;
        var factory = new Factory
        {
            Id = 1,
            Name = "Test Factory",
            CreatedAt = now,
            UpdatedAt = now.AddDays(1),
            PurchasedAt = now.AddDays(2)
        };

        Assert.Equal(1, factory.Id);
        Assert.Equal("Test Factory", factory.Name);
        Assert.Equal(now, factory.CreatedAt);
        Assert.Equal(now.AddDays(1), factory.UpdatedAt);
        Assert.Equal(now.AddDays(2), factory.PurchasedAt);
    }
}
