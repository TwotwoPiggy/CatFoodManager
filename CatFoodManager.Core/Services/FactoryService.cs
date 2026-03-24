using CatFoodManager.Core.Interfaces;
using CatFoodManager.Core.Models;

namespace CatFoodManager.Core.Services
{
    /// <summary>
    /// 代工厂服务类，提供代工厂相关的业务操作。
    /// Factory service class, providing factory-related business operations.
    /// </summary>
    public class FactoryService : GenericServiceBase<Factory>, IService<Factory>
    {
        /// <summary>
        /// 构造函数。
        /// Constructor.
        /// </summary>
        /// <param name="repo">仓储实例 / Repository instance</param>
        /// <param name="needMigrate">是否需要执行数据库迁移 / Whether database migration is needed</param>
        public FactoryService(IRepository repo, bool needMigrate) : base(repo, needMigrate) { }

        /// <summary>
        /// 保存单个代工厂实体。
        /// Saves a single factory entity.
        /// </summary>
        /// <param name="factory">要保存的代工厂实体 / The factory entity to save</param>
        public void Save(Factory factory)
        {
            _repo.Add(factory);
        }

        /// <summary>
        /// 批量保存代工厂实体。
        /// Batch saves factory entities.
        /// </summary>
        /// <param name="factories">要保存的代工厂实体集合 / The collection of factory entities to save</param>
        public void BatchSave(IEnumerable<Factory> factories)
        {
            _repo.BatchAdd(factories);
        }

        /// <summary>
        /// 根据ID查询代工厂。
        /// Queries a factory by ID.
        /// </summary>
        /// <param name="id">代工厂ID / Factory ID</param>
        /// <returns>查询到的代工厂或null / The found factory or null</returns>
        public Factory? Query(long id)
        {
            return _repo.Query<Factory>(factory => factory.Id == id);
        }

        /// <summary>
        /// 根据名称查询代工厂。
        /// Queries a factory by name.
        /// </summary>
        /// <param name="factoryName">代工厂名称 / Factory name</param>
        /// <returns>查询到的代工厂或null / The found factory or null</returns>
        public Factory? Query(string factoryName)
        {
            return _repo.Query<Factory>(factory => factory.Name == factoryName);
        }

        /// <summary>
        /// 获取所有代工厂。
        /// Gets all factories.
        /// </summary>
        /// <returns>代工厂列表 / List of factories</returns>
        public IEnumerable<Factory> GetAll()
        {
            return _repo.QueryList<Factory>();
        }

        /// <summary>
        /// 获取所有代工厂及其总数。
        /// Gets all factories with total count.
        /// </summary>
        /// <returns>代工厂列表和总数元组 / Tuple of factory list and total count</returns>
        public (IEnumerable<Factory>, int) GetAllWithCount()
        {
            var list = GetAll();
            return (list, list.Count());
        }

        /// <summary>
        /// 执行模糊查询。
        /// Executes a fuzzy query.
        /// </summary>
        /// <param name="queryString">查询字符串 / Query string</param>
        /// <param name="args">查询参数 / Query arguments</param>
        /// <returns>代工厂列表 / List of factories</returns>
        public IEnumerable<Factory> FuzzyQuery(string queryString, params object[] args)
        {
            return _repo.FuzzyQueryWithChildren<Factory>(queryString, false, false, args);
        }

        /// <summary>
        /// 执行模糊查询并返回总数。
        /// Executes a fuzzy query with total count.
        /// </summary>
        /// <param name="queryString">查询字符串 / Query string</param>
        /// <param name="args">查询参数 / Query arguments</param>
        /// <returns>代工厂列表和总数元组 / Tuple of factory list and total count</returns>
        public (IEnumerable<Factory>, int) FuzzyQueryWithCount(string queryString, params object[] args)
        {
            var list = _repo.FuzzyQueryWithChildren<Factory>(queryString, false, false, args);
            return (list, list.Count());
        }

        /// <summary>
        /// 更新代工厂实体。
        /// Updates a factory entity.
        /// </summary>
        /// <param name="factory">要更新的代工厂实体 / The factory entity to update</param>
        public void Update(Factory factory)
        {
            _repo.Update(factory);
        }

        /// <summary>
        /// 根据ID删除代工厂。
        /// Deletes a factory by ID.
        /// </summary>
        /// <param name="id">代工厂ID / Factory ID</param>
        public void Delete(int id)
        {
            _repo.Delete<Factory>(id);
        }
    }
}
