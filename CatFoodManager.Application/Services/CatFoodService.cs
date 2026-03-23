using CatFoodManager.Application.Common;
using CatFoodManager.Application.Interfaces;
using CatFoodManager.Domain.Entities;
using CatFoodManager.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace CatFoodManager.Application.Services;

/// <summary>
/// 猫粮服务类，提供猫粮相关的业务操作。
/// Cat food service class, providing cat food-related business operations.
/// </summary>
public class CatFoodService : ICatFoodService
{
    private readonly IRepository<CatFood> _repository;
    private readonly ILogger<CatFoodService> _logger;

    /// <summary>
    /// 构造函数。
    /// Constructor.
    /// </summary>
    /// <param name="repository">仓储实例 / Repository instance</param>
    /// <param name="logger">日志记录器 / Logger</param>
    public CatFoodService(IRepository<CatFood> repository, ILogger<CatFoodService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    /// <summary>
    /// 根据ID获取猫粮。
    /// Gets a cat food by ID.
    /// </summary>
    /// <param name="id">猫粮ID / Cat food ID</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    /// <returns>猫粮实体或null / Cat food entity or null</returns>
    public async Task<CatFood?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting cat food by id: {Id}", id);
        return await _repository.GetByIdAsync(id, cancellationToken);
    }

    /// <summary>
    /// 根据名称获取猫粮。
    /// Gets a cat food by name.
    /// </summary>
    /// <param name="name">猫粮名称 / Cat food name</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    /// <returns>猫粮实体或null / Cat food entity or null</returns>
    public async Task<CatFood?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting cat food by name: {Name}", name);
        var result = await _repository.FindAsync(e => e.Name == name, cancellationToken);
        return result.FirstOrDefault();
    }

    /// <summary>
    /// 获取所有猫粮。
    /// Gets all cat foods.
    /// </summary>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    /// <returns>猫粮列表 / List of cat foods</returns>
    public async Task<IReadOnlyList<CatFood>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting all cat foods");
        return await _repository.GetAllAsync(cancellationToken);
    }

    /// <summary>
    /// 分页获取猫粮。
    /// Gets cat foods with pagination.
    /// </summary>
    /// <param name="page">页码 / Page number</param>
    /// <param name="pageSize">每页大小 / Page size</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    /// <returns>分页结果 / Paged result</returns>
    public async Task<PagedResult<CatFood>> GetPagedAsync(int page, int pageSize, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting paged cat foods: page {Page}, pageSize {PageSize}", page, pageSize);
        var allItems = await _repository.GetAllAsync(cancellationToken);
        var totalCount = allItems.Count;
        var items = allItems.Skip((page - 1) * pageSize).Take(pageSize).ToList();

        return new PagedResult<CatFood>
        {
            Items = items,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    /// <summary>
    /// 搜索猫粮。
    /// Searches cat foods.
    /// </summary>
    /// <param name="keyword">搜索关键词 / Search keyword</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    /// <returns>猫粮列表 / List of cat foods</returns>
    public async Task<IReadOnlyList<CatFood>> SearchAsync(string keyword, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Searching cat foods with keyword: {Keyword}", keyword);
        return await _repository.FindAsync(e => e.Name != null && e.Name.Contains(keyword), cancellationToken);
    }

    /// <summary>
    /// 添加猫粮。
    /// Adds a cat food.
    /// </summary>
    /// <param name="entity">要添加的猫粮实体 / Cat food entity to add</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    public async Task AddAsync(CatFood entity, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Adding cat food: {Name}", entity.Name);
        await _repository.AddAsync(entity, cancellationToken);
    }

    /// <summary>
    /// 批量添加猫粮。
    /// Adds a range of cat foods.
    /// </summary>
    /// <param name="entities">要添加的猫粮实体集合 / Collection of cat food entities to add</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    public async Task AddRangeAsync(IEnumerable<CatFood> entities, CancellationToken cancellationToken = default)
    {
        var entityList = entities.ToList();
        _logger.LogInformation("Adding {Count} cat foods", entityList.Count);
        await _repository.AddRangeAsync(entityList, cancellationToken);
    }

    /// <summary>
    /// 更新猫粮。
    /// Updates a cat food.
    /// </summary>
    /// <param name="entity">要更新的猫粮实体 / Cat food entity to update</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    public async Task UpdateAsync(CatFood entity, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Updating cat food: {Id}", entity.Id);
        await _repository.UpdateAsync(entity, cancellationToken);
    }

    /// <summary>
    /// 删除猫粮。
    /// Deletes a cat food.
    /// </summary>
    /// <param name="id">要删除的猫粮ID / Cat food ID to delete</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    public async Task DeleteAsync(long id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Deleting cat food: {Id}", id);
        await _repository.DeleteAsync(id, cancellationToken);
    }
}
