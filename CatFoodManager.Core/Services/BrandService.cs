using CatFoodManager.Core.Interfaces;
using CatFoodManager.Core.Models;

namespace CatFoodManager.Core.Services
{
    /// <summary>
    /// 品牌服务类，提供品牌相关的业务操作。
    /// Brand service class, providing brand-related business operations.
    /// </summary>
    public class BrandService : GenericServiceBase<Brand>, IService<Brand>
    {
        /// <summary>
        /// 构造函数。
        /// Constructor.
        /// </summary>
        /// <param name="repo">仓储实例 / Repository instance</param>
        /// <param name="needMigrate">是否需要执行数据库迁移 / Whether database migration is needed</param>
        public BrandService(IRepository repo, bool needMigrate) : base(repo, needMigrate) { }

        /// <summary>
        /// 保存单个品牌实体。
        /// Saves a single brand entity.
        /// </summary>
        /// <param name="brand">要保存的品牌实体 / The brand entity to save</param>
        public void Save(Brand brand)
        {
            if (brand is null) throw new ArgumentNullException(nameof(brand));
            _repo.Add(brand);
        }

        /// <summary>
        /// 批量保存品牌实体。
        /// Batch saves brand entities.
        /// </summary>
        /// <param name="brands">要保存的品牌实体集合 / The collection of brand entities to save</param>
        public void BatchSave(IEnumerable<Brand> brands)
        {
            if (brands is null) throw new ArgumentNullException(nameof(brands));
            var list = brands as IList<Brand> ?? brands.ToList();
            if (list.Count == 0) return;
            _repo.BatchAdd(list);
        }

        /// <summary>
        /// 根据ID查询品牌。
        /// Queries a brand by ID.
        /// </summary>
        /// <param name="id">品牌ID / Brand ID</param>
        /// <returns>查询到的品牌或null / The found brand or null</returns>
        public Brand? Query(long id) => _repo.Query<Brand>(brand => brand.Id == id);

        /// <summary>
        /// 根据名称查询品牌。
        /// Queries a brand by name.
        /// </summary>
        /// <param name="brandName">品牌名称 / Brand name</param>
        /// <returns>查询到的品牌或null / The found brand or null</returns>
        public Brand? Query(string brandName)
        {
            if (string.IsNullOrWhiteSpace(brandName)) return null;
            return _repo.Query<Brand>(brand => brand.Name == brandName);
        }

        /// <summary>
        /// 获取所有品牌。
        /// Gets all brands.
        /// </summary>
        /// <returns>品牌列表 / List of brands</returns>
        public IEnumerable<Brand> GetAll() => _repo.QueryList<Brand>();

        /// <summary>
        /// 获取所有品牌及其总数。
        /// Gets all brands with total count.
        /// </summary>
        /// <returns>品牌列表和总数元组 / Tuple of brand list and total count</returns>
        public (IEnumerable<Brand>, int) GetAllWithCount()
        {
            var list = _repo.QueryList<Brand>().ToList();
            return (list, list.Count);
        }

        /// <summary>
        /// 执行模糊查询。
        /// Executes a fuzzy query.
        /// </summary>
        /// <param name="queryString">查询字符串 / Query string</param>
        /// <param name="args">查询参数 / Query arguments</param>
        /// <returns>品牌列表 / List of brands</returns>
        public IEnumerable<Brand> FuzzyQuery(string queryString, params object[] args) => _repo.FuzzyQueryWithChildren<Brand>(queryString, false, false, args);

        /// <summary>
        /// 执行模糊查询并返回总数。
        /// Executes a fuzzy query with total count.
        /// </summary>
        /// <param name="queryString">查询字符串 / Query string</param>
        /// <param name="args">查询参数 / Query arguments</param>
        /// <returns>品牌列表和总数元组 / Tuple of brand list and total count</returns>
        public (IEnumerable<Brand>, int) FuzzyQueryWithCount(string queryString, params object[] args)
        {
            var list = _repo.FuzzyQueryWithChildren<Brand>(queryString, false, false, args).ToList();
            return (list, list.Count);
        }

        /// <summary>
        /// 更新品牌实体。
        /// Updates a brand entity.
        /// </summary>
        /// <param name="brand">要更新的品牌实体 / The brand entity to update</param>
        public void Update(Brand brand)
        {
            if (brand is null) throw new ArgumentNullException(nameof(brand));
            _repo.Update(brand);
        }

        /// <summary>
        /// 根据ID删除品牌。
        /// Deletes a brand by ID.
        /// </summary>
        /// <param name="id">品牌ID / Brand ID</param>
        public void Delete(int id) => _repo.Delete<Brand>(id);
    }
}
