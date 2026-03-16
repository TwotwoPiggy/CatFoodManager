using CatFoodManager.Application.Interfaces;
using CatFoodManager.Domain.Entities;
using CatFoodManager.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace CatFoodManager.Application.Services;

public class PlatformRegExpService : IPlatformRegExpService
{
    private readonly IRepository<PlatformRegExp> _repository;
    private readonly ILogger<PlatformRegExpService> _logger;

    public PlatformRegExpService(IRepository<PlatformRegExp> repository, ILogger<PlatformRegExpService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<IReadOnlyList<PlatformRegExp>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting all platform regexps");
        return await _repository.GetAllAsync(cancellationToken);
    }
}
