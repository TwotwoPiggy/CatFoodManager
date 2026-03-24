using CatFoodManager.Core.Interfaces;
using CatFoodManager.Core.Utils;
using CommonTools;
using CommonTools.Database;
using SQLiteNetExtensions.Extensions;
using System.Linq.Expressions;

namespace CatFoodManager.Core.Repositories
{
    /// <summary>
    /// 通用仓储类，实现数据访问的基本操作。
    /// Common repository class, implementing basic data access operations.
    /// </summary>
    public class CommonRepository : RepositoryBase, IRepository
    {
        /// <summary>
        /// 构造函数。
        /// Constructor.
        /// </summary>
        /// <param name="sqliteHelper">SQLite帮助类实例 / SQLite helper instance</param>
        public CommonRepository(SQLiteHelper sqliteHelper):base(sqliteHelper) { }

        /// <summary>
        /// 添加单个实体到数据库。
        /// Adds a single entity to the database.
        /// </summary>
        /// <typeparam name="T">实体类型 / Entity type</typeparam>
        /// <param name="entity">要添加的实体 / The entity to add</param>
        public void Add<T>(T entity) => _sqliteHelper.Db.Insert(entity);

        /// <summary>
        /// 批量添加实体到数据库。
        /// Batch adds entities to the database.
        /// </summary>
        /// <typeparam name="T">实体类型 / Entity type</typeparam>
        /// <param name="entities">要添加的实体集合 / The collection of entities to add</param>
        public void BatchAdd<T>(IEnumerable<T> entities) => _sqliteHelper.Db.InsertAll(entities);

        /// <summary>
        /// 更新实体。
        /// Updates an entity.
        /// </summary>
        /// <typeparam name="T">实体类型 / Entity type</typeparam>
        /// <param name="entity">要更新的实体 / The entity to update</param>
        public void Update<T>(T entity) where T : new() => _sqliteHelper.Db.Update(entity);

        /// <summary>
        /// 根据条件查询单个实体。
        /// Queries a single entity based on the specified condition.
        /// </summary>
        /// <typeparam name="T">实体类型 / Entity type</typeparam>
        /// <param name="predExpr">查询条件表达式 / Query condition expression</param>
        /// <param name="loadChildren">是否加载子实体 / Whether to load child entities</param>
        /// <param name="recursive">是否递归加载子实体 / Whether to recursively load child entities</param>
        /// <returns>查询到的实体或null / The found entity or null</returns>
        public T? Query<T>(Expression<Func<T, bool>> predExpr, bool loadChildren = false, bool recursive = false) where T : new()
        {
            if (loadChildren)
            {
                return _sqliteHelper.Db.GetAllWithChildren(predExpr, recursive: recursive)!.AsQueryable().FirstOrDefault(predExpr);
            }
            else
            {
                return _sqliteHelper.Db.Table<T>().FirstOrDefault(predExpr);
            }
        }

        /// <summary>
        /// 根据条件查询实体列表。
        /// Queries a list of entities based on the specified condition.
        /// </summary>
        /// <typeparam name="T">实体类型 / Entity type</typeparam>
        /// <param name="predExpr">查询条件表达式 / Query condition expression</param>
        /// <param name="loadChildren">是否加载子实体 / Whether to load child entities</param>
        /// <param name="recursive">是否递归加载子实体 / Whether to recursively load child entities</param>
        /// <returns>实体列表 / List of entities</returns>
        public IEnumerable<T> QueryList<T>(Expression<Func<T, bool>>? predExpr = null, bool loadChildren = false, bool recursive = false) where T : new()
        {
            if (loadChildren)
            {
                return predExpr == null ? _sqliteHelper.Db.GetAllWithChildren<T>(recursive : recursive) : _sqliteHelper.Db.GetAllWithChildren<T>(predExpr, recursive : recursive);
            }
            else
            {
                return predExpr == null ? _sqliteHelper.Db.Table<T>().AsQueryable() : _sqliteHelper.Db.Table<T>().Where(predExpr);
            }
        }

        /// <summary>
        /// 执行模糊查询。
        /// Executes a fuzzy query.
        /// </summary>
        /// <typeparam name="T">实体类型 / Entity type</typeparam>
        /// <param name="query">SQL查询语句 / SQL query statement</param>
        /// <param name="loadChilden">是否加载子实体 / Whether to load child entities</param>
        /// <param name="recursive">是否递归加载子实体 / Whether to recursively load child entities</param>
        /// <returns>实体列表 / List of entities</returns>
        public IEnumerable<T> FuzzyQuery<T>(string query, bool loadChilden = false, bool recursive = false) where T : new()
        {
            var queryResults = _sqliteHelper.Db.Query<T>(query);
            if (loadChilden)
            {
                queryResults.GetChildren(_sqliteHelper.Db);
            }
            return queryResults;
        }

        /// <summary>
        /// 执行带参数的模糊查询并加载子实体。
        /// Executes a fuzzy query with parameters and loads child entities.
        /// </summary>
        /// <typeparam name="T">实体类型 / Entity type</typeparam>
        /// <param name="query">SQL查询语句 / SQL query statement</param>
        /// <param name="loadChilden">是否加载子实体 / Whether to load child entities</param>
        /// <param name="recursive">是否递归加载子实体 / Whether to recursively load child entities</param>
        /// <param name="parameters">查询参数 / Query parameters</param>
        /// <returns>实体列表 / List of entities</returns>
        public IEnumerable<T> FuzzyQueryWithChildren<T>(string query, bool loadChilden = false, bool recursive = false, params object[] parameters) where T : new()
        {
            var queryResults = _sqliteHelper.Db.Query<T>(query, parameters);
            if (loadChilden)
            {
                queryResults.GetChildren(_sqliteHelper.Db);
            }
            return queryResults;
        }

        /// <summary>
        /// 根据主键删除实体。
        /// Deletes an entity by its primary key.
        /// </summary>
        /// <typeparam name="T">实体类型 / Entity type</typeparam>
        /// <param name="key">主键值 / Primary key value</param>
        public void Delete<T>(object key) => _sqliteHelper.Db.Delete<T>(key);
    }
}
