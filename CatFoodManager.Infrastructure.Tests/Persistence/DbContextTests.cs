using CatFoodManager.Infrastructure.Persistence;
using SQLite;
using Xunit;

namespace CatFoodManager.Infrastructure.Tests.Persistence;

public class DbContextTests : IDisposable
{
    private readonly string _testDbPath;
    private readonly DbContext _dbContext;

    public DbContextTests()
    {
        _testDbPath = Path.Combine(Path.GetTempPath(), $"test_{Guid.NewGuid()}.db");
        _dbContext = new DbContext(_testDbPath);
    }

    [Fact]
    public void Constructor_ShouldInitializeDatabase()
    {
        Assert.NotNull(_dbContext.Database);
        Assert.IsType<SQLiteAsyncConnection>(_dbContext.Database);
    }

    [Fact]
    public void Database_ShouldReturnSameInstance()
    {
        var database1 = _dbContext.Database;
        var database2 = _dbContext.Database;

        Assert.Same(database1, database2);
    }

    [Fact]
    public async Task CreateTableAsync_ShouldCreateTable()
    {
        await _dbContext.CreateTableAsync<TestEntity>();

        var entity = new TestEntity { Name = "Test" };
        var result = await _dbContext.Database.InsertAsync(entity);
        Assert.Equal(1, result);
    }

    [Fact]
    public async Task DropTableAsync_ShouldDropTable()
    {
        await _dbContext.CreateTableAsync<TestEntity>();
        await _dbContext.DropTableAsync<TestEntity>();

        var entity = new TestEntity { Name = "Test" };
        await Assert.ThrowsAsync<SQLiteException>(() => _dbContext.Database.InsertAsync(entity));
    }

    [Fact]
    public void Dispose_ShouldNotThrowException()
    {
        var dbContext = new DbContext(_testDbPath);
        var exception = Record.Exception(() => dbContext.Dispose());
        Assert.Null(exception);
    }

    [Fact]
    public void Dispose_CalledMultipleTimes_ShouldNotThrowException()
    {
        var dbContext = new DbContext(_testDbPath);
        dbContext.Dispose();
        var exception = Record.Exception(() => dbContext.Dispose());
        Assert.Null(exception);
    }

    public void Dispose()
    {
        _dbContext?.Dispose();
        if (File.Exists(_testDbPath))
        {
            File.Delete(_testDbPath);
        }
    }

    private class TestEntity
    {
        [PrimaryKey, AutoIncrement]
        public long Id { get; set; }
        public string? Name { get; set; }
    }
}
