using CatFoodManager.Application.Common;
using CatFoodManager.Application.Interfaces;
using CatFoodManager.Domain.Entities;
using CatFoodManager.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace CatFoodManager.Application.Services;

/// <summary>
/// 最低价格服务类，提供最低价格相关的业务操作�?/// Best price service class, providing best price-related business operations.
/// </summary>
public class BestPriceService : IBestPriceService
{
    private readonly IRepository<BestPrice> _repository;
    private readonly ILogger<BestPriceService> _logger;

    /// <summary>
    /// 构造函数�?    /// Constructor.
    /// </summary>
    /// <param name="repository">仓储实例 / Repository instance</param>
    /// <param name="logger">日志记录�?/ Logger</param>
    public BestPriceService(IRepository<BestPrice> repository, ILogger<BestPriceService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    /// <summary>
    /// 根据ID获取最低价格记录�?    /// Gets a best price record by ID.
    /// </summary>
    /// <param name="id">记录ID / Record ID</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    /// <returns>最低价格记录或null / Best price record or null</returns>
    public async Task<BestPrice?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting best price by id: {Id}", id);
        return await _repository.GetByIdAsync(id, cancellationToken);
    }

    /// <summary>
    /// 获取所有最低价格记录�?    /// Gets all best price records.
    /// </summary>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    /// <returns>最低价格记录列�?/ List of best price records</returns>
    public async Task<IReadOnlyList<BestPrice>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting all best prices");
        return await _repository.GetAllAsync(cancellationToken);
    }

    /// <summary>
    /// 分页获取最低价格记录。
    /// Gets best price records with pagination.
    /// </summary>
    /// <param name="page">页码 / Page number</param>
    /// <param name="pageSize">每页大小 / Page size</param>
    /// <param name="searchKey">搜索关键词（可选）/ Search keyword (optional)</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    /// <returns>分页结果 / Paged result</returns>
    public async Task<PagedResult<BestPrice>> GetPagedAsync(int page, int pageSize, string? searchKey = null, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting paged best prices: page {Page}, pageSize {PageSize}, searchKey: {SearchKey}", page, pageSize, searchKey);
        
        IReadOnlyList<BestPrice> allItems;
        
        if (!string.IsNullOrWhiteSpace(searchKey))
        {
            allItems = await _repository.FindAsync(e => e.Name != null && e.Name.Contains(searchKey), cancellationToken);
        }
        else
        {
            allItems = await _repository.GetAllAsync(cancellationToken);
        }
        
        var totalCount = allItems.Count;
        var items = allItems.Skip((page - 1) * pageSize).Take(pageSize).ToList();

        return new PagedResult<BestPrice>
        {
            Items = items,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    /// <summary>
    /// 搜索最低价格记录�?    /// Searches best price records.
    /// </summary>
    /// <param name="keyword">搜索关键�?/ Search keyword</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    /// <returns>最低价格记录列�?/ List of best price records</returns>
    public async Task<IReadOnlyList<BestPrice>> SearchAsync(string keyword, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Searching best prices with keyword: {Keyword}", keyword);
        return await _repository.FindAsync(e => e.Name != null && e.Name.Contains(keyword), cancellationToken);
    }

    /// <summary>
    /// 添加最低价格记录�?    /// Adds a best price record.
    /// </summary>
    /// <param name="entity">要添加的实体 / Entity to add</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    public async Task AddAsync(BestPrice entity, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Adding best price: {Name}", entity.Name);
        await _repository.AddAsync(entity, cancellationToken);
    }

    /// <summary>
    /// 批量添加最低价格记录�?    /// Adds a range of best price records.
    /// </summary>
    /// <param name="entities">要添加的实体集合 / Collection of entities to add</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    public async Task AddRangeAsync(IEnumerable<BestPrice> entities, CancellationToken cancellationToken = default)
    {
        var entityList = entities.ToList();
        _logger.LogInformation("Adding {Count} best prices", entityList.Count);
        await _repository.AddRangeAsync(entityList, cancellationToken);
    }

    /// <summary>
    /// 更新最低价格记录�?    /// Updates a best price record.
    /// </summary>
    /// <param name="entity">要更新的实体 / Entity to update</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    public async Task UpdateAsync(BestPrice entity, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Updating best price: {Id}", entity.Id);
        await _repository.UpdateAsync(entity, cancellationToken);
    }

    /// <summary>
    /// 删除最低价格记录�?    /// Deletes a best price record.
    /// </summary>
    /// <param name="id">要删除的记录ID / ID of record to delete</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    public async Task DeleteAsync(long id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Deleting best price: {Id}", id);
        await _repository.DeleteAsync(id, cancellationToken);
    }

    /// <summary>
    /// 批量删除最低价格记录�?    /// Deletes multiple best price records.
    /// </summary>
    /// <param name="ids">要删除的记录ID集合 / Collection of record IDs to delete</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    /// <returns>删除的记录数量 / Number of deleted records</returns>
    public async Task<int> DeleteRangeAsync(IEnumerable<long> ids, CancellationToken cancellationToken = default)
    {
        var idList = ids.ToList();
        _logger.LogInformation("Deleting {Count} best prices", idList.Count);
        return await _repository.DeleteRangeAsync(e => idList.Contains(e.Id), cancellationToken);
    }
}
