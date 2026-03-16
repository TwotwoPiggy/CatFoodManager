using CatFoodManager.Application.Common;
using CatFoodManager.Domain.Entities;

namespace CatFoodManager.Application.Interfaces;

public interface IBestPriceService
{
    Task<BestPrice?> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<BestPrice>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<PagedResult<BestPrice>> GetPagedAsync(int page, int pageSize, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<BestPrice>> SearchAsync(string keyword, CancellationToken cancellationToken = default);
    Task AddAsync(BestPrice entity, CancellationToken cancellationToken = default);
    Task AddRangeAsync(IEnumerable<BestPrice> entities, CancellationToken cancellationToken = default);
    Task UpdateAsync(BestPrice entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(long id, CancellationToken cancellationToken = default);
}
