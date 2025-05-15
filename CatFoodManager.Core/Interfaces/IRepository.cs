using CatFoodManager.Core.Repositories;
using System.Linq.Expressions;

namespace CatFoodManager.Core.Interfaces
{
	public interface IRepository : IRepositoryBase
	{
		void Add<T>(T entity);
		void BatchAdd<T>(IEnumerable<T> entities);
		public T? Query<T>(Expression<Func<T, bool>> predExpr, bool loadChildren = false, bool recursive = false) where T : new();
		void Update<T>(T entity) where T : new();
		IEnumerable<T> QueryList<T>(Expression<Func<T, bool>>? predExpr = null, bool loadChildren = false, bool recursive = false) where T : new();
		IEnumerable<T> FuzzyQuery<T>(string query, bool loadChilden = false, bool recursive = false) where T : new();
		IEnumerable<T> FuzzyQueryWithChildren<T>(string query, bool loadChilden = false, bool recursive = false, params object[] parameters) where T : new();
		void Delete<T>(object key);
	}
}
