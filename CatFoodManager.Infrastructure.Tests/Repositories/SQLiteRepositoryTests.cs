using CatFoodManager.Domain.Interfaces;
using CatFoodManager.Infrastructure.Persistence;
using CatFoodManager.Infrastructure.Repositories;
using SQLite;
using Xunit;

namespace CatFoodManager.Infrastructure.Tests.Repositories;

public class SQLiteRepositoryTests : IDisposable
{
    private readonly string _testDbPath;
    private readonly DbContext _dbContext;
    private readonly SQLiteRepository<TestEntity> _repository;

    public SQLiteRepositoryTests()
    {
        _testDbPath = Path.Combine(Path.GetTempPath(), $"test_{Guid.NewGuid()}.db");
        _dbContext = new DbContext(_testDbPath);
        _repository = new SQLiteRepository<TestEntity>(_dbContext);
        _dbContext.CreateTableAsync<TestEntity>().GetAwaiter().GetResult();
    }

    [Fact]
    public async Task GetByIdAsync_WhenEntityExists_ShouldReturnEntity()
    {
        var entity = new TestEntity { Name = "Test" };
        await _repository.AddAsync(entity);

        var result = await _repository.GetByIdAsync(entity.Id);

        Assert.NotNull(result);
        Assert.Equal(entity.Id, result.Id);
        Assert.Equal(entity.Name, result.Name);
    }

    [Fact]
    public async Task GetByIdAsync_WhenEntityNotExists_ShouldReturnNull()
    {
        var result = await _repository.GetByIdAsync(99999);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllEntities()
    {
        await _repository.AddAsync(new TestEntity { Name = "Test1" });
        await _repository.AddAsync(new TestEntity { Name = "Test2" });

        var result = await _repository.GetAllAsync();

        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task FindAsync_WithMatchingPredicate_ShouldReturnFilteredEntities()
    {
        await _repository.AddAsync(new TestEntity { Name = "Test1" });
        await _repository.AddAsync(new TestEntity { Name = "Other" });
        await _repository.AddAsync(new TestEntity { Name = "Test2" });

        var result = await _repository.FindAsync(e => e.Name!.StartsWith("Test"));

        Assert.Equal(2, result.Count);
        Assert.All(result, e => Assert.StartsWith("Test", e.Name!));
    }

    [Fact]
    public async Task AddAsync_ShouldInsertEntity()
    {
        var entity = new TestEntity { Name = "Test" };

        await _repository.AddAsync(entity);

        Assert.True(entity.Id > 0);
        var retrieved = await _repository.GetByIdAsync(entity.Id);
        Assert.NotNull(retrieved);
        Assert.Equal(entity.Name, retrieved.Name);
    }

    [Fact]
    public async Task AddRangeAsync_ShouldInsertAllEntities()
    {
        var entities = new[]
        {
            new TestEntity { Name = "Test1" },
            new TestEntity { Name = "Test2" },
            new TestEntity { Name = "Test3" }
        };

        await _repository.AddRangeAsync(entities);

        var result = await _repository.GetAllAsync();
        Assert.Equal(3, result.Count);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateEntity()
    {
        var entity = new TestEntity { Name = "Original" };
        await _repository.AddAsync(entity);

        entity.Name = "Updated";
        await _repository.UpdateAsync(entity);

        var result = await _repository.GetByIdAsync(entity.Id);
        Assert.NotNull(result);
        Assert.Equal("Updated", result.Name);
    }

    [Fact]
    public async Task DeleteAsync_WhenEntityExists_ShouldRemoveEntity()
    {
        var entity = new TestEntity { Name = "Test" };
        await _repository.AddAsync(entity);

        await _repository.DeleteAsync(entity.Id);

        var result = await _repository.GetByIdAsync(entity.Id);
        Assert.Null(result);
    }

    [Fact]
    public async Task DeleteAsync_WhenEntityNotExists_ShouldNotThrowException()
    {
        var exception = await Record.ExceptionAsync(() => _repository.DeleteAsync(99999));
        Assert.Null(exception);
    }

    [Fact]
    public async Task CountAsync_ShouldReturnCorrectCount()
    {
        await _repository.AddAsync(new TestEntity { Name = "Test1" });
        await _repository.AddAsync(new TestEntity { Name = "Test2" });

        var count = await _repository.CountAsync();

        Assert.Equal(2, count);
    }

    [Fact]
    public async Task ExistsAsync_WhenEntityExists_ShouldReturnTrue()
    {
        var entity = new TestEntity { Name = "Test" };
        await _repository.AddAsync(entity);

        var exists = await _repository.ExistsAsync(entity.Id);

        Assert.True(exists);
    }

    [Fact]
    public async Task ExistsAsync_WhenEntityNotExists_ShouldReturnFalse()
    {
        var exists = await _repository.ExistsAsync(99999);

        Assert.False(exists);
    }

    public void Dispose()
    {
        _dbContext?.Dispose();
        if (File.Exists(_testDbPath))
        {
            File.Delete(_testDbPath);
        }
    }

    [Table("TestEntities")]
    private class TestEntity : IEntity
    {
        [PrimaryKey, AutoIncrement]
        public long Id { get; set; }
        public string? Name { get; set; }
    }
}
