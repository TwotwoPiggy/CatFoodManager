using CatFoodManager.Application.Interfaces;
using CatFoodManager.Domain.Entities;
using CatFoodManager.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace CatFoodManager.Application.Services;

public class FactoryService : IFactoryService
{
    private readonly IRepository<Factory> _repository;
    private readonly ILogger<FactoryService> _logger;

    public FactoryService(IRepository<Factory> repository, ILogger<FactoryService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<Factory?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting factory by id: {Id}", id);
        return await _repository.GetByIdAsync(id, cancellationToken);
    }

    public async Task<IReadOnlyList<Factory>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting all factories");
        return await _repository.GetAllAsync(cancellationToken);
    }

    public async Task AddAsync(Factory entity, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Adding factory: {Name}", entity.Name);
        await _repository.AddAsync(entity, cancellationToken);
    }

    public async Task UpdateAsync(Factory entity, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Updating factory: {Id}", entity.Id);
        await _repository.UpdateAsync(entity, cancellationToken);
    }

    public async Task DeleteAsync(long id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Deleting factory: {Id}", id);
        await _repository.DeleteAsync(id, cancellationToken);
    }
}
