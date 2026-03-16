using CatFoodManager.Infrastructure.Persistence;
using CatFoodManager.Infrastructure.Repositories;
using Xunit;

namespace CatFoodManager.Infrastructure.Tests.Integration;

public class BestPriceIntegrationTests : IAsyncLifetime
{
    private readonly TestDatabaseFixture _fixture;
    private DbContext _dbContext = null!;
    private SQLiteRepository<TestBestPrice> _repository = null!;

    public BestPriceIntegrationTests()
    {
        _fixture = new TestDatabaseFixture();
    }

    public Task InitializeAsync()
    {
        _dbContext = _fixture.CreateDbContext();
        _repository = new SQLiteRepository<TestBestPrice>(_dbContext);
        return Task.CompletedTask;
    }

    public async Task DisposeAsync()
    {
        _fixture.Cleanup(_dbContext);
    }

    [Fact]
    public async Task CreateAsync_ShouldAddBestPriceToDatabase()
    {
        var bestPrice = new TestBestPrice
        {
            Name = "Best Price Test",
            Type = 0,
            Platform = 1,
            LowestPrice = 99.99m,
            HasPurchased = false,
            FactoryName = "Factory A",
            HasTestReport = true,
            IsWorthRepurchasing = true,
            CreatedAt = DateTimeOffset.Now
        };

        await _repository.AddAsync(bestPrice);

        var result = await _repository.GetByIdAsync(bestPrice.Id);
        Assert.NotNull(result);
        Assert.Equal("Best Price Test", result.Name);
        Assert.Equal(0, result.Type);
        Assert.Equal(1, result.Platform);
        Assert.Equal(99.99m, result.LowestPrice);
        Assert.False(result.HasPurchased);
        Assert.True(result.HasTestReport);
        Assert.True(result.IsWorthRepurchasing);
    }

    [Fact]
    public async Task UpdateAsync_ShouldModifyBestPrice()
    {
        var bestPrice = new TestBestPrice
        {
            Name = "Update Test",
            Type = 2,
            Platform = 2,
            LowestPrice = 50.0m,
            HasPurchased = false,
            CreatedAt = DateTimeOffset.Now
        };

        await _repository.AddAsync(bestPrice);

        bestPrice.LowestPrice = 45.0m;
        bestPrice.FinalPrice = 40.0m;
        bestPrice.HasPurchased = true;
        bestPrice.PurchasedAt = DateTimeOffset.Now;
        bestPrice.UpdatedAt = DateTimeOffset.Now;
        await _repository.UpdateAsync(bestPrice);

        var result = await _repository.GetByIdAsync(bestPrice.Id);
        Assert.NotNull(result);
        Assert.Equal(45.0m, result.LowestPrice);
        Assert.Equal(40.0m, result.FinalPrice);
        Assert.True(result.HasPurchased);
        Assert.NotNull(result.PurchasedAt);
    }

    [Fact]
    public async Task FindByPlatformAsync_ShouldReturnFilteredResults()
    {
        var bestPrices = new List<TestBestPrice>
        {
            new() { Name = "JD Item", Type = 0, Platform = 1, LowestPrice = 100, CreatedAt = DateTimeOffset.Now },
            new() { Name = "Taobao Item", Type = 0, Platform = 2, LowestPrice = 90, CreatedAt = DateTimeOffset.Now },
            new() { Name = "PDD Item", Type = 0, Platform = 3, LowestPrice = 80, CreatedAt = DateTimeOffset.Now }
        };

        await _repository.AddRangeAsync(bestPrices);

        var jdItems = await _repository.FindAsync(b => b.Platform == 1);
        var taobaoItems = await _repository.FindAsync(b => b.Platform == 2);

        Assert.Single(jdItems);
        Assert.Single(taobaoItems);
        Assert.Equal("JD Item", jdItems[0].Name);
        Assert.Equal("Taobao Item", taobaoItems[0].Name);
    }

    [Fact]
    public async Task FindByTypeAsync_ShouldReturnFilteredResults()
    {
        var bestPrices = new List<TestBestPrice>
        {
            new() { Name = "Cat Food", Type = 0, Platform = 1, LowestPrice = 100, CreatedAt = DateTimeOffset.Now },
            new() { Name = "Snack", Type = 1, Platform = 1, LowestPrice = 50, CreatedAt = DateTimeOffset.Now },
            new() { Name = "Canned", Type = 2, Platform = 1, LowestPrice = 30, CreatedAt = DateTimeOffset.Now }
        };

        await _repository.AddRangeAsync(bestPrices);

        var catFoodItems = await _repository.FindAsync(b => b.Type == 0);
        var snackItems = await _repository.FindAsync(b => b.Type == 1);

        Assert.Single(catFoodItems);
        Assert.Single(snackItems);
        Assert.Equal("Cat Food", catFoodItems[0].Name);
        Assert.Equal("Snack", snackItems[0].Name);
    }

