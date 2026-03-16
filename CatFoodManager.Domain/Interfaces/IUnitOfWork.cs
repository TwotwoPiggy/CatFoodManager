namespace CatFoodManager.Domain.Interfaces;

public interface IUnitOfWork : IDisposable
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    IRepository<T> Repository<T>() where T : class, IEntity, new();
}
