using CatFoodManager.Application.Interfaces;
using CatFoodManager.Domain.Entities;
using CatFoodManager.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace CatFoodManager.Application.Services;

public class BrandService : IBrandService
{
    private readonly IRepository<Brand> _repository;
    private readonly ILogger<BrandService> _logger;

    public BrandService(IRepository<Brand> repository, ILogger<BrandService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<Brand?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting brand by id: {Id}", id);
        return await _repository.GetByIdAsync(id, cancellationToken);
    }

    public async Task<Brand?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting brand by name: {Name}", name);
        var result = await _repository.FindAsync(e => e.Name == name, cancellationToken);
        return result.FirstOrDefault();
    }

    public async Task<IReadOnlyList<Brand>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting all brands");
        return await _repository.GetAllAsync(cancellationToken);
    }

    public async Task AddAsync(Brand entity, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Adding brand: {Name}", entity.Name);
        await _repository.AddAsync(entity, cancellationToken);
    }

    public async Task UpdateAsync(Brand entity, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Updating brand: {Id}", entity.Id);
        await _repository.UpdateAsync(entity, cancellationToken);
    }

    public async Task DeleteAsync(long id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Deleting brand: {Id}", id);
        await _repository.DeleteAsync(id, cancellationToken);
    }
}
