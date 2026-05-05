using System.Linq.Expressions;
using CatFoodManager.Domain.Interfaces;
using CatFoodManager.Infrastructure.Persistence;
using Microsoft.Extensions.Logging;
using SQLite;
using SQLiteNetExtensions.Extensions;

namespace CatFoodManager.Infrastructure.Repositories;

/// <summary>
/// SQLite仓储实现类，提供基于SQLite的数据访问操作。
/// SQLite repository implementation class, providing SQLite-based data access operations.
/// </summary>
/// <typeparam name="T">实体类型 / Entity type</typeparam>
public class SQLiteRepository<T> : IRepository<T> where T : class, IEntity, new()
{
    private readonly IDbContext _dbContext;
    private readonly ILogger<SQLiteRepository<T>> _logger;

    /// <summary>
    /// 构造函数。
    /// Constructor.
    /// </summary>
    /// <param name="dbContext">数据库上下文 / Database context</param>
    /// <param name="logger">日志记录器 / Logger</param>
    public SQLiteRepository(IDbContext dbContext, ILogger<SQLiteRepository<T>> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    /// <summary>
    /// 根据ID获取实体。
    /// Gets an entity by ID.
    /// </summary>
    /// <param name="id">实体ID / Entity ID</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    /// <returns>查询到的实体或null / The found entity or null</returns>
    public async Task<T?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Getting {EntityType} by id: {Id}", typeof(T).Name, id);
        return await Task.Run(() => _dbContext.Connection.Find<T>(id), cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// 根据ID获取实体及其关联实体。
    /// Gets an entity by ID with its related entities.
    /// </summary>
    /// <param name="id">实体ID / Entity ID</param>
    /// <param name="recursive">是否递归加载所有关联实体 / Whether to recursively load all related entities</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    /// <returns>查询到的实体或null / The found entity or null</returns>
    public async Task<T?> GetByIdWithChildrenAsync(long id, bool recursive = false, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Getting {EntityType} by id: {Id} with children (recursive: {Recursive})", typeof(T).Name, id, recursive);
        return await Task.Run(() =>
        {
            var entity = _dbContext.Connection.Find<T>(id);
            if (entity != null)
            {
                _dbContext.Connection.GetChildren(entity, recursive);
            }
            return entity;
        }, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// 获取所有实体。
    /// Gets all entities.
    /// </summary>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    /// <returns>实体只读列表 / Read-only list of entities</returns>
    public async Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Getting all {EntityType}", typeof(T).Name);
        var items = await Task.Run(() => _dbContext.Connection.Table<T>().ToList(), cancellationToken).ConfigureAwait(false);
        return items.AsReadOnly();
    }

    /// <summary>
    /// 获取所有实体及其关联实体。
    /// Gets all entities with their related entities.
    /// </summary>
    /// <param name="recursive">是否递归加载所有关联实体 / Whether to recursively load all related entities</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    /// <returns>实体只读列表 / Read-only list of entities</returns>
    public async Task<IReadOnlyList<T>> GetAllWithChildrenAsync(bool recursive = false, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Getting all {EntityType} with children (recursive: {Recursive})", typeof(T).Name, recursive);
        var items = await Task.Run(() => _dbContext.Connection.GetAllWithChildren<T>(recursive: recursive), cancellationToken).ConfigureAwait(false);
        return items.AsReadOnly();
    }

    /// <summary>
    /// 根据条件查找实体。
    /// Finds entities based on the specified condition.
    /// </summary>
    /// <param name="predicate">查询条件 / Query condition</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    /// <returns>实体只读列表 / Read-only list of entities</returns>
    public async Task<IReadOnlyList<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Finding {EntityType} with predicate", typeof(T).Name);
        var items = await Task.Run(() => _dbContext.Connection.Table<T>().Where(predicate).ToList(), cancellationToken).ConfigureAwait(false);
        return items.AsReadOnly();
    }

    /// <summary>
    /// 根据条件查找实体及其关联实体。
    /// Finds entities with their related entities based on the specified condition.
    /// </summary>
    /// <param name="predicate">查询条件 / Query condition</param>
    /// <param name="recursive">是否递归加载所有关联实体 / Whether to recursively load all related entities</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    /// <returns>实体只读列表 / Read-only list of entities</returns>
    public async Task<IReadOnlyList<T>> FindWithChildrenAsync(Expression<Func<T, bool>> predicate, bool recursive = false, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Finding {EntityType} with predicate and children (recursive: {Recursive})", typeof(T).Name, recursive);
        var items = await Task.Run(() =>
        {
            var results = _dbContext.Connection.Table<T>().Where(predicate).ToList();
            foreach (var item in results)
            {
                _dbContext.Connection.GetChildren(item, recursive);
            }
            return results;
        }, cancellationToken).ConfigureAwait(false);
        return items.AsReadOnly();
    }

    /// <summary>
    /// 执行原始SQL查询。
    /// Executes a raw SQL query.
    /// </summary>
    /// <param name="sql">SQL查询语句 / SQL query statement</param>
    /// <param name="parameters">查询参数 / Query parameters</param>
    /// <returns>实体只读列表 / Read-only list of entities</returns>
    public IReadOnlyList<T> Query(string sql, params object[] parameters)
    {
        _logger.LogDebug("Executing raw SQL query for {EntityType}: {Sql}", typeof(T).Name, sql);
        var items = _dbContext.Connection.Query<T>(sql, parameters);
        return items.AsReadOnly();
    }

    /// <summary>
    /// 执行原始SQL查询并加载关联实体。
    /// Executes a raw SQL query and loads related entities.
    /// </summary>
    /// <param name="sql">SQL查询语句 / SQL query statement</param>
    /// <param name="recursive">是否递归加载所有关联实体 / Whether to recursively load all related entities</param>
    /// <param name="parameters">查询参数 / Query parameters</param>
    /// <returns>实体只读列表 / Read-only list of entities</returns>
    public IReadOnlyList<T> QueryWithChildren(string sql, bool recursive = false, params object[] parameters)
    {
        _logger.LogDebug("Executing raw SQL query for {EntityType} with children: {Sql}", typeof(T).Name, sql);
        var items = _dbContext.Connection.Query<T>(sql, parameters);
        foreach (var item in items)
        {
            _dbContext.Connection.GetChildren(item, recursive);
        }
        return items.AsReadOnly();
    }

    /// <summary>
    /// 添加实体。
    /// Adds an entity.
    /// </summary>
    /// <param name="entity">要添加的实体 / The entity to add</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    public async Task AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Adding {EntityType}", typeof(T).Name);
        await Task.Run(() => _dbContext.Connection.Insert(entity), cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// 批量添加实体。
    /// Adds a range of entities.
    /// </summary>
    /// <param name="entities">要添加的实体集合 / The collection of entities to add</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    public async Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
    {
        var entityList = entities.ToList();
        _logger.LogDebug("Adding {Count} {EntityType} entities", entityList.Count, typeof(T).Name);
        await Task.Run(() => _dbContext.Connection.InsertAll(entityList), cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// 更新实体。
    /// Updates an entity.
    /// </summary>
    /// <param name="entity">要更新的实体 / The entity to update</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    public async Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Updating {EntityType} with id: {Id}", typeof(T).Name, entity.Id);
        await Task.Run(() => _dbContext.Connection.Update(entity), cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// 更新实体及其关联实体。
    /// Updates an entity with its related entities.
    /// </summary>
    /// <param name="entity">要更新的实体 / The entity to update</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    public async Task UpdateWithChildrenAsync(T entity, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Updating {EntityType} with id: {Id} and children", typeof(T).Name, entity.Id);
        await Task.Run(() => _dbContext.Connection.UpdateWithChildren(entity), cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// 根据ID删除实体。
    /// Deletes an entity by ID.
    /// </summary>
    /// <param name="id">实体ID / Entity ID</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    public async Task DeleteAsync(long id, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Deleting {EntityType} with id: {Id}", typeof(T).Name, id);
        await Task.Run(() => _dbContext.Connection.Delete<T>(id), cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// 根据条件批量删除实体。
    /// Deletes entities based on the specified condition.
    /// </summary>
    /// <param name="predicate">删除条件 / Delete condition</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    /// <returns>删除的实体数量 / Number of deleted entities</returns>
    public async Task<int> DeleteRangeAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Deleting {EntityType} entities with predicate", typeof(T).Name);
        return await Task.Run(() =>
        {
            var items = _dbContext.Connection.Table<T>().Where(predicate).ToList();
            if (items.Count == 0) return 0;

            _dbContext.Connection.BeginTransaction();
            try
            {
                foreach (var item in items)
                {
                    _dbContext.Connection.Delete(item);
                }
                _dbContext.Connection.Commit();
                _logger.LogDebug("Deleted {Count} {EntityType} entities", items.Count, typeof(T).Name);
                return items.Count;
            }
            catch
            {
                _dbContext.Connection.Rollback();
                throw;
            }
        }, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// 获取实体总数。
    /// Gets the total count of entities.
    /// </summary>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    /// <returns>实体总数 / Total count of entities</returns>
    public async Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        return await Task.Run(() => _dbContext.Connection.Table<T>().Count(), cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// 根据条件获取实体数量。
    /// Gets the count of entities based on the specified condition.
    /// </summary>
    /// <param name="predicate">查询条件 / Query condition</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    /// <returns>实体数量 / Count of entities</returns>
    public async Task<int> CountAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await Task.Run(() => _dbContext.Connection.Table<T>().Where(predicate).Count(), cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// 检查实体是否存在。
    /// Checks if an entity exists.
    /// </summary>
    /// <param name="id">实体ID / Entity ID</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    /// <returns>实体是否存在 / Whether the entity exists</returns>
    public async Task<bool> ExistsAsync(long id, CancellationToken cancellationToken = default)
    {
        var entity = await GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
        return entity != null;
    }

    /// <summary>
    /// 根据条件检查实体是否存在。
    /// Checks if any entity exists based on the specified condition.
    /// </summary>
    /// <param name="predicate">查询条件 / Query condition</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    /// <returns>实体是否存在 / Whether any entity exists</returns>
    public async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        var count = await CountAsync(predicate, cancellationToken).ConfigureAwait(false);
        return count > 0;
    }
}
