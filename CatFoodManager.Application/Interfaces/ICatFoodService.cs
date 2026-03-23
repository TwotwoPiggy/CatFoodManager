using CatFoodManager.Application.Common;
using CatFoodManager.Domain.Entities;

namespace CatFoodManager.Application.Interfaces;

/// <summary>
/// 猫粮服务接口
/// Cat food service interface.
/// </summary>
public interface ICatFoodService
{
    /// <summary>
    /// 根据ID获取猫粮。
    /// Gets a cat food by ID.
    /// </summary>
    /// <param name="id">猫粮ID / Cat food ID</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    /// <returns>猫粮实体或null / Cat food entity or null</returns>
    Task<CatFood?> GetByIdAsync(long id, CancellationToken cancellationToken = default);

    /// <summary>
    /// 根据名称获取猫粮。
    /// Gets a cat food by name.
    /// </summary>
    /// <param name="name">猫粮名称 / Cat food name</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    /// <returns>猫粮实体或null / Cat food entity or null</returns>
    Task<CatFood?> GetByNameAsync(string name, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取所有猫粮。
    /// Gets all cat foods.
    /// </summary>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    /// <returns>猫粮列表 / List of cat foods</returns>
    Task<IReadOnlyList<CatFood>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 分页获取猫粮。
    /// Gets cat foods with pagination.
    /// </summary>
    /// <param name="page">页码 / Page number</param>
    /// <param name="pageSize">每页大小 / Page size</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    /// <returns>分页结果 / Paged result</returns>
    Task<PagedResult<CatFood>> GetPagedAsync(int page, int pageSize, CancellationToken cancellationToken = default);

    /// <summary>
    /// 搜索猫粮。
    /// Searches cat foods.
    /// </summary>
    /// <param name="keyword">搜索关键词 / Search keyword</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    /// <returns>猫粮列表 / List of cat foods</returns>
    Task<IReadOnlyList<CatFood>> SearchAsync(string keyword, CancellationToken cancellationToken = default);

    /// <summary>
    /// 添加猫粮。
    /// Adds a cat food.
    /// </summary>
    /// <param name="entity">要添加的猫粮实体 / Cat food entity to add</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    Task AddAsync(CatFood entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// 批量添加猫粮。
    /// Adds a range of cat foods.
    /// </summary>
    /// <param name="entities">要添加的猫粮实体集合 / Collection of cat food entities to add</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    Task AddRangeAsync(IEnumerable<CatFood> entities, CancellationToken cancellationToken = default);

    /// <summary>
    /// 更新猫粮。
    /// Updates a cat food.
    /// </summary>
    /// <param name="entity">要更新的猫粮实体 / Cat food entity to update</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    Task UpdateAsync(CatFood entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// 删除猫粮。
    /// Deletes a cat food.
    /// </summary>
    /// <param name="id">要删除的猫粮ID / Cat food ID to delete</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    Task DeleteAsync(long id, CancellationToken cancellationToken = default);
}
