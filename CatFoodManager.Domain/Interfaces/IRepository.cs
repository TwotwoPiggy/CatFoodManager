using System.Linq.Expressions;

namespace CatFoodManager.Domain.Interfaces;

/// <summary>
/// 泛型仓储接口，提供实体的数据访问操作。
/// Generic repository interface, providing data access operations for entities.
/// </summary>
/// <typeparam name="T">实体类型 / Entity type</typeparam>
public interface IRepository<T> where T : class, IEntity
{
    /// <summary>
    /// 根据ID获取实体。
    /// Gets an entity by ID.
    /// </summary>
    /// <param name="id">实体ID / Entity ID</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    /// <returns>查询到的实体或null / The found entity or null</returns>
    Task<T?> GetByIdAsync(long id, CancellationToken cancellationToken = default);

    /// <summary>
    /// 根据ID获取实体及其关联实体。
    /// Gets an entity by ID with its related entities.
    /// </summary>
    /// <param name="id">实体ID / Entity ID</param>
    /// <param name="recursive">是否递归加载所有关联实体 / Whether to recursively load all related entities</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    /// <returns>查询到的实体或null / The found entity or null</returns>
    Task<T?> GetByIdWithChildrenAsync(long id, bool recursive = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取所有实体。
    /// Gets all entities.
    /// </summary>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    /// <returns>实体只读列表 / Read-only list of entities</returns>
    Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取所有实体及其关联实体。
    /// Gets all entities with their related entities.
    /// </summary>
    /// <param name="recursive">是否递归加载所有关联实体 / Whether to recursively load all related entities</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    /// <returns>实体只读列表 / Read-only list of entities</returns>
    Task<IReadOnlyList<T>> GetAllWithChildrenAsync(bool recursive = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// 根据条件查找实体。
    /// Finds entities based on the specified condition.
    /// </summary>
    /// <param name="predicate">查询条件 / Query condition</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    /// <returns>实体只读列表 / Read-only list of entities</returns>
    Task<IReadOnlyList<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);

    /// <summary>
    /// 根据条件查找实体及其关联实体。
    /// Finds entities with their related entities based on the specified condition.
    /// </summary>
    /// <param name="predicate">查询条件 / Query condition</param>
    /// <param name="recursive">是否递归加载所有关联实体 / Whether to recursively load all related entities</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    /// <returns>实体只读列表 / Read-only list of entities</returns>
    Task<IReadOnlyList<T>> FindWithChildrenAsync(Expression<Func<T, bool>> predicate, bool recursive = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// 执行原始SQL查询。
    /// Executes a raw SQL query.
    /// </summary>
    /// <param name="sql">SQL查询语句 / SQL query statement</param>
    /// <param name="parameters">查询参数 / Query parameters</param>
    /// <returns>实体只读列表 / Read-only list of entities</returns>
    IReadOnlyList<T> Query(string sql, params object[] parameters);

    /// <summary>
    /// 执行原始SQL查询并加载关联实体。
    /// Executes a raw SQL query and loads related entities.
    /// </summary>
    /// <param name="sql">SQL查询语句 / SQL query statement</param>
    /// <param name="recursive">是否递归加载所有关联实体 / Whether to recursively load all related entities</param>
    /// <param name="parameters">查询参数 / Query parameters</param>
    /// <returns>实体只读列表 / Read-only list of entities</returns>
    IReadOnlyList<T> QueryWithChildren(string sql, bool recursive = false, params object[] parameters);

    /// <summary>
    /// 添加实体。
    /// Adds an entity.
    /// </summary>
    /// <param name="entity">要添加的实体 / The entity to add</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    Task AddAsync(T entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// 批量添加实体。
    /// Adds a range of entities.
    /// </summary>
    /// <param name="entities">要添加的实体集合 / The collection of entities to add</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);

    /// <summary>
    /// 更新实体。
    /// Updates an entity.
    /// </summary>
    /// <param name="entity">要更新的实体 / The entity to update</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    Task UpdateAsync(T entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// 更新实体及其关联实体。
    /// Updates an entity with its related entities.
    /// </summary>
    /// <param name="entity">要更新的实体 / The entity to update</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    Task UpdateWithChildrenAsync(T entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// 根据ID删除实体。
    /// Deletes an entity by ID.
    /// </summary>
    /// <param name="id">实体ID / Entity ID</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    Task DeleteAsync(long id, CancellationToken cancellationToken = default);

    /// <summary>
    /// 根据条件批量删除实体。
    /// Deletes entities based on the specified condition.
    /// </summary>
    /// <param name="predicate">删除条件 / Delete condition</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    /// <returns>删除的实体数量 / Number of deleted entities</returns>
    Task<int> DeleteRangeAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取实体总数。
    /// Gets the total count of entities.
    /// </summary>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    /// <returns>实体总数 / Total count of entities</returns>
    Task<int> CountAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 根据条件获取实体数量。
    /// Gets the count of entities based on the specified condition.
    /// </summary>
    /// <param name="predicate">查询条件 / Query condition</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    /// <returns>实体数量 / Count of entities</returns>
    Task<int> CountAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);

    /// <summary>
    /// 检查实体是否存在。
    /// Checks if an entity exists.
    /// </summary>
    /// <param name="id">实体ID / Entity ID</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    /// <returns>实体是否存在 / Whether the entity exists</returns>
    Task<bool> ExistsAsync(long id, CancellationToken cancellationToken = default);

    /// <summary>
    /// 根据条件检查实体是否存在。
    /// Checks if any entity exists based on the specified condition.
    /// </summary>
    /// <param name="predicate">查询条件 / Query condition</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    /// <returns>实体是否存在 / Whether any entity exists</returns>
    Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
}
