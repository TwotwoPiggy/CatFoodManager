using CatFoodManager.Application.Common;
using CatFoodManager.Application.Interfaces;
using CatFoodManager.Domain.Entities;
using CatFoodManager.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace CatFoodManager.Application.Services;

public class BestPriceService : IBestPriceService
{
    private readonly IRepository<BestPrice> _repository;
    private readonly ILogger<BestPriceService> _logger;

    public BestPriceService(IRepository<BestPrice> repository, ILogger<BestPriceService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<BestPrice?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting best price by id: {Id}", id);
        return await _repository.GetByIdAsync(id, cancellationToken);
    }

    public async Task<IReadOnlyList<BestPrice>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting all best prices");
        return await _repository.GetAllAsync(cancellationToken);
    }

    public async Task<PagedResult<BestPrice>> GetPagedAsync(int page, int pageSize, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting paged best prices: page {Page}, pageSize {PageSize}", page, pageSize);
        var allItems = await _repository.GetAllAsync(cancellationToken);
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

    public async Task<IReadOnlyList<BestPrice>> SearchAsync(string keyword, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Searching best prices with keyword: {Keyword}", keyword);
        return await _repository.FindAsync(e => e.Name != null && e.Name.Contains(keyword), cancellationToken);
    }

    public async Task AddAsync(BestPrice entity, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Adding best price: {Name}", entity.Name);
        await _repository.AddAsync(entity, cancellationToken);
    }

    public async Task AddRangeAsync(IEnumerable<BestPrice> entities, CancellationToken cancellationToken = default)
    {
        var entityList = entities.ToList();
        _logger.LogInformation("Adding {Count} best prices", entityList.Count);
        await _repository.AddRangeAsync(entityList, cancellationToken);
    }

    public async Task UpdateAsync(BestPrice entity, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Updating best price: {Id}", entity.Id);
        await _repository.UpdateAsync(entity, cancellationToken);
    }

    public async Task DeleteAsync(long id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Deleting best price: {Id}", id);
        await _repository.DeleteAsync(id, cancellationToken);
    }
}
