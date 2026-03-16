using CatFoodManager.Infrastructure.Persistence;
using CatFoodManager.Infrastructure.Repositories;
using Xunit;

namespace CatFoodManager.Infrastructure.Tests.Integration;

public class RepositoryIntegrationTests : IAsyncLifetime
{
    private readonly TestDatabaseFixture _fixture;
    private DbContext _dbContext = null!;
    private SQLiteRepository<TestCatFood> _catFoodRepository = null!;
    private SQLiteRepository<TestBrand> _brandRepository = null!;
    private SQLiteRepository<TestFactory> _factoryRepository = null!;

    public RepositoryIntegrationTests()
    {
        _fixture = new TestDatabaseFixture();
    }

    public Task InitializeAsync()
    {
        _dbContext = _fixture.CreateDbContext();
        _catFoodRepository = new SQLiteRepository<TestCatFood>(_dbContext);
        _brandRepository = new SQLiteRepository<TestBrand>(_dbContext);
        _factoryRepository = new SQLiteRepository<TestFactory>(_dbContext);
        return Task.CompletedTask;
    }

    public async Task DisposeAsync()
    {
        _fixture.Cleanup(_dbContext);
    }

    [Fact]
    public async Task GetByIdAsync_WhenEntityExists_ShouldReturnEntity()
    {
        var brand = new TestBrand { Name = "Test Brand" };
        await _brandRepository.AddAsync(brand);

        var result = await _brandRepository.GetByIdAsync(brand.Id);

        Assert.NotNull(result);
        Assert.Equal(brand.Id, result.Id);
        Assert.Equal("Test Brand", result.Name);
    }

