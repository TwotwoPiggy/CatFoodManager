using CatFoodManager.Domain.Entities;
using Xunit;

namespace CatFoodManager.Tests.Core.Models;

public class CoreBaseEntityTests
{
    [Fact]
    public void CreatedAtString_SetWithTicksFormat_ShouldParseCorrectly()
    {
        var expectedDate = new DateTime(2024, 1, 15, 10, 30, 45, DateTimeKind.Utc);
        var ticks = expectedDate.Ticks;

        var entity = new TestCoreEntity
        {
            CreatedAtString = ticks.ToString()
        };

        Assert.Equal(expectedDate, entity.CreatedAt.UtcDateTime);
    }

    [Fact]
    public void CreatedAtString_SetWithDateTimeStringFormat_ShouldParseCorrectly()
    {
        var expectedDate = new DateTime(2024, 1, 15, 10, 30, 45);
        var dateString = expectedDate.ToString("yyyy-MM-dd HH:mm:ss");

        var entity = new TestCoreEntity
        {
            CreatedAtString = dateString
        };

        Assert.Equal(expectedDate, entity.CreatedAt.DateTime);
    }

    [Fact]
    public void CreatedAtString_Get_ShouldReturnFormattedString()
    {
        var date = new DateTime(2024, 1, 15, 10, 30, 45);
        var entity = new TestCoreEntity
        {
            CreatedAt = new DateTimeOffset(date, TimeSpan.Zero)
        };

        var result = entity.CreatedAtString;

        Assert.Equal("2024-01-15 10:30:45", result);
    }

    [Fact]
    public void UpdatedAtString_SetWithTicksFormat_ShouldParseCorrectly()
    {
        var expectedDate = new DateTime(2024, 1, 15, 10, 30, 45, DateTimeKind.Utc);
        var ticks = expectedDate.Ticks;

        var entity = new TestCoreEntity
        {
            UpdatedAtString = ticks.ToString()
        };

        Assert.Equal(expectedDate, entity.UpdatedAt!.Value.UtcDateTime);
    }

    [Fact]
    public void UpdatedAtString_SetWithNull_ShouldSetNull()
    {
        var entity = new TestCoreEntity
        {
            UpdatedAtString = null
        };

        Assert.Null(entity.UpdatedAt);
    }

    [Fact]
    public void UpdatedAtString_SetWithEmptyString_ShouldSetNull()
    {
        var entity = new TestCoreEntity
        {
            UpdatedAtString = string.Empty
        };

        Assert.Null(entity.UpdatedAt);
    }

    [Fact]
    public void PurchasedAtString_SetWithTicksFormat_ShouldParseCorrectly()
    {
        var expectedDate = new DateTime(2024, 1, 15, 10, 30, 45, DateTimeKind.Utc);
        var ticks = expectedDate.Ticks;

        var entity = new TestCoreEntity
        {
            PurchasedAtString = ticks.ToString()
        };

        Assert.Equal(expectedDate, entity.PurchasedAt!.Value.UtcDateTime);
    }

    [Fact]
    public void UpdatedAtString_Get_WhenNull_ShouldReturnNull()
    {
        var entity = new TestCoreEntity
        {
            UpdatedAt = null
        };

        Assert.Null(entity.UpdatedAtString);
    }

    [Fact]
    public void PurchasedAtString_Get_WhenNull_ShouldReturnNull()
    {
        var entity = new TestCoreEntity
        {
            PurchasedAt = null
        };

        Assert.Null(entity.PurchasedAtString);
    }

    [Fact]
    public void CreatedAtString_WithActualTicksFromError_ShouldParseCorrectly()
    {
        var ticks = 639098700594077081L;

        var entity = new TestCoreEntity
        {
            CreatedAtString = ticks.ToString()
        };

        var expectedDate = new DateTimeOffset(ticks, TimeSpan.Zero);
        Assert.Equal(expectedDate, entity.CreatedAt);
    }

    private class TestCoreEntity : BaseEntity { }
}
