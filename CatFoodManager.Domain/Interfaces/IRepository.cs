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
    /// 获取所有实体。
    /// Gets all entities.
    /// </summary>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    /// <returns>实体只读列表 / Read-only list of entities</returns>
    Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 根据条件查找实体。
    /// Finds entities based on the specified condition.
    /// </summary>
    /// <param name="predicate">查询条件 / Query condition</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    /// <returns>实体只读列表 / Read-only list of entities</returns>
    Task<IReadOnlyList<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);

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
    /// 根据ID删除实体。
    /// Deletes an entity by ID.
    /// </summary>
    /// <param name="id">实体ID / Entity ID</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    Task DeleteAsync(long id, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取实体总数。
    /// Gets the total count of entities.
    /// </summary>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    /// <returns>实体总数 / Total count of entities</returns>
    Task<int> CountAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 检查实体是否存在。
    /// Checks if an entity exists.
    /// </summary>
    /// <param name="id">实体ID / Entity ID</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    /// <returns>实体是否存在 / Whether the entity exists</returns>
    Task<bool> ExistsAsync(long id, CancellationToken cancellationToken = default);
}
