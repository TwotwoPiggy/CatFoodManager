using CatFoodManager.Application.Interfaces;
using CatFoodManager.Domain.Entities;
using CatFoodManager.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace CatFoodManager.Application.Services;

/// <summary>
/// 品牌服务类，提供品牌相关的业务操作。
/// Brand service class, providing brand-related business operations.
/// </summary>
public class BrandService : IBrandService
{
    private readonly IRepository<Brand> _repository;
    private readonly ILogger<BrandService> _logger;

    /// <summary>
    /// 构造函数。
    /// Constructor.
    /// </summary>
    /// <param name="repository">仓储实例 / Repository instance</param>
    /// <param name="logger">日志记录器 / Logger</param>
    public BrandService(IRepository<Brand> repository, ILogger<BrandService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    /// <summary>
    /// 根据ID获取品牌。
    /// Gets a brand by ID.
    /// </summary>
    /// <param name="id">品牌ID / Brand ID</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    /// <returns>品牌实体或null / Brand entity or null</returns>
    public async Task<Brand?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting brand by id: {Id}", id);
        return await _repository.GetByIdAsync(id, cancellationToken);
    }

    /// <summary>
    /// 根据名称获取品牌。
    /// Gets a brand by name.
    /// </summary>
    /// <param name="name">品牌名称 / Brand name</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    /// <returns>品牌实体或null / Brand entity or null</returns>
    public async Task<Brand?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting brand by name: {Name}", name);
        var result = await _repository.FindAsync(e => e.Name == name, cancellationToken);
        return result.FirstOrDefault();
    }

    /// <summary>
    /// 获取所有品牌。
    /// Gets all brands.
    /// </summary>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    /// <returns>品牌列表 / List of brands</returns>
    public async Task<IReadOnlyList<Brand>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting all brands");
        return await _repository.GetAllAsync(cancellationToken);
    }

    /// <summary>
    /// 搜索品牌。
    /// Searches brands by keyword.
    /// </summary>
    /// <param name="searchKey">搜索关键词 / Search keyword</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    /// <returns>品牌列表 / List of brands</returns>
    public async Task<IReadOnlyList<Brand>> SearchAsync(string searchKey, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Searching brands with key: {SearchKey}", searchKey);
        
        if (string.IsNullOrWhiteSpace(searchKey))
        {
            return await GetAllAsync(cancellationToken);
        }

        if (long.TryParse(searchKey, out var id))
        {
            var brand = await GetByIdAsync(id, cancellationToken);
            return brand != null ? new List<Brand> { brand }.AsReadOnly() : Array.Empty<Brand>();
        }

        return await _repository.FindAsync(
            b => b.Name != null && b.Name.Contains(searchKey, StringComparison.OrdinalIgnoreCase),
            cancellationToken);
    }

    /// <summary>
    /// 添加品牌。
    /// Adds a brand.
    /// </summary>
    /// <param name="entity">要添加的品牌实体 / Brand entity to add</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    public async Task AddAsync(Brand entity, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Adding brand: {Name}", entity.Name);
        await _repository.AddAsync(entity, cancellationToken);
    }

    /// <summary>
    /// 更新品牌。
    /// Updates a brand.
    /// </summary>
    /// <param name="entity">要更新的品牌实体 / Brand entity to update</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    public async Task UpdateAsync(Brand entity, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Updating brand: {Id}", entity.Id);
        await _repository.UpdateAsync(entity, cancellationToken);
    }

    /// <summary>
    /// 删除品牌。
    /// Deletes a brand.
    /// </summary>
    /// <param name="id">要删除的品牌ID / Brand ID to delete</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    public async Task DeleteAsync(long id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Deleting brand: {Id}", id);
        await _repository.DeleteAsync(id, cancellationToken);
    }
}
