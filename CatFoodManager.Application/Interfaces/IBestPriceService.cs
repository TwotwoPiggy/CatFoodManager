using CatFoodManager.Application.Common;
using CatFoodManager.Domain.Entities;

namespace CatFoodManager.Application.Interfaces;

/// <summary>
/// 最低价格服务接口，/// Best price service interface.
/// </summary>
public interface IBestPriceService
{
    /// <summary>
    /// 根据ID获取最低价格记录。
    /// Gets a best price record by ID.
    /// </summary>
    /// <param name="id">记录ID / Record ID</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    /// <returns>最低价格记录或 Best price record</returns>
    Task<BestPrice?> GetByIdAsync(long id, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取所有最低价格记录。
    /// Gets all best price records.
    /// </summary>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    /// <returns>最低价格记录列表 / List of best price records</returns>
    Task<IReadOnlyList<BestPrice>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 分页获取最低价格记录。
    /// Gets best price records with pagination.
    /// </summary>
    /// <param name="page">页码 / Page number</param>
    /// <param name="pageSize">每页大小 / Page size</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    /// <returns>分页结果 / Paged result</returns>
    Task<PagedResult<BestPrice>> GetPagedAsync(int page, int pageSize, CancellationToken cancellationToken = default);

    /// <summary>
    /// 搜索最低价格记录。
    /// Searches best price records.
    /// </summary>
    /// <param name="keyword">搜索关键词 / Search keyword</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    /// <returns>最低价格记录列表 / List of best price records</returns>
    Task<IReadOnlyList<BestPrice>> SearchAsync(string keyword, CancellationToken cancellationToken = default);

    /// <summary>
    /// 添加最低价格记录。
    /// Adds a best price record.
    /// </summary>
    /// <param name="entity">要添加的实体 / Entity to add</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    Task AddAsync(BestPrice entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// 批量添加最低价格记录。
    /// Adds a range of best price records.
    /// </summary>
    /// <param name="entities">要添加的实体集合 / Collection of entities to add</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    Task AddRangeAsync(IEnumerable<BestPrice> entities, CancellationToken cancellationToken = default);

    /// <summary>
    /// 更新最低价格记录。
    /// Updates a best price record.
    /// </summary>
    /// <param name="entity">要更新的实体 / Entity to update</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    Task UpdateAsync(BestPrice entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// 删除最低价格记录。
    /// Deletes a best price record.
    /// </summary>
    /// <param name="id">要删除的记录ID / ID of record to delete</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    Task DeleteAsync(long id, CancellationToken cancellationToken = default);
}
