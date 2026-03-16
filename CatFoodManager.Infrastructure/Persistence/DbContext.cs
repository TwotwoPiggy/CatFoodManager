using SQLite;

namespace CatFoodManager.Infrastructure.Persistence;

public class DbContext : IDbContext
{
    private readonly SQLiteAsyncConnection _database;
    private bool _disposed;

    public DbContext(string databasePath)
    {
        _database = new SQLiteAsyncConnection(databasePath);
    }

    public SQLiteAsyncConnection Database => _database;

    public async Task CreateTableAsync<T>() where T : new()
    {
        await _database.CreateTableAsync<T>().ConfigureAwait(false);
    }

    public async Task DropTableAsync<T>() where T : new()
    {
        await _database.DropTableAsync<T>().ConfigureAwait(false);
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            _disposed = true;
        }
        GC.SuppressFinalize(this);
    }
}
