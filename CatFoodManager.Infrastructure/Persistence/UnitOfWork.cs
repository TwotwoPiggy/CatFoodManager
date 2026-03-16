using System.Collections.Concurrent;
using CatFoodManager.Domain.Interfaces;
using CatFoodManager.Infrastructure.Persistence;
using CatFoodManager.Infrastructure.Repositories;

namespace CatFoodManager.Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly IDbContext _dbContext;
    private readonly ConcurrentDictionary<Type, object> _repositories;
    private bool _disposed;

    public UnitOfWork(IDbContext dbContext)
    {
        _dbContext = dbContext;
        _repositories = new ConcurrentDictionary<Type, object>();
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await Task.FromResult(0).ConfigureAwait(false);
    }

    public IRepository<T> Repository<T>() where T : class, IEntity, new()
    {
        var type = typeof(T);

        if (_repositories.TryGetValue(type, out var repository))
        {
            return (IRepository<T>)repository;
        }

        var newRepository = new SQLiteRepository<T>(_dbContext);
        _repositories.TryAdd(type, newRepository);
        return newRepository;
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            _dbContext?.Dispose();
            _repositories.Clear();
            _disposed = true;
        }
        GC.SuppressFinalize(this);
    }
}
