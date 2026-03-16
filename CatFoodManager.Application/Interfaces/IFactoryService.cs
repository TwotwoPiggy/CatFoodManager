using CatFoodManager.Domain.Entities;

namespace CatFoodManager.Application.Interfaces;

public interface IFactoryService
{
    Task<Factory?> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Factory>> GetAllAsync(CancellationToken cancellationToken = default);
    Task AddAsync(Factory entity, CancellationToken cancellationToken = default);
    Task UpdateAsync(Factory entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(long id, CancellationToken cancellationToken = default);
}
