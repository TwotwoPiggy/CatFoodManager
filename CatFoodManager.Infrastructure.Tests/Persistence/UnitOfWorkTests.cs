using CatFoodManager.Domain.Interfaces;
using CatFoodManager.Infrastructure.Persistence;
using SQLite;
using Xunit;

namespace CatFoodManager.Infrastructure.Tests.Persistence;

public class UnitOfWorkTests : IDisposable
{
    private readonly string _testDbPath;
    private readonly DbContext _dbContext;
    private readonly UnitOfWork _unitOfWork;

    public UnitOfWorkTests()
    {
        _testDbPath = Path.Combine(Path.GetTempPath(), $"test_{Guid.NewGuid()}.db");
        _dbContext = new DbContext(_testDbPath);
        _unitOfWork = new UnitOfWork(_dbContext);
    }

    [Fact]
    public async Task SaveChangesAsync_ShouldReturnZero()
    {
        var result = await _unitOfWork.SaveChangesAsync();

        Assert.Equal(0, result);
    }

    [Fact]
    public void Repository_ShouldReturnRepositoryInstance()
    {
        var repository = _unitOfWork.Repository<TestEntity>();

        Assert.NotNull(repository);
        Assert.IsAssignableFrom<IRepository<TestEntity>>(repository);
    }

    [Fact]
    public void Repository_CalledMultipleTimesForSameType_ShouldReturnSameInstance()
    {
        var repository1 = _unitOfWork.Repository<TestEntity>();
        var repository2 = _unitOfWork.Repository<TestEntity>();

        Assert.Same(repository1, repository2);
    }

    [Fact]
    public void Repository_CalledForDifferentTypes_ShouldReturnDifferentInstances()
    {
        var repository1 = _unitOfWork.Repository<TestEntity>();
        var repository2 = _unitOfWork.Repository<AnotherTestEntity>();

        Assert.NotSame(repository1, repository2);
    }

    [Fact]
    public void Dispose_ShouldNotThrowException()
    {
        var exception = Record.Exception(() => _unitOfWork.Dispose());
        Assert.Null(exception);
    }

    [Fact]
    public void Dispose_CalledMultipleTimes_ShouldNotThrowException()
    {
        _unitOfWork.Dispose();
        var exception = Record.Exception(() => _unitOfWork.Dispose());
        Assert.Null(exception);
    }

    public void Dispose()
    {
        _unitOfWork?.Dispose();
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

    [Table("AnotherTestEntities")]
    private class AnotherTestEntity : IEntity
    {
        [PrimaryKey, AutoIncrement]
        public long Id { get; set; }
        public string? Description { get; set; }
    }
}
