using SQLite;

namespace CatFoodManager.Infrastructure.Persistence;

public interface IDbContext : IDisposable
{
    SQLiteAsyncConnection Database { get; }
    Task CreateTableAsync<T>() where T : new();
    Task DropTableAsync<T>() where T : new();
}
