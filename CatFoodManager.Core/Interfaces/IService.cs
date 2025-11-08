namespace CatFoodManager.Core.Interfaces
{
	public interface IService<T>
	{
		void Save(T entity);
		void BatchSave(IEnumerable<T> entities);
		T? Query(long id);
		T? Query(string name);
		IEnumerable<T> GetAll();
		(IEnumerable<T>, int) GetAllWithCount();
		IEnumerable<T> FuzzyQuery(string queryString);
		(IEnumerable<T>, int) FuzzyQueryWithCount(string queryString);
		void Update(T brand);
		void Delete(int id);
	}
}
