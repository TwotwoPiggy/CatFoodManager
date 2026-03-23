namespace CatFoodManager.Core.Interfaces
{
    /// <summary>
    /// 仓储基础接口，提供数据库迁移功能。
    /// Repository base interface, providing database migration functionality.
    /// </summary>
    public interface IRepositoryBase
    {
        /// <summary>
        /// 为指定类型创建数据库表。
        /// Creates a database table for the specified type.
        /// </summary>
        /// <typeparam name="T">实体类型 / Entity type</typeparam>
        void Migrate<T>();
    }
}
