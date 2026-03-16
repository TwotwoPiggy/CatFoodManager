using CatFoodManager.Domain.Entities;

namespace CatFoodManager.Application.Interfaces;

public interface IPlatformRegExpService
{
    Task<IReadOnlyList<PlatformRegExp>> GetAllAsync(CancellationToken cancellationToken = default);
}
