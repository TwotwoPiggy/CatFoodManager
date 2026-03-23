namespace CatFoodManager.Core.Interfaces
{
    public interface IRepositoryBase
    {
        void Migrate<T>();
    }
}
