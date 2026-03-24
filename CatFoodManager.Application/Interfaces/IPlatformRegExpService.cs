using CatFoodManager.Domain.Entities;

namespace CatFoodManager.Application.Interfaces;

/// <summary>
/// 平台正则表达式服务接口，提供平台正则表达式的查询功能。
/// Platform regular expression service interface, providing platform regex query functionality.
/// </summary>
public interface IPlatformRegExpService
{
    /// <summary>
    /// 获取所有平台正则表达式。
    /// Gets all platform regular expressions.
    /// </summary>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    /// <returns>平台正则表达式列表 / List of platform regular expressions</returns>
    Task<IReadOnlyList<PlatformRegExp>> GetAllAsync(CancellationToken cancellationToken = default);
}
