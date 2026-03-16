using CatFoodManager.Infrastructure.Persistence;
using CatFoodManager.Infrastructure.Repositories;
using Xunit;

namespace CatFoodManager.Infrastructure.Tests.Integration;

public class CatFoodIntegrationTests : IAsyncLifetime
{
    private readonly TestDatabaseFixture _fixture;
    private DbContext _dbContext = null!;
    private SQLiteRepository<TestCatFood> _repository = null!;

    public CatFoodIntegrationTests()
    {
        _fixture = new TestDatabaseFixture();
    }

    public Task InitializeAsync()
    {
        _dbContext = _fixture.CreateDbContext();
        _repository = new SQLiteRepository<TestCatFood>(_dbContext);
        return Task.CompletedTask;
    }

    public async Task DisposeAsync()
    {
        _fixture.Cleanup(_dbContext);
    }

    [Fact]
    public async Task CreateAsync_ShouldAddCatFoodToDatabase()
    {
        var catFood = new TestCatFood
        {
            Name = "Test Cat Food",
            OrderId = "ORD001",
            FoodType = 0,
            Count = 10,
            Price = 99.99,
            Weights = 500,
            ProductionDate = DateTimeOffset.Now,
            BrandId = 1,
            FactoryId = 1,
            CreatedAt = DateTimeOffset.Now
        };

        await _repository.AddAsync(catFood);

        var result = await _repository.GetByIdAsync(catFood.Id);
        Assert.NotNull(result);
        Assert.Equal("Test Cat Food", result.Name);
        Assert.Equal("ORD001", result.OrderId);
        Assert.Equal(0, result.FoodType);
        Assert.Equal(10, result.Count);
        Assert.Equal(99.99, result.Price);
    }

    [Fact]
    public async Task ReadAsync_ShouldReturnCatFoodFromDatabase()
    {
        var catFood = new TestCatFood
        {
            Name = "Read Test",
            FoodType = 2,
            Count = 5,
            Price = 50.0,
            Weights = 200,
            ProductionDate = DateTimeOffset.Now,
            BrandId = 1,
            FactoryId = 1,
            CreatedAt = DateTimeOffset.Now
        };

        await _repository.AddAsync(catFood);

        var result = await _repository.GetByIdAsync(catFood.Id);

        Assert.NotNull(result);
        Assert.Equal("Read Test", result.Name);
        Assert.Equal(2, result.FoodType);
    }

