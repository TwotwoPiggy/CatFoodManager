using CatFoodManager.Core.Interfaces;
using CatFoodManager.Core.Statics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CatFoodManager.Core.Services
{
	public class GenericServiceBase<T> : ServiceBase, IServiceBase
	{
		public GenericServiceBase(IRepository repo, bool needMigrate) : base(repo)
		{
			if (needMigrate)
				Migrate<T>();
		}

		public void Migrate<U>()
		{
			_repo.Migrate<U>();
		}
	}
}
