using CatFoodManager.Core.Interfaces;
using CatFoodManager.Core.Repositories;
using CatFoodManager.Core.Statics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CatFoodManager.Core.Services
{
	public class ServiceBase
	{
		protected readonly IRepository _repo;
        public ServiceBase(IRepository repo)
		{
			_repo = repo;
		}

	}
}
