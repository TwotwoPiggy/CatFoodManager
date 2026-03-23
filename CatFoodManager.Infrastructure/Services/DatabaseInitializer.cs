using CatFoodManager.Domain.Entities;
using CatFoodManager.Infrastructure.Persistence;

namespace CatFoodManager.Infrastructure.Services
{
    /// <summary>
    /// 数据库初始化器接口，提供数据库初始化功能。
    /// Database initializer interface, providing database initialization functionality.
    /// </summary>
    public interface IDatabaseInitializer
    {
        /// <summary>
        /// 初始化数据库。
        /// Initializes the database.
        /// </summary>
        Task InitializeAsync();
    }

    /// <summary>
    /// 数据库初始化器，创建必要的数据表。
    /// Database initializer, creating necessary data tables.
    /// </summary>
    public class DatabaseInitializer : IDatabaseInitializer
    {
        private readonly IDbContext _dbContext;

        /// <summary>
        /// 构造函数。
        /// Constructor.
        /// </summary>
        /// <param name="dbContext">数据库上下文 / Database context</param>
        public DatabaseInitializer(IDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// 初始化数据库，创建所有必要的数据表。
        /// Initializes the database, creating all necessary data tables.
        /// </summary>
        public async Task InitializeAsync()
        {
            await _dbContext.CreateTableAsync<CatFood>();
            await _dbContext.CreateTableAsync<Brand>();
            await _dbContext.CreateTableAsync<Factory>();
            await _dbContext.CreateTableAsync<BestPrice>();
            await _dbContext.CreateTableAsync<PlatformRegExp>();
            await _dbContext.CreateTableAsync<OcrPrompt>();
            await _dbContext.CreateTableAsync<TaskItem>();
            await _dbContext.CreateTableAsync<TaskConfiguration>();
        }
    }
}
