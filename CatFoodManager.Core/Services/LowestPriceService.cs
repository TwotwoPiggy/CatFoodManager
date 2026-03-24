using CatFoodManager.Core.Interfaces;
using CatFoodManager.Core.Models;

namespace CatFoodManager.Core.Services
{
    /// <summary>
    /// 最低价格服务类，提供最低价格相关的业务操作。
    /// Lowest price service class, providing lowest price-related business operations.
    /// </summary>
    public class LowestPriceService : GenericServiceBase<BestPrice>, IService<BestPrice>
    {
        /// <summary>
        /// 构造函数。
        /// Constructor.
        /// </summary>
        /// <param name="repo">仓储实例 / Repository instance</param>
        /// <param name="needMigrate">是否需要执行数据库迁移 / Whether database migration is needed</param>
        public LowestPriceService(IRepository repo, bool needMigrate) : base(repo, needMigrate) { }

        /// <summary>
        /// 保存单个最低价格实体。
        /// Saves a single best price entity.
        /// </summary>
        /// <param name="bestPrice">要保存的最低价格实体 / The best price entity to save</param>
        public void Save(BestPrice bestPrice)
        {
            ArgumentNullException.ThrowIfNull(bestPrice);
            _repo.Add(bestPrice);
        }

        /// <summary>
        /// 批量保存最低价格实体。
        /// Batch saves best price entities.
        /// </summary>
        /// <param name="bestPrices">要保存的最低价格实体集合 / The collection of best price entities to save</param>
        public void BatchSave(IEnumerable<BestPrice> bestPrices)
        {
            ArgumentNullException.ThrowIfNull(bestPrices);
            var list = bestPrices as IList<BestPrice> ?? [.. bestPrices];
            if (list.Count == 0) return;
            _repo.BatchAdd(list);
        }

        /// <summary>
        /// 根据ID查询最低价格。
        /// Queries a best price by ID.
        /// </summary>
        /// <param name="id">最低价格ID / Best price ID</param>
        /// <returns>查询到的最低价格或null / The found best price or null</returns>
        public BestPrice? Query(long id) => _repo.Query<BestPrice>(price => price.Id == id, true);

        /// <summary>
        /// 根据名称查询最低价格。
        /// Queries a best price by name.
        /// </summary>
        /// <param name="catFoodName">猫粮名称 / Cat food name</param>
        /// <returns>查询到的最低价格或null / The found best price or null</returns>
        public BestPrice? Query(string catFoodName)
        {
            if (string.IsNullOrWhiteSpace(catFoodName)) return null;
            return _repo.Query<BestPrice>(price => price.Name == catFoodName, true);
        }

        /// <summary>
        /// 获取所有最低价格。
        /// Gets all best prices.
        /// </summary>
        /// <returns>最低价格列表 / List of best prices</returns>
        public IEnumerable<BestPrice> GetAll() => _repo.QueryList<BestPrice>(loadChildren: true);

        /// <summary>
        /// 获取所有最低价格及其总数。
        /// Gets all best prices with total count.
        /// </summary>
        /// <returns>最低价格列表和总数元组 / Tuple of best price list and total count</returns>
        public (IEnumerable<BestPrice>, int) GetAllWithCount()
        {
            var list = _repo.QueryList<BestPrice>(loadChildren: true).ToList();
            return (list, list.Count);
        }

        /// <summary>
        /// 执行模糊查询。
        /// Executes a fuzzy query.
        /// </summary>
        /// <param name="queryString">查询字符串 / Query string</param>
        /// <param name="args">查询参数 / Query arguments</param>
        /// <returns>最低价格列表 / List of best prices</returns>
        public IEnumerable<BestPrice> FuzzyQuery(string queryString, params object[] args) => _repo.FuzzyQueryWithChildren<BestPrice>(queryString, true, false, args);

        /// <summary>
        /// 执行模糊查询并返回总数。
        /// Executes a fuzzy query with total count.
        /// </summary>
        /// <param name="queryString">查询字符串 / Query string</param>
        /// <param name="args">查询参数 / Query arguments</param>
        /// <returns>最低价格列表和总数元组 / Tuple of best price list and total count</returns>
        public (IEnumerable<BestPrice>, int) FuzzyQueryWithCount(string queryString, params object[] args)
        {
            var list = _repo.FuzzyQueryWithChildren<BestPrice>(queryString, true, false, args).ToList();
            return (list, list.Count);
        }

        /// <summary>
        /// 更新最低价格实体。
        /// Updates a best price entity.
        /// </summary>
        /// <param name="price">要更新的最低价格实体 / The best price entity to update</param>
        public void Update(BestPrice price)
        {
            ArgumentNullException.ThrowIfNull(price);
            _repo.Update(price);
        }

        /// <summary>
        /// 根据ID删除最低价格。
        /// Deletes a best price by ID.
        /// </summary>
        /// <param name="id">最低价格ID / Best price ID</param>
        public void Delete(int id) => _repo.Delete<BestPrice>(id);
    }
}
