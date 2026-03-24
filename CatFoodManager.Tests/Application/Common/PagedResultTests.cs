using CatFoodManager.Application.Common;
using Xunit;

namespace CatFoodManager.Tests.Application.Common;

public class PagedResultTests
{
    [Fact]
    public void Constructor_ShouldInitializeWithDefaultValues()
    {
        var result = new PagedResult<string>();

        Assert.NotNull(result.Items);
        Assert.Empty(result.Items);
        Assert.Equal(0, result.TotalCount);
        Assert.Equal(0, result.Page);
        Assert.Equal(0, result.PageSize);
    }

    [Fact]
    public void TotalPages_ShouldCalculateCorrectly_WhenTotalCountIsDivisibleByPageSize()
    {
        var result = new PagedResult<string>
        {
            TotalCount = 100,
            PageSize = 10
        };

        Assert.Equal(10, result.TotalPages);
    }

    [Fact]
    public void TotalPages_ShouldRoundUp_WhenTotalCountIsNotDivisibleByPageSize()
    {
        var result = new PagedResult<string>
        {
            TotalCount = 95,
            PageSize = 10
        };

        Assert.Equal(10, result.TotalPages);
    }

    [Fact]
    public void TotalPages_ShouldReturnOne_WhenTotalCountIsLessThanPageSize()
    {
        var result = new PagedResult<string>
        {
            TotalCount = 5,
            PageSize = 10
        };

        Assert.Equal(1, result.TotalPages);
    }

    [Fact]
    public void TotalPages_ShouldReturnZero_WhenPageSizeIsZero()
    {
        var result = new PagedResult<string>
        {
            TotalCount = 100,
            PageSize = 0
        };

        Assert.Equal(0, result.TotalPages);
    }

    [Fact]
    public void Items_ShouldBeSettable()
    {
        var items = new List<string> { "item1", "item2", "item3" };
        var result = new PagedResult<string>
        {
            Items = items
        };

        Assert.Equal(3, result.Items.Count);
        Assert.Equal("item1", result.Items[0]);
        Assert.Equal("item2", result.Items[1]);
        Assert.Equal("item3", result.Items[2]);
    }
}