    [Fact]
    public async Task FindPurchasedItemsAsync_ShouldReturnCorrectResults()
    {
        var bestPrices = new List<TestBestPrice>
        {
            new() { Name = "Purchased 1", Type = 0, Platform = 1, LowestPrice = 100, HasPurchased = true, CreatedAt = DateTimeOffset.Now },
            new() { Name = "Not Purchased", Type = 0, Platform = 1, LowestPrice = 90, HasPurchased = false, CreatedAt = DateTimeOffset.Now },
            new() { Name = "Purchased 2", Type = 0, Platform = 2, LowestPrice = 80, HasPurchased = true, CreatedAt = DateTimeOffset.Now }
        };

        await _repository.AddRangeAsync(bestPrices);

        var purchasedItems = await _repository.FindAsync(b => b.HasPurchased);
        var notPurchasedItems = await _repository.FindAsync(b => !b.HasPurchased);

        Assert.Equal(2, purchasedItems.Count);
        Assert.Single(notPurchasedItems);
    }

    [Fact]
    public async Task FindWorthRepurchasingAsync_ShouldReturnCorrectResults()
    {
        var bestPrices = new List<TestBestPrice>
        {
            new() { Name = "Worth 1", Type = 0, Platform = 1, LowestPrice = 100, IsWorthRepurchasing = true, CreatedAt = DateTimeOffset.Now },
            new() { Name = "Not Worth", Type = 0, Platform = 1, LowestPrice = 90, IsWorthRepurchasing = false, CreatedAt = DateTimeOffset.Now },
            new() { Name = "Worth 2", Type = 0, Platform = 2, LowestPrice = 80, IsWorthRepurchasing = true, CreatedAt = DateTimeOffset.Now }
        };

        await _repository.AddRangeAsync(bestPrices);

        var worthItems = await _repository.FindAsync(b => b.IsWorthRepurchasing);

        Assert.Equal(2, worthItems.Count);
    }

    [Fact]
    public async Task UpdatePriceHistoryAsync_ShouldTrackChanges()
    {
        var bestPrice = new TestBestPrice
        {
            Name = "Price History Test",
            Type = 0,
            Platform = 1,
            LowestPrice = 100m,
            HasPurchased = false,
            CreatedAt = DateTimeOffset.Now
        };

        await _repository.AddAsync(bestPrice);

        bestPrice.LowestPrice = 90m;
        bestPrice.UpdatedAt = DateTimeOffset.Now;
        await _repository.UpdateAsync(bestPrice);

        var result1 = await _repository.GetByIdAsync(bestPrice.Id);
        Assert.NotNull(result1);
        Assert.Equal(90m, result1.LowestPrice);

        bestPrice.LowestPrice = 85m;
        bestPrice.FinalPrice = 80m;
        bestPrice.UpdatedAt = DateTimeOffset.Now;
        await _repository.UpdateAsync(bestPrice);

        var result2 = await _repository.GetByIdAsync(bestPrice.Id);
        Assert.NotNull(result2);
        Assert.Equal(85m, result2.LowestPrice);
        Assert.Equal(80m, result2.FinalPrice);
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveBestPrice()
    {
        var bestPrice = new TestBestPrice
        {
            Name = "Delete Test",
            Type = 0,
            Platform = 1,
            LowestPrice = 100,
            CreatedAt = DateTimeOffset.Now
        };

        await _repository.AddAsync(bestPrice);
        var id = bestPrice.Id;

        await _repository.DeleteAsync(id);

        var result = await _repository.GetByIdAsync(id);
        Assert.Null(result);
    }

    [Fact]
    public async Task FindByPriceRangeAsync_ShouldReturnCorrectResults()
    {
        var bestPrices = new List<TestBestPrice>
        {
            new() { Name = "Low Price", Type = 0, Platform = 1, LowestPrice = 50, CreatedAt = DateTimeOffset.Now },
            new() { Name = "Mid Price", Type = 0, Platform = 1, LowestPrice = 100, CreatedAt = DateTimeOffset.Now },
            new() { Name = "High Price", Type = 0, Platform = 1, LowestPrice = 200, CreatedAt = DateTimeOffset.Now }
        };

        await _repository.AddRangeAsync(bestPrices);

        var midRangeItems = await _repository.FindAsync(b => b.LowestPrice >= 75 && b.LowestPrice <= 150);

        Assert.Single(midRangeItems);
        Assert.Equal("Mid Price", midRangeItems[0].Name);
    }
}
