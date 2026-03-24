using CatFoodManager.Infrastructure.Persistence;
using SQLite;

namespace CatFoodManager.Infrastructure.Tests.Integration;

public class TestDatabaseFixture
{
    public DbContext CreateDbContext()
    {
        var databasePath = Path.Combine(Path.GetTempPath(), $"test_{Guid.NewGuid()}.db");
        var dbContext = new DbContext(databasePath);
        InitializeDatabase(dbContext).GetAwaiter().GetResult();
        return dbContext;
    }

    private async Task InitializeDatabase(DbContext dbContext)
    {
        await dbContext.CreateTableAsync<TestCatFood>();
        await dbContext.CreateTableAsync<TestBrand>();
        await dbContext.CreateTableAsync<TestFactory>();
        await dbContext.CreateTableAsync<TestBestPrice>();
    }

    public void Cleanup(DbContext dbContext)
    {
        dbContext.Dispose();
    }
}
