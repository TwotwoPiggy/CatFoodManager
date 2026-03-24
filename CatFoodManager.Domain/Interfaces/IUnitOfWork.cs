namespace CatFoodManager.Domain.Interfaces;

/// <summary>
/// 工作单元接口，提供事务管理和仓储访问。
/// Unit of work interface, providing transaction management and repository access.
/// </summary>
public interface IUnitOfWork : IDisposable
{
    /// <summary>
    /// 异步保存更改。
    /// Saves changes asynchronously.
    /// </summary>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    /// <returns>受影响的行数 / Number of affected rows</returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取指定实体类型的仓储实例。
    /// Gets the repository instance for the specified entity type.
    /// </summary>
    /// <typeparam name="T">实体类型 / Entity type</typeparam>
    /// <returns>仓储实例 / Repository instance</returns>
    IRepository<T> Repository<T>() where T : class, IEntity, new();
}
