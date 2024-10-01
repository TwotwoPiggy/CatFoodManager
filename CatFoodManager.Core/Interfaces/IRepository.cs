using CatFoodManager.Core.Repositories;
using System.Linq.Expressions;

namespace CatFoodManager.Core.Interfaces
{
	public interface IRepository : IRepositoryBase
	{
		void Add<T>(T entity);
		T Query<T>(Expression<Func<T, bool>> predExpr) where T : new();
		void Update<T>(T entity) where T : new();
		IEnumerable<T> QueryList<T>(Expression<Func<T, bool>>? predExpr = null) where T : new();
		IEnumerable<T> FuzzyQuery<T>(string  query) where T : new();
		void Delete<T>(object key);
	}
}