    [Fact]
    public async Task GetByIdAsync_WhenEntityNotExists_ShouldReturnNull()
    {
        var result = await _brandRepository.GetByIdAsync(999999);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetAllAsync_WhenNoEntities_ShouldReturnEmptyList()
    {
        var result = await _factoryRepository.GetAllAsync();

        Assert.Empty(result);
    }

    [Fact]
    public async Task GetAllAsync_WhenEntitiesExist_ShouldReturnAllEntities()
    {
        var brands = new List<TestBrand>
        {
            new() { Name = "Brand 1" },
            new() { Name = "Brand 2" },
            new() { Name = "Brand 3" }
        };

        await _brandRepository.AddRangeAsync(brands);

        var result = await _brandRepository.GetAllAsync();

        Assert.Equal(3, result.Count);
    }

    [Fact]
    public async Task AddAsync_ShouldGenerateId()
    {
        var brand = new TestBrand { Name = "New Brand" };

        await _brandRepository.AddAsync(brand);

        Assert.True(brand.Id > 0);
    }

    [Fact]
    public async Task AddRangeAsync_ShouldAddMultipleEntities()
    {
        var factories = new List<TestFactory>
        {
            new() { Name = "Factory 1", CreatedAt = DateTimeOffset.Now },
            new() { Name = "Factory 2", CreatedAt = DateTimeOffset.Now }
        };

        await _factoryRepository.AddRangeAsync(factories);

        var result = await _factoryRepository.GetAllAsync();
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task UpdateAsync_ShouldModifyEntity()
    {
        var brand = new TestBrand { Name = "Original Name" };
        await _brandRepository.AddAsync(brand);

        brand.Name = "Updated Name";
        await _brandRepository.UpdateAsync(brand);

        var result = await _brandRepository.GetByIdAsync(brand.Id);
        Assert.NotNull(result);
        Assert.Equal("Updated Name", result.Name);
    }

    [Fact]
    public async Task DeleteAsync_WhenEntityExists_ShouldRemoveEntity()
    {
        var brand = new TestBrand { Name = "To Delete" };
        await _brandRepository.AddAsync(brand);
        var id = brand.Id;

        await _brandRepository.DeleteAsync(id);

        var result = await _brandRepository.GetByIdAsync(id);
        Assert.Null(result);
    }

    [Fact]
    public async Task DeleteAsync_WhenEntityNotExists_ShouldNotThrow()
    {
        var exception = await Record.ExceptionAsync(() => _brandRepository.DeleteAsync(999999));

        Assert.Null(exception);
    }

    [Fact]
    public async Task CountAsync_ShouldReturnCorrectCount()
    {
        await _brandRepository.AddAsync(new TestBrand { Name = "Count Test 1" });
        await _brandRepository.AddAsync(new TestBrand { Name = "Count Test 2" });

        var count = await _brandRepository.CountAsync();
        Assert.Equal(2, count);
    }

    [Fact]
    public async Task ExistsAsync_WhenEntityExists_ShouldReturnTrue()
    {
        var brand = new TestBrand { Name = "Exists Test" };
        await _brandRepository.AddAsync(brand);

        var result = await _brandRepository.ExistsAsync(brand.Id);

        Assert.True(result);
    }

    [Fact]
    public async Task ExistsAsync_WhenEntityNotExists_ShouldReturnFalse()
    {
        var result = await _brandRepository.ExistsAsync(999999);

        Assert.False(result);
    }

    [Fact]
    public async Task FindAsync_WithPredicate_ShouldReturnMatchingEntities()
    {
        var brands = new List<TestBrand>
        {
            new() { Name = "Apple" },
            new() { Name = "Banana" },
            new() { Name = "Apricot" }
        };

        await _brandRepository.AddRangeAsync(brands);

        var result = await _brandRepository.FindAsync(b => b.Name != null && b.Name.StartsWith("Ap"));

        Assert.Equal(2, result.Count);
        Assert.All(result, b => Assert.StartsWith("Ap", b.Name ?? string.Empty));
    }

    [Fact]
    public async Task FindAsync_WithNoMatches_ShouldReturnEmptyList()
    {
        var result = await _brandRepository.FindAsync(b => b.Name == "NonExistent");

        Assert.Empty(result);
    }

    [Fact]
    public async Task MultipleOperations_ShouldWorkCorrectly()
    {
        var brand = new TestBrand { Name = "Multi Op Brand" };
        await _brandRepository.AddAsync(brand);

        var catFood = new TestCatFood
        {
            Name = "Multi Op CatFood",
            FoodType = 0,
            Count = 5,
            Price = 50.0,
            Weights = 500,
            ProductionDate = DateTimeOffset.Now,
            BrandId = brand.Id,
            FactoryId = 1,
            CreatedAt = DateTimeOffset.Now
        };
        await _catFoodRepository.AddAsync(catFood);

        var retrievedCatFood = await _catFoodRepository.GetByIdAsync(catFood.Id);
        Assert.NotNull(retrievedCatFood);
        Assert.Equal(brand.Id, retrievedCatFood.BrandId);

        catFood.Count = 10;
        catFood.UpdatedAt = DateTimeOffset.Now;
        await _catFoodRepository.UpdateAsync(catFood);

        var updatedCatFood = await _catFoodRepository.GetByIdAsync(catFood.Id);
        Assert.NotNull(updatedCatFood);
        Assert.Equal(10, updatedCatFood.Count);

        await _catFoodRepository.DeleteAsync(catFood.Id);
        await _brandRepository.DeleteAsync(brand.Id);

        var deletedCatFood = await _catFoodRepository.GetByIdAsync(catFood.Id);
        var deletedBrand = await _brandRepository.GetByIdAsync(brand.Id);
        Assert.Null(deletedCatFood);
        Assert.Null(deletedBrand);
    }

    [Fact]
    public async Task ConcurrentOperations_ShouldHandleCorrectly()
    {
        var tasks = new List<Task>();

        for (int i = 0; i < 10; i++)
        {
            var index = i;
            tasks.Add(Task.Run(async () =>
            {
                var brand = new TestBrand { Name = $"Concurrent Brand {index}" };
                await _brandRepository.AddAsync(brand);
            }));
        }

        await Task.WhenAll(tasks);

        var result = await _brandRepository.GetAllAsync();
        Assert.Equal(10, result.Count);
    }

    [Fact]
    public async Task TransactionLikeBehavior_ShouldMaintainDataIntegrity()
    {
        var brand = new TestBrand { Name = "Transaction Test Brand" };
        await _brandRepository.AddAsync(brand);

        var catFoods = new List<TestCatFood>
        {
            new()
            {
                Name = "Transaction CatFood 1",
                FoodType = 0,
                Count = 1,
                Price = 10,
                Weights = 100,
                ProductionDate = DateTimeOffset.Now,
                BrandId = brand.Id,
                FactoryId = 1,
                CreatedAt = DateTimeOffset.Now
            },
            new()
            {
                Name = "Transaction CatFood 2",
                FoodType = 0,
                Count = 2,
                Price = 20,
                Weights = 200,
                ProductionDate = DateTimeOffset.Now,
                BrandId = brand.Id,
                FactoryId = 1,
                CreatedAt = DateTimeOffset.Now
            }
        };

        await _catFoodRepository.AddRangeAsync(catFoods);

        var relatedCatFoods = await _catFoodRepository.FindAsync(c => c.BrandId == brand.Id);

        Assert.Equal(2, relatedCatFoods.Count);
    }

    [Fact]
    public async Task LargeDataset_ShouldPerformEfficiently()
    {
        var brands = new List<TestBrand>();
        for (int i = 0; i < 100; i++)
        {
            brands.Add(new TestBrand { Name = $"Bulk Brand {i}" });
        }

        await _brandRepository.AddRangeAsync(brands);

        var count = await _brandRepository.CountAsync();
        Assert.Equal(100, count);

        var allBrands = await _brandRepository.GetAllAsync();
        Assert.Equal(100, allBrands.Count);
    }
}
