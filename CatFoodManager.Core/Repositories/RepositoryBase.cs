using CatFoodManager.Core.Interfaces;
using CommonTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CatFoodManager.Core.Repositories
{
    public abstract class RepositoryBase : IRepositoryBase
	{
        protected readonly SQLiteHelper _sqliteHelper;
		public RepositoryBase(SQLiteHelper sqliteHelper) => _sqliteHelper = sqliteHelper;

		public void Migrate<T>()
        {
            _sqliteHelper.Db.CreateTable<T>();
        }

        public void Dispose()
        {
            _sqliteHelper.Disconnect();
        }
    }
}
