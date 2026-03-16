using CatFoodManager.Application.Common;
using CatFoodManager.Domain.Entities;

namespace CatFoodManager.Application.Interfaces;

public interface IBrandService
{
    Task<Brand?> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<Brand?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Brand>> GetAllAsync(CancellationToken cancellationToken = default);
    Task AddAsync(Brand entity, CancellationToken cancellationToken = default);
    Task UpdateAsync(Brand entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(long id, CancellationToken cancellationToken = default);
}
