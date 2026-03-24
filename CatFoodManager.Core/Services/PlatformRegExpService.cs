using CatFoodManager.Core.Interfaces;
using CatFoodManager.Core.Models;
using CatFoodManager.Core.Statics;

namespace CatFoodManager.Core.Services
{
    /// <summary>
    /// 平台正则表达式服务类，提供平台正则表达式相关的业务操作。
    /// Platform regular expression service class, providing platform regex-related business operations.
    /// </summary>
    public class PlatformRegExpService : GenericServiceBase<PlatformRegExp>, IPlatformRegExpService
    {
        /// <summary>
        /// 构造函数。
        /// Constructor.
        /// </summary>
        /// <param name="repo">仓储实例 / Repository instance</param>
        /// <param name="needMigrate">是否需要执行数据库迁移 / Whether database migration is needed</param>
        public PlatformRegExpService(IRepository repo, bool needMigrate) : base(repo, needMigrate) { }

        /// <summary>
        /// 保存单个平台正则表达式实体。
        /// Saves a single platform regular expression entity.
        /// </summary>
        /// <param name="platformRegExp">要保存的平台正则表达式实体 / The platform regular expression entity to save</param>
        public void Save(PlatformRegExp platformRegExp)
        {
            _repo.Add(platformRegExp);
        }

        /// <summary>
        /// 批量保存平台正则表达式实体。
        /// Batch saves platform regular expression entities.
        /// </summary>
        /// <param name="platformRegExp">要保存的平台正则表达式实体集合 / The collection of platform regular expression entities to save</param>
        public void BatchSave(IEnumerable<PlatformRegExp> platformRegExp)
        {
            _repo.BatchAdd(platformRegExp);
        }

        /// <summary>
        /// 根据ID查询平台正则表达式。
        /// Queries a platform regular expression by ID.
        /// </summary>
        /// <param name="id">平台正则表达式ID / Platform regular expression ID</param>
        /// <returns>查询到的平台正则表达式或null / The found platform regular expression or null</returns>
        public PlatformRegExp? Query(long id)
        {
            return _repo.Query<PlatformRegExp>(platformRegExp => platformRegExp.Id == id);
        }

        /// <summary>
        /// 根据名称查询平台正则表达式。
        /// Queries a platform regular expression by name.
        /// </summary>
        /// <param name="platformName">平台名称 / Platform name</param>
        /// <returns>查询到的平台正则表达式或null / The found platform regular expression or null</returns>
        public PlatformRegExp? Query(string platformName)
        {
            return _repo.Query<PlatformRegExp>(platformRegExp => platformRegExp.Name == platformName);
        }

        /// <summary>
        /// 根据平台类型获取正则表达式列表。
        /// Gets regular expression list by platform type.
        /// </summary>
        /// <param name="platformType">平台类型 / Platform type</param>
        /// <returns>正则表达式列表 / List of regular expressions</returns>
        public IEnumerable<PlatformRegExp> GetRegExpByPlatform(PlatformType platformType)
        {
            return _repo.QueryList<PlatformRegExp>(reg => reg.Platform == platformType);
        }

        /// <summary>
        /// 获取所有平台正则表达式。
        /// Gets all platform regular expressions.
        /// </summary>
        /// <returns>平台正则表达式列表 / List of platform regular expressions</returns>
        public IEnumerable<PlatformRegExp> GetAll()
        {
            return _repo.QueryList<PlatformRegExp>();
        }

        /// <summary>
        /// 获取所有平台正则表达式及其总数。
        /// Gets all platform regular expressions with total count.
        /// </summary>
        /// <returns>平台正则表达式列表和总数元组 / Tuple of platform regular expression list and total count</returns>
        public (IEnumerable<PlatformRegExp>, int) GetAllWithCount()
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
        /// <returns>平台正则表达式列表 / List of platform regular expressions</returns>
        public IEnumerable<PlatformRegExp> FuzzyQuery(string queryString, params object[] args)
        {
            return _repo.FuzzyQueryWithChildren<PlatformRegExp>(queryString, false, false, args);
        }

        /// <summary>
        /// 执行模糊查询并返回总数。
        /// Executes a fuzzy query with total count.
        /// </summary>
        /// <param name="queryString">查询字符串 / Query string</param>
        /// <param name="args">查询参数 / Query arguments</param>
        /// <returns>平台正则表达式列表和总数元组 / Tuple of platform regular expression list and total count</returns>
        public (IEnumerable<PlatformRegExp>, int) FuzzyQueryWithCount(string queryString, params object[] args)
        {
            var list = _repo.FuzzyQueryWithChildren<PlatformRegExp>(queryString, false, false, args);
            return (list, list.Count());
        }

        /// <summary>
        /// 更新平台正则表达式实体。
        /// Updates a platform regular expression entity.
        /// </summary>
        /// <param name="platformRegExp">要更新的平台正则表达式实体 / The platform regular expression entity to update</param>
        public void Update(PlatformRegExp platformRegExp)
        {
            _repo.Update(platformRegExp);
        }

        /// <summary>
        /// 根据ID删除平台正则表达式。
        /// Deletes a platform regular expression by ID.
        /// </summary>
        /// <param name="id">平台正则表达式ID / Platform regular expression ID</param>
        public void Delete(int id)
        {
            _repo.Delete<PlatformRegExp>(id);
        }
    }
}
