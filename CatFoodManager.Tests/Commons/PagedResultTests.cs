using CatFoodManager.Commons;
using Xunit;

namespace CatFoodManager.Tests.Commons;

public class PagedResultTests
{
    [Fact]
    public void Constructor_ShouldInitializeWithDefaultValues()
    {
        var result = new PagedResult<int>();

        Assert.Empty(result.Items);
        Assert.Equal(0, result.TotalCount);
        Assert.Equal(0, result.PageNumber);
        Assert.Equal(0, result.PageSize);
    }

    [Fact]
    public void Constructor_WithParameters_ShouldSetProperties()
    {
        var items = new List<int> { 1, 2, 3 };
        var result = new PagedResult<int>(items, 100, 2, 10);

        Assert.Equal(items, result.Items);
        Assert.Equal(100, result.TotalCount);
        Assert.Equal(2, result.PageNumber);
        Assert.Equal(10, result.PageSize);
    }

    [Fact]
    public void TotalPages_WhenPageSizeIsZero_ShouldReturnOne()
    {
        var result = new PagedResult<int>
        {
            TotalCount = 100,
            PageSize = 0
        };

        Assert.Equal(1, result.TotalPages);
    }

    [Fact]
    public void TotalPages_ShouldCalculateCorrectly()
    {
        var result = new PagedResult<int>
        {
            TotalCount = 100,
            PageSize = 10
        };

        Assert.Equal(10, result.TotalPages);
    }

    [Fact]
    public void TotalPages_WithRemainder_ShouldRoundUp()
    {
        var result = new PagedResult<int>
        {
            TotalCount = 95,
            PageSize = 10
        };

        Assert.Equal(10, result.TotalPages);
    }

    [Fact]
    public void HasPreviousPage_WhenPageNumberIsOne_ShouldReturnFalse()
    {
        var result = new PagedResult<int> { PageNumber = 1 };

        Assert.False(result.HasPreviousPage);
    }

    [Fact]
    public void HasPreviousPage_WhenPageNumberGreaterThanOne_ShouldReturnTrue()
    {
        var result = new PagedResult<int> { PageNumber = 2 };

        Assert.True(result.HasPreviousPage);
    }

    [Fact]
    public void HasNextPage_WhenAtLastPage_ShouldReturnFalse()
    {
        var result = new PagedResult<int>
        {
            PageNumber = 10,
            TotalCount = 100,
            PageSize = 10
        };

        Assert.False(result.HasNextPage);
    }

    [Fact]
    public void HasNextPage_WhenNotAtLastPage_ShouldReturnTrue()
    {
        var result = new PagedResult<int>
        {
            PageNumber = 1,
            TotalCount = 100,
            PageSize = 10
        };

        Assert.True(result.HasNextPage);
    }

    [Fact]
    public void Create_ShouldReturnCorrectPagedResult()
    {
        var source = Enumerable.Range(1, 100);

        var result = PagedResult<int>.Create(source, 2, 10);

        Assert.Equal(10, result.Items.Count());
        Assert.Equal(100, result.TotalCount);
        Assert.Equal(2, result.PageNumber);
        Assert.Equal(10, result.PageSize);
        Assert.Equal(11, result.Items.First());
        Assert.Equal(20, result.Items.Last());
    }

    [Fact]
    public void Create_WithFirstPage_ShouldReturnFirstItems()
    {
        var source = Enumerable.Range(1, 100);

        var result = PagedResult<int>.Create(source, 1, 10);

        Assert.Equal(1, result.Items.First());
        Assert.Equal(10, result.Items.Last());
    }

    [Fact]
    public void Create_WithEmptySource_ShouldReturnEmptyResult()
    {
        var source = new List<int>();

        var result = PagedResult<int>.Create(source, 1, 10);

        Assert.Empty(result.Items);
        Assert.Equal(0, result.TotalCount);
    }
}
