using System.Linq.Expressions;
using CatFoodManager.Domain.Interfaces;
using CatFoodManager.Infrastructure.Persistence;
using SQLite;

namespace CatFoodManager.Infrastructure.Repositories;

public class SQLiteRepository<T> : IRepository<T> where T : class, IEntity, new()
{
    private readonly SQLiteAsyncConnection _database;

    public SQLiteRepository(IDbContext dbContext)
    {
        _database = dbContext.Database;
    }

    public async Task<T?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        return await _database.FindAsync<T>(id).ConfigureAwait(false);
    }

    public async Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var result = await _database.Table<T>().ToListAsync().ConfigureAwait(false);
        return result.AsReadOnly();
    }

    public async Task<IReadOnlyList<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        var result = await _database.Table<T>().Where(predicate).ToListAsync().ConfigureAwait(false);
        return result.AsReadOnly();
    }

    public async Task AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        await _database.InsertAsync(entity).ConfigureAwait(false);
    }

    public async Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
    {
        await _database.InsertAllAsync(entities).ConfigureAwait(false);
    }

    public async Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
    {
        await _database.UpdateAsync(entity).ConfigureAwait(false);
    }

    public async Task DeleteAsync(long id, CancellationToken cancellationToken = default)
    {
        var entity = await GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
        if (entity != null)
        {
            await _database.DeleteAsync(entity).ConfigureAwait(false);
        }
    }

    public async Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        return await _database.Table<T>().CountAsync().ConfigureAwait(false);
    }

    public async Task<bool> ExistsAsync(long id, CancellationToken cancellationToken = default)
    {
        var entity = await GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
        return entity != null;
    }
}
