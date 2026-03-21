using CatFoodManager.Domain.Entities;
using CatFoodManager.Infrastructure.Persistence;

namespace CatFoodManager.Infrastructure.Services
{
    public interface IDatabaseInitializer
    {
        Task InitializeAsync();
    }

    public class DatabaseInitializer : IDatabaseInitializer
    {
        private readonly IDbContext _dbContext;

        public DatabaseInitializer(IDbContext dbContext)
        {
            _dbContext = dbContext;
        }

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
