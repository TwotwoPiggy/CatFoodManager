using CatFoodManager.Core.Interfaces;

namespace CatFoodManager.Core.Services
{
    /// <summary>
    /// 服务基类，提供仓储实例的公共访问。
    /// Service base class, providing common access to repository instance.
    /// </summary>
    public class ServiceBase
    {
        /// <summary>
        /// 仓储实例。
        /// Repository instance.
        /// </summary>
        protected readonly IRepository _repo;

        /// <summary>
        /// 构造函数。
        /// Constructor.
        /// </summary>
        /// <param name="repo">仓储实例 / Repository instance</param>
        public ServiceBase(IRepository repo)
        {
            _repo = repo;
        }
    }
}
