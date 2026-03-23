using CatFoodManager.Core.Models;
using CatFoodManager.Core.Statics;

namespace CatFoodManager.Core.Interfaces
{
    /// <summary>
    /// 平台正则表达式服务接口，提供平台相关的正则表达式管理功能。
    /// Platform regular expression service interface, providing platform-related regex management functionality.
    /// </summary>
    public interface IPlatformRegExpService: IService<PlatformRegExp>
    {
        /// <summary>
        /// 根据平台类型获取正则表达式列表。
        /// Gets regular expression list by platform type.
        /// </summary>
        /// <param name="platformType">平台类型 / Platform type</param>
        /// <returns>正则表达式列表 / List of regular expressions</returns>
        IEnumerable<PlatformRegExp> GetRegExpByPlatform(PlatformType platformType);
    }
}
