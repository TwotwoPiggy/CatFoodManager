namespace CatFoodManager.Core.Interfaces
{
    /// <summary>
    /// 服务接口，提供实体的增删改查操作。
    /// Service interface, providing CRUD operations for entities.
    /// </summary>
    /// <typeparam name="T">实体类型 / Entity type</typeparam>
    public interface IService<T>
    {
        /// <summary>
        /// 保存单个实体。
        /// Saves a single entity.
        /// </summary>
        /// <param name="entity">要保存的实体 / The entity to save</param>
        void Save(T entity);

        /// <summary>
        /// 批量保存实体。
        /// Batch saves entities.
        /// </summary>
        /// <param name="entities">要保存的实体集合 / The collection of entities to save</param>
        void BatchSave(IEnumerable<T> entities);

        /// <summary>
        /// 根据ID查询实体。
        /// Queries an entity by ID.
        /// </summary>
        /// <param name="id">实体ID / Entity ID</param>
        /// <returns>查询到的实体或null / The found entity or null</returns>
        T? Query(long id);

        /// <summary>
        /// 根据名称查询实体。
        /// Queries an entity by name.
        /// </summary>
        /// <param name="name">实体名称 / Entity name</param>
        /// <returns>查询到的实体或null / The found entity or null</returns>
        T? Query(string name);

        /// <summary>
        /// 获取所有实体。
        /// Gets all entities.
        /// </summary>
        /// <returns>实体列表 / List of entities</returns>
        IEnumerable<T> GetAll();

        /// <summary>
        /// 获取所有实体及其总数。
        /// Gets all entities with total count.
        /// </summary>
        /// <returns>实体列表和总数元组 / Tuple of entity list and total count</returns>
        (IEnumerable<T>, int) GetAllWithCount();

        /// <summary>
        /// 执行模糊查询。
        /// Executes a fuzzy query.
        /// </summary>
        /// <param name="queryString">查询字符串 / Query string</param>
        /// <param name="args">查询参数 / Query arguments</param>
        /// <returns>实体列表 / List of entities</returns>
        IEnumerable<T> FuzzyQuery(string queryString, params object[] args);

        /// <summary>
        /// 执行模糊查询并返回总数。
        /// Executes a fuzzy query with total count.
        /// </summary>
        /// <param name="queryString">查询字符串 / Query string</param>
        /// <param name="args">查询参数 / Query arguments</param>
        /// <returns>实体列表和总数元组 / Tuple of entity list and total count</returns>
        (IEnumerable<T>, int) FuzzyQueryWithCount(string queryString, params object[] args);

        /// <summary>
        /// 更新实体。
        /// Updates an entity.
        /// </summary>
        /// <param name="entity">要更新的实体 / The entity to update</param>
        void Update(T entity);

        /// <summary>
        /// 根据ID删除实体。
        /// Deletes an entity by ID.
        /// </summary>
        /// <param name="id">实体ID / Entity ID</param>
        void Delete(int id);
    }
}
