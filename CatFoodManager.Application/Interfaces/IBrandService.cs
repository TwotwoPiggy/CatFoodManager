using CatFoodManager.Domain.Entities;

namespace CatFoodManager.Application.Interfaces;

/// <summary>
/// 品牌服务接口
/// Brand service interface.
/// </summary>
public interface IBrandService
{
    /// <summary>
    /// 根据ID获取品牌。
    /// Gets a brand by ID.
    /// </summary>
    /// <param name="id">品牌ID / Brand ID</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    /// <returns>品牌实体或null / Brand entity or null</returns>
    Task<Brand?> GetByIdAsync(long id, CancellationToken cancellationToken = default);

    /// <summary>
    /// 根据名称获取品牌。
    /// Gets a brand by name.
    /// </summary>
    /// <param name="name">品牌名称 / Brand name</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    /// <returns>品牌实体或null / Brand entity or null</returns>
    Task<Brand?> GetByNameAsync(string name, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取所有品牌。
    /// Gets all brands.
    /// </summary>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    /// <returns>品牌列表 / List of brands</returns>
    Task<IReadOnlyList<Brand>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 搜索品牌。
    /// Searches brands by keyword.
    /// </summary>
    /// <param name="searchKey">搜索关键词 / Search keyword</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    /// <returns>品牌列表 / List of brands</returns>
    Task<IReadOnlyList<Brand>> SearchAsync(string searchKey, CancellationToken cancellationToken = default);

    /// <summary>
    /// 添加品牌。
    /// Adds a brand.
    /// </summary>
    /// <param name="entity">要添加的品牌实体 / Brand entity to add</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    Task AddAsync(Brand entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// 更新品牌。
    /// Updates a brand.
    /// </summary>
    /// <param name="entity">要更新的品牌实体 / Brand entity to update</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    Task UpdateAsync(Brand entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// 删除品牌。
    /// Deletes a brand.
    /// </summary>
    /// <param name="id">要删除的品牌ID / Brand ID to delete</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    Task DeleteAsync(long id, CancellationToken cancellationToken = default);
}
