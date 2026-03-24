using CatFoodManager.Application.Interfaces;
using CatFoodManager.Domain.Entities;
using CatFoodManager.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace CatFoodManager.Application.Services;

/// <summary>
/// 代工厂服务类，提供代工厂相关的业务操作。
/// Factory service class, providing factory-related business operations.
/// </summary>
public class FactoryService : IFactoryService
{
    private readonly IRepository<Factory> _repository;
    private readonly ILogger<FactoryService> _logger;

    /// <summary>
    /// 构造函数。
    /// Constructor.
    /// </summary>
    /// <param name="repository">仓储实例 / Repository instance</param>
    /// <param name="logger">日志记录器 / Logger</param>
    public FactoryService(IRepository<Factory> repository, ILogger<FactoryService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    /// <summary>
    /// 根据ID获取代工厂。
    /// Gets a factory by ID.
    /// </summary>
    /// <param name="id">代工厂ID / Factory ID</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    /// <returns>代工厂实体或null / Factory entity or null</returns>
    public async Task<Factory?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting factory by id: {Id}", id);
        return await _repository.GetByIdAsync(id, cancellationToken);
    }

    /// <summary>
    /// 获取所有代工厂。
    /// Gets all factories.
    /// </summary>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    /// <returns>代工厂列表 / List of factories</returns>
    public async Task<IReadOnlyList<Factory>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting all factories");
        return await _repository.GetAllAsync(cancellationToken);
    }

    /// <summary>
    /// 添加代工厂。
    /// Adds a factory.
    /// </summary>
    /// <param name="entity">要添加的代工厂实体 / Factory entity to add</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    public async Task AddAsync(Factory entity, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Adding factory: {Name}", entity.Name);
        await _repository.AddAsync(entity, cancellationToken);
    }

    /// <summary>
    /// 更新代工厂。
    /// Updates a factory.
    /// </summary>
    /// <param name="entity">要更新的代工厂实体 / Factory entity to update</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    public async Task UpdateAsync(Factory entity, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Updating factory: {Id}", entity.Id);
        await _repository.UpdateAsync(entity, cancellationToken);
    }

    /// <summary>
    /// 删除代工厂。
    /// Deletes a factory.
    /// </summary>
    /// <param name="id">要删除的代工厂ID / Factory ID to delete</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    public async Task DeleteAsync(long id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Deleting factory: {Id}", id);
        await _repository.DeleteAsync(id, cancellationToken);
    }
}
