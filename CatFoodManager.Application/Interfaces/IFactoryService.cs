using CatFoodManager.Domain.Entities;

namespace CatFoodManager.Application.Interfaces;

/// <summary>
/// 代工厂服务接�?/// Factory service interface.
/// </summary>
public interface IFactoryService
{
    /// <summary>
    /// 根据ID获取代工厂�?    /// Gets a factory by ID.
    /// </summary>
    /// <param name="id">代工厂ID / Factory ID</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    /// <returns>代工厂实体或null / Factory entity or null</returns>
    Task<Factory?> GetByIdAsync(long id, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取所有代工厂�?    /// Gets all factories.
    /// </summary>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    /// <returns>代工厂列�?/ List of factories</returns>
    Task<IReadOnlyList<Factory>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 添加代工厂�?    /// Adds a factory.
    /// </summary>
    /// <param name="entity">要添加的代工厂实�?/ Factory entity to add</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    Task AddAsync(Factory entity, CancellationToken cancellationToken = default);
}
