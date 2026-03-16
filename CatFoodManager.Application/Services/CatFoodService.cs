using CatFoodManager.Application.Common;
using CatFoodManager.Application.Interfaces;
using CatFoodManager.Domain.Entities;
using CatFoodManager.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace CatFoodManager.Application.Services;

public class CatFoodService : ICatFoodService
{
    private readonly IRepository<CatFood> _repository;
    private readonly ILogger<CatFoodService> _logger;

    public CatFoodService(IRepository<CatFood> repository, ILogger<CatFoodService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<CatFood?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting cat food by id: {Id}", id);
        return await _repository.GetByIdAsync(id, cancellationToken);
    }

    public async Task<CatFood?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting cat food by name: {Name}", name);
        var result = await _repository.FindAsync(e => e.Name == name, cancellationToken);
        return result.FirstOrDefault();
    }

    public async Task<IReadOnlyList<CatFood>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting all cat foods");
        return await _repository.GetAllAsync(cancellationToken);
    }

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

    public async Task<IReadOnlyList<CatFood>> SearchAsync(string keyword, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Searching cat foods with keyword: {Keyword}", keyword);
        return await _repository.FindAsync(e => e.Name != null && e.Name.Contains(keyword), cancellationToken);
    }

    public async Task AddAsync(CatFood entity, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Adding cat food: {Name}", entity.Name);
        await _repository.AddAsync(entity, cancellationToken);
    }

    public async Task AddRangeAsync(IEnumerable<CatFood> entities, CancellationToken cancellationToken = default)
    {
        var entityList = entities.ToList();
        _logger.LogInformation("Adding {Count} cat foods", entityList.Count);
        await _repository.AddRangeAsync(entityList, cancellationToken);
    }

    public async Task UpdateAsync(CatFood entity, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Updating cat food: {Id}", entity.Id);
        await _repository.UpdateAsync(entity, cancellationToken);
    }

    public async Task DeleteAsync(long id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Deleting cat food: {Id}", id);
        await _repository.DeleteAsync(id, cancellationToken);
    }
}
