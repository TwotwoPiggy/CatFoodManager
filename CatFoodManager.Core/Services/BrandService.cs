using CatFoodManager.Core.Interfaces;
using CatFoodManager.Core.Models;
using CatFoodManager.Core.Statics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.Design;

namespace CatFoodManager.Core.Services
{
	public class BrandService : GenericServiceBase<Brand>, IService<Brand>
	{
		public BrandService(IRepository repo, bool needMigrate) : base(repo, needMigrate) { }
		public void Save(Brand brand)
		{
			_repo.Add(brand);
		}

		public Brand Query(int id)
		{
			return _repo.Query<Brand>(brand => brand.Id == id);
		}

		public IEnumerable<Brand> GetAll()
		{
			return _repo.QueryList<Brand>();
		}

		public IEnumerable<Brand> FuzzyQuery(string queryString)
		{
			return _repo.FuzzyQuery<Brand>(queryString);
		}

		public void Update(Brand brand)
		{
			_repo.Update(brand);
		}

		public void Delete(int id)
		{
			_repo.Delete<Brand>(id);
		}
	}
}
