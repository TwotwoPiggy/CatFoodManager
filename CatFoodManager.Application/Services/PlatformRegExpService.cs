using CatFoodManager.Application.Interfaces;
using CatFoodManager.Domain.Entities;
using CatFoodManager.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace CatFoodManager.Application.Services;

/// <summary>
/// 平台正则表达式服务类，提供平台正则表达式的查询功能。
/// Platform regular expression service class, providing platform regex query functionality.
/// </summary>
public class PlatformRegExpService : IPlatformRegExpService
{
    private readonly IRepository<PlatformRegExp> _repository;
    private readonly ILogger<PlatformRegExpService> _logger;

    /// <summary>
    /// 构造函数。
    /// Constructor.
    /// </summary>
    /// <param name="repository">仓储实例 / Repository instance</param>
    /// <param name="logger">日志记录器 / Logger</param>
    public PlatformRegExpService(IRepository<PlatformRegExp> repository, ILogger<PlatformRegExpService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    /// <summary>
    /// 获取所有平台正则表达式。
    /// Gets all platform regular expressions.
    /// </summary>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    /// <returns>平台正则表达式列表 / List of platform regular expressions</returns>
    public async Task<IReadOnlyList<PlatformRegExp>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting all platform regexps");
        return await _repository.GetAllAsync(cancellationToken);
    }
}
