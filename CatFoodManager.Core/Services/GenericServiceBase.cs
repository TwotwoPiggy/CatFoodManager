using CatFoodManager.Core.Interfaces;

namespace CatFoodManager.Core.Services
{
    /// <summary>
    /// 泛型服务基类，提供通用的数据库迁移功能。
    /// Generic service base class, providing common database migration functionality.
    /// </summary>
    /// <typeparam name="T">实体类型 / Entity type</typeparam>
    public class GenericServiceBase<T> : ServiceBase, IServiceBase
    {
        /// <summary>
        /// 构造函数。
        /// Constructor.
        /// </summary>
        /// <param name="repo">仓储实例 / Repository instance</param>
        /// <param name="needMigrate">是否需要执行数据库迁移 / Whether database migration is needed</param>
        public GenericServiceBase(IRepository repo, bool needMigrate) : base(repo)
        {
            if (needMigrate)
                Migrate<T>();
        }

        /// <summary>
        /// 为指定类型创建数据库表。
        /// Creates a database table for the specified type.
        /// </summary>
        /// <typeparam name="U">实体类型 / Entity type</typeparam>
        public void Migrate<U>()
        {
            _repo.Migrate<U>();
        }
    }
}
