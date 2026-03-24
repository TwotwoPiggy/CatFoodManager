namespace CatFoodManager.Core.Interfaces
{
    /// <summary>
    /// 服务基础接口，提供数据库迁移功能。
    /// Service base interface, providing database migration functionality.
    /// </summary>
    public interface IServiceBase
    {
        /// <summary>
        /// 为指定类型创建数据库表。
        /// Creates a database table for the specified type.
        /// </summary>
        /// <typeparam name="T">实体类型 / Entity type</typeparam>
        void Migrate<T>();
    }
}
