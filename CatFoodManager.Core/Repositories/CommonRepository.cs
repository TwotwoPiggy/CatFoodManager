using CatFoodManager.Core.Interfaces;
using CommonTools;
using System.Drawing;
using System.Linq.Expressions;

namespace CatFoodManager.Core.Repositories
{
	public class CommonRepository : RepositoryBase, IRepository
	{
        public CommonRepository(SQLiteHelper sqliteHelper):base(sqliteHelper) { }
        public void Add<T>(T entity) => _sqliteHelper.Db.Insert(entity);

		public void Update<T>(T entity) where T : new() => _sqliteHelper.Db.Update(entity);

		public T Query<T>(Expression<Func<T, bool>> predExpr) where T : new()
			=> _sqliteHelper.Db.Table<T>().FirstOrDefault(predExpr);

		public IEnumerable<T> QueryList<T>(Expression<Func<T, bool>>? predExpr = null) where T : new()
			=> predExpr == null ? _sqliteHelper.Db.Table<T>().AsQueryable() : _sqliteHelper.Db.Table<T>().Where(predExpr);

		public IEnumerable<T> FuzzyQuery<T>(string query) where T : new()
			=> _sqliteHelper.Db.Query<T>(query);

		public void Delete<T>(object key) => _sqliteHelper.Db.Delete<T>(key);
	}
}
