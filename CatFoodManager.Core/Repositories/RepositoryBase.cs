using CatFoodManager.Core.Interfaces;
using CommonTools.Database;

namespace CatFoodManager.Core.Repositories
{
    /// <summary>
    /// 仓储基类，提供数据库操作的公共基础功能。
    /// Repository base class, providing common base functionality for database operations.
    /// </summary>
    public abstract class RepositoryBase : IRepositoryBase
    {
        /// <summary>
        /// SQLite帮助类实例。
        /// SQLite helper instance.
        /// </summary>
        protected readonly SQLiteHelper _sqliteHelper;

        /// <summary>
        /// 构造函数。
        /// Constructor.
        /// </summary>
        /// <param name="sqliteHelper">SQLite帮助类实例 / SQLite helper instance</param>
        public RepositoryBase(SQLiteHelper sqliteHelper) => _sqliteHelper = sqliteHelper;

        /// <summary>
        /// 为指定类型创建数据库表。
        /// Creates a database table for the specified type.
        /// </summary>
        /// <typeparam name="T">实体类型 / Entity type</typeparam>
        public void Migrate<T>()
        {
            _sqliteHelper.Db.CreateTable<T>();
        }
    }
}
