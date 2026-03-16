using CatFoodManager.Application.Common;
using CatFoodManager.Domain.Entities;

namespace CatFoodManager.Application.Interfaces;

public interface ICatFoodService
{
    Task<CatFood?> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<CatFood?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<CatFood>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<PagedResult<CatFood>> GetPagedAsync(int page, int pageSize, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<CatFood>> SearchAsync(string keyword, CancellationToken cancellationToken = default);
    Task AddAsync(CatFood entity, CancellationToken cancellationToken = default);
    Task AddRangeAsync(IEnumerable<CatFood> entities, CancellationToken cancellationToken = default);
    Task UpdateAsync(CatFood entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(long id, CancellationToken cancellationToken = default);
}
