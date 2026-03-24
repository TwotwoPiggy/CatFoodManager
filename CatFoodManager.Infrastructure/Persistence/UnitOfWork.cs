using CatFoodManager.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace CatFoodManager.Infrastructure.Persistence;

/// <summary>
/// 工作单元实现类，提供事务管理和仓储访问。
/// Unit of work implementation class, providing transaction management and repository access.
/// </summary>
public class UnitOfWork : IUnitOfWork
{
    private readonly IDbContext _dbContext;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILoggerFactory _loggerFactory;
    private bool _disposed;

    /// <summary>
    /// 仓储实例缓存。
    /// Repository instance cache.
    /// </summary>
    private readonly Dictionary<Type, object> _repositories = new();

    /// <summary>
    /// 构造函数。
    /// Constructor.
    /// </summary>
    /// <param name="dbContext">数据库上下文 / Database context</param>
    /// <param name="serviceProvider">服务提供者 / Service provider</param>
    /// <param name="loggerFactory">日志工厂 / Logger factory</param>
    public UnitOfWork(IDbContext dbContext, IServiceProvider serviceProvider, ILoggerFactory loggerFactory)
    {
        _dbContext = dbContext;
        _serviceProvider = serviceProvider;
        _loggerFactory = loggerFactory;
    }

    /// <summary>
    /// 异步保存更改。
    /// Saves changes asynchronously.
    /// </summary>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    /// <returns>受影响的行数 / Number of affected rows</returns>
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(0);
    }

    /// <summary>
    /// 获取指定实体类型的仓储实例。
    /// Gets the repository instance for the specified entity type.
    /// </summary>
    /// <typeparam name="T">实体类型 / Entity type</typeparam>
    /// <returns>仓储实例 / Repository instance</returns>
    public IRepository<T> Repository<T>() where T : class, IEntity, new()
    {
        var type = typeof(T);

        if (!_repositories.TryGetValue(type, out var repository))
        {
            var logger = _loggerFactory.CreateLogger<Repositories.SQLiteRepository<T>>();
            repository = new Repositories.SQLiteRepository<T>(_dbContext, logger);
            _repositories[type] = repository;
        }

        return (IRepository<T>)repository;
    }

    /// <summary>
    /// 释放资源。
    /// Disposes resources.
    /// </summary>
    public void Dispose()
    {
        if (!_disposed)
        {
            _repositories.Clear();
            _disposed = true;
        }
        GC.SuppressFinalize(this);
    }
}
