using CatFoodManager.Domain.Entities;
using CatFoodManager.Infrastructure.Caching;
using Xunit;

namespace CatFoodManager.Infrastructure.Tests.Caching;

public class CacheKeyManagerTests
{
    [Fact]
    public void ForEntity_ShouldGenerateCorrectKey()
    {
        var id = 123L;

        var key = CacheKeyManager.ForEntity<CatFood>(id);

        Assert.Equal("entity:catfood:123", key);
    }

    [Fact]
    public void ForEntity_WithDifferentType_ShouldGenerateDifferentKey()
    {
        var id = 123L;

        var key1 = CacheKeyManager.ForEntity<CatFood>(id);
        var key2 = CacheKeyManager.ForEntity<Brand>(id);

        Assert.NotEqual(key1, key2);
        Assert.Equal("entity:catfood:123", key1);
        Assert.Equal("entity:brand:123", key2);
    }

    [Fact]
    public void ForList_WithoutSuffix_ShouldGenerateBaseKey()
    {
        var key = CacheKeyManager.ForList<CatFood>();

        Assert.Equal("list:catfood", key);
    }

    [Fact]
    public void ForList_WithSuffix_ShouldGenerateKeyWithSuffix()
    {
        var key = CacheKeyManager.ForList<CatFood>("active");

        Assert.Equal("list:catfood:active", key);
    }

    [Fact]
    public void ForList_WithEmptySuffix_ShouldGenerateBaseKey()
    {
        var key = CacheKeyManager.ForList<CatFood>("");

        Assert.Equal("list:catfood", key);
    }

    [Fact]
    public void ForList_WithNullSuffix_ShouldGenerateBaseKey()
    {
        var key = CacheKeyManager.ForList<CatFood>(null);

        Assert.Equal("list:catfood", key);
    }

    [Fact]
    public void ForSearch_ShouldGenerateCorrectKey()
    {
        var keyword = "Royal Canin";

        var key = CacheKeyManager.ForSearch<CatFood>(keyword);

        Assert.Equal("search:catfood:royal canin", key);
    }

    [Fact]
    public void ForSearch_WithMixedCase_ShouldConvertToLower()
    {
        var keyword = "ROYAL canin";

        var key = CacheKeyManager.ForSearch<CatFood>(keyword);

        Assert.Equal("search:catfood:royal canin", key);
    }

    [Fact]
    public void ForPaged_ShouldGenerateCorrectKey()
    {
        var page = 2;
        var pageSize = 20;

        var key = CacheKeyManager.ForPaged<CatFood>(page, pageSize);

        Assert.Equal("paged:catfood:2:20", key);
    }

    [Fact]
    public void ForPaged_WithDifferentParameters_ShouldGenerateDifferentKeys()
    {
        var key1 = CacheKeyManager.ForPaged<CatFood>(1, 10);
        var key2 = CacheKeyManager.ForPaged<CatFood>(2, 10);
        var key3 = CacheKeyManager.ForPaged<CatFood>(1, 20);

        Assert.NotEqual(key1, key2);
        Assert.NotEqual(key1, key3);
        Assert.NotEqual(key2, key3);
    }

    [Fact]
    public void ForEntity_WithDifferentIds_ShouldGenerateDifferentKeys()
    {
        var key1 = CacheKeyManager.ForEntity<CatFood>(1);
        var key2 = CacheKeyManager.ForEntity<CatFood>(2);

        Assert.NotEqual(key1, key2);
    }

    [Fact]
    public void ForList_WithDifferentTypes_ShouldGenerateDifferentKeys()
    {
        var key1 = CacheKeyManager.ForList<CatFood>();
        var key2 = CacheKeyManager.ForList<Brand>();
        var key3 = CacheKeyManager.ForList<Factory>();

        Assert.NotEqual(key1, key2);
        Assert.NotEqual(key1, key3);
        Assert.NotEqual(key2, key3);
    }
}
