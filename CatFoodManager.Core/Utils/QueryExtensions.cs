using CommonTools;
using SQLite;
using SQLiteNetExtensions.Extensions;

namespace CatFoodManager.Core.Utils
{
    /// <summary>
    /// 查询扩展方法类，提供查询相关的扩展方法。
    /// Query extensions class, providing query-related extension methods.
    /// </summary>
    public static class QueryExtensions
    {
        /// <summary>
        /// 为实体列表加载子实体。
        /// Loads child entities for a list of entities.
        /// </summary>
        /// <typeparam name="T">实体类型 / Entity type</typeparam>
        /// <param name="entities">实体列表 / List of entities</param>
        /// <param name="db">SQLite数据库连接 / SQLite database connection</param>
        /// <param name="recursive">是否递归加载 / Whether to load recursively</param>
        public static void GetChildren<T>(this List<T> entities, SQLiteConnection db, bool recursive = false)
        {
            foreach (var entity in entities)
            {
                db.GetChildren(entity, recursive);
            }
        }
    }
}