    [Fact]
    public async Task UpdateAsync_ShouldModifyCatFoodInDatabase()
    {
        var catFood = new TestCatFood
        {
            Name = "Update Test",
            FoodType = 3,
            Count = 3,
            Price = 30.0,
            Weights = 100,
            ProductionDate = DateTimeOffset.Now,
            BrandId = 1,
            FactoryId = 1,
            CreatedAt = DateTimeOffset.Now
        };

        await _repository.AddAsync(catFood);

        catFood.Name = "Updated Name";
        catFood.Price = 45.0;
        catFood.UpdatedAt = DateTimeOffset.Now;
        await _repository.UpdateAsync(catFood);

        var result = await _repository.GetByIdAsync(catFood.Id);
        Assert.NotNull(result);
        Assert.Equal("Updated Name", result.Name);
        Assert.Equal(45.0, result.Price);
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveCatFoodFromDatabase()
    {
        var catFood = new TestCatFood
        {
            Name = "Delete Test",
            FoodType = 1,
            Count = 1,
            Price = 15.0,
            Weights = 50,
            ProductionDate = DateTimeOffset.Now,
            BrandId = 1,
            FactoryId = 1,
            CreatedAt = DateTimeOffset.Now
        };

        await _repository.AddAsync(catFood);
        var id = catFood.Id;

        await _repository.DeleteAsync(id);

        var result = await _repository.GetByIdAsync(id);
        Assert.Null(result);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllCatFoods()
    {
        var catFoods = new List<TestCatFood>
        {
            new() { Name = "Item1", FoodType = 0, Count = 1, Price = 10, Weights = 100, ProductionDate = DateTimeOffset.Now, BrandId = 1, FactoryId = 1, CreatedAt = DateTimeOffset.Now },
            new() { Name = "Item2", FoodType = 0, Count = 2, Price = 20, Weights = 200, ProductionDate = DateTimeOffset.Now, BrandId = 1, FactoryId = 1, CreatedAt = DateTimeOffset.Now },
            new() { Name = "Item3", FoodType = 0, Count = 3, Price = 30, Weights = 300, ProductionDate = DateTimeOffset.Now, BrandId = 1, FactoryId = 1, CreatedAt = DateTimeOffset.Now }
        };

        await _repository.AddRangeAsync(catFoods);

        var result = await _repository.GetAllAsync();

        Assert.Equal(3, result.Count);
    }

    [Fact]
    public async Task FindAsync_ShouldReturnFilteredResults()
    {
        var catFood = new TestCatFood
        {
            Name = "Find Test Unique",
            FoodType = 2,
            Count = 5,
            Price = 50.0,
            Weights = 200,
            ProductionDate = DateTimeOffset.Now,
            BrandId = 1,
            FactoryId = 1,
            CreatedAt = DateTimeOffset.Now
        };

        await _repository.AddAsync(catFood);

        var result = await _repository.FindAsync(c => c.Name == "Find Test Unique");

        Assert.Single(result);
        Assert.Equal("Find Test Unique", result[0].Name);
    }

    [Fact]
    public async Task CountAsync_ShouldReturnCorrectCount()
    {
        var catFoods = new List<TestCatFood>
        {
            new() { Name = "Count1", FoodType = 0, Count = 1, Price = 10, Weights = 100, ProductionDate = DateTimeOffset.Now, BrandId = 1, FactoryId = 1, CreatedAt = DateTimeOffset.Now },
            new() { Name = "Count2", FoodType = 0, Count = 2, Price = 20, Weights = 200, ProductionDate = DateTimeOffset.Now, BrandId = 1, FactoryId = 1, CreatedAt = DateTimeOffset.Now }
        };

        await _repository.AddRangeAsync(catFoods);

        var count = await _repository.CountAsync();
        Assert.Equal(2, count);
    }

    [Fact]
    public async Task ExistsAsync_ShouldReturnCorrectResult()
    {
        var catFood = new TestCatFood
        {
            Name = "Exists Test",
            FoodType = 0,
            Count = 1,
            Price = 10,
            Weights = 100,
            ProductionDate = DateTimeOffset.Now,
            BrandId = 1,
            FactoryId = 1,
            CreatedAt = DateTimeOffset.Now
        };

        await _repository.AddAsync(catFood);

        var exists = await _repository.ExistsAsync(catFood.Id);
        var notExists = await _repository.ExistsAsync(999999);

        Assert.True(exists);
        Assert.False(notExists);
    }

    [Fact]
    public async Task Pagination_ShouldWorkWithLargeDataset()
    {
        for (int i = 0; i < 25; i++)
        {
            var catFood = new TestCatFood
            {
                Name = $"Page Item {i}",
                FoodType = 0,
                Count = i + 1,
                Price = (i + 1) * 10,
                Weights = (i + 1) * 100,
                ProductionDate = DateTimeOffset.Now,
                BrandId = 1,
                FactoryId = 1,
                CreatedAt = DateTimeOffset.Now
            };
            await _repository.AddAsync(catFood);
        }

        var allItems = await _repository.GetAllAsync();
        var page1 = allItems.Take(10).ToList();
        var page2 = allItems.Skip(10).Take(10).ToList();

        Assert.Equal(10, page1.Count);
        Assert.Equal(10, page2.Count);
    }

    [Fact]
    public async Task SearchAsync_ShouldFindByName()
    {
        var catFood = new TestCatFood
        {
            Name = "Special Search Keyword Product",
            FoodType = 0,
            Count = 1,
            Price = 10,
            Weights = 100,
            ProductionDate = DateTimeOffset.Now,
            BrandId = 1,
            FactoryId = 1,
            CreatedAt = DateTimeOffset.Now
        };

        await _repository.AddAsync(catFood);

        var result = await _repository.FindAsync(c => c.Name != null && c.Name.Contains("Search Keyword"));

        Assert.Single(result);
        Assert.Contains("Search Keyword", result[0].Name);
    }
}
