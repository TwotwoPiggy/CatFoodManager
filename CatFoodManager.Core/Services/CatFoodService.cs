using CatFoodManager.Core.Interfaces;
using CatFoodManager.Core.Models;

namespace CatFoodManager.Core.Services
{
    /// <summary>
    /// 猫粮服务类，提供猫粮相关的业务操作。
    /// Cat food service class, providing cat food-related business operations.
    /// </summary>
    public class CatFoodService : GenericServiceBase<CatFood>, IService<CatFood>
    {
        /// <summary>
        /// 构造函数。
        /// Constructor.
        /// </summary>
        /// <param name="repo">仓储实例 / Repository instance</param>
        /// <param name="needMigrate">是否需要执行数据库迁移 / Whether database migration is needed</param>
        public CatFoodService(IRepository repo, bool needMigrate) : base(repo, needMigrate) { }

        /// <summary>
        /// 保存单个猫粮实体。
        /// Saves a single cat food entity.
        /// </summary>
        /// <param name="catFood">要保存的猫粮实体 / The cat food entity to save</param>
        public void Save(CatFood catFood)
        {
            ArgumentNullException.ThrowIfNull(catFood);
            _repo.Add(catFood);
        }

        /// <summary>
        /// 批量保存猫粮实体。
        /// Batch saves cat food entities.
        /// </summary>
        /// <param name="catFoods">要保存的猫粮实体集合 / The collection of cat food entities to save</param>
        public void BatchSave(IEnumerable<CatFood> catFoods)
        {
            ArgumentNullException.ThrowIfNull(catFoods);
            var list = catFoods as IList<CatFood> ?? [.. catFoods];
            if (list.Count == 0) return;
            _repo.BatchAdd(list);
        }

        /// <summary>
        /// 根据ID查询猫粮。
        /// Queries a cat food by ID.
        /// </summary>
        /// <param name="id">猫粮ID / Cat food ID</param>
        /// <returns>查询到的猫粮或null / The found cat food or null</returns>
        public CatFood? Query(long id) => _repo.Query<CatFood>(catFood => catFood.Id == id, true);

        /// <summary>
        /// 根据名称查询猫粮。
        /// Queries a cat food by name.
        /// </summary>
        /// <param name="catFoodName">猫粮名称 / Cat food name</param>
        /// <returns>查询到的猫粮或null / The found cat food or null</returns>
        public CatFood? Query(string catFoodName)
        {
            if (string.IsNullOrWhiteSpace(catFoodName)) return null;
            return _repo.Query<CatFood>(catFood => catFood.Name == catFoodName, true);
        }

        /// <summary>
        /// 获取所有猫粮。
        /// Gets all cat foods.
        /// </summary>
        /// <returns>猫粮列表 / List of cat foods</returns>
        public IEnumerable<CatFood> GetAll() => _repo.QueryList<CatFood>(loadChildren: true);

        /// <summary>
        /// 获取所有猫粮及其总数。
        /// Gets all cat foods with total count.
        /// </summary>
        /// <returns>猫粮列表和总数元组 / Tuple of cat food list and total count</returns>
        public (IEnumerable<CatFood>, int) GetAllWithCount()
        {
            var list = _repo.QueryList<CatFood>(loadChildren: true).ToList();
            return (list, list.Count);
        }

        /// <summary>
        /// 执行模糊查询。
        /// Executes a fuzzy query.
        /// </summary>
        /// <param name="queryString">查询字符串 / Query string</param>
        /// <param name="args">查询参数 / Query arguments</param>
        /// <returns>猫粮列表 / List of cat foods</returns>
        public IEnumerable<CatFood> FuzzyQuery(string queryString, params object[] args) => _repo.FuzzyQueryWithChildren<CatFood>(queryString, true, false, args);

        /// <summary>
        /// 执行模糊查询并返回总数。
        /// Executes a fuzzy query with total count.
        /// </summary>
        /// <param name="queryString">查询字符串 / Query string</param>
        /// <param name="args">查询参数 / Query arguments</param>
        /// <returns>猫粮列表和总数元组 / Tuple of cat food list and total count</returns>
        public (IEnumerable<CatFood>, int) FuzzyQueryWithCount(string queryString, params object[] args)
        {
            var list = _repo.FuzzyQueryWithChildren<CatFood>(queryString, true, false, args).ToList();
            return (list, list.Count);
        }

        /// <summary>
        /// 更新猫粮实体。
        /// Updates a cat food entity.
        /// </summary>
        /// <param name="catFood">要更新的猫粮实体 / The cat food entity to update</param>
        public void Update(CatFood catFood)
        {
            if (catFood is null) throw new ArgumentNullException(nameof(catFood));
            _repo.Update(catFood);
        }

        /// <summary>
        /// 根据ID删除猫粮。
        /// Deletes a cat food by ID.
        /// </summary>
        /// <param name="id">猫粮ID / Cat food ID</param>
        public void Delete(int id) => _repo.Delete<CatFood>(id);
    }
}
