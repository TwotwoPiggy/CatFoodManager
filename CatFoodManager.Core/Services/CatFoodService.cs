using CatFoodManager.Core.Interfaces;
using CatFoodManager.Core.Models;
using CatFoodManager.Core.Statics;

namespace CatFoodManager.Core.Services
{
	public class CatFoodService : GenericServiceBase<CatFood>, IService<CatFood>
	{
        public CatFoodService(IRepository repo, bool needMigrate) : base(repo, needMigrate) { }
		public void Save(CatFood catFood)
		{
			_repo.Add(catFood);
		}

		public CatFood Query(int id)
		{
			return _repo.Query<CatFood>(catFood => catFood.Id == id);
		}

		public IEnumerable<CatFood> GetAll()
		{
			return _repo.QueryList<CatFood>();
		}

		public IEnumerable<CatFood> FuzzyQuery(string queryString)
		{
			return _repo.FuzzyQuery<CatFood>(queryString);
		}

		public void Update(CatFood catFood)
		{
			_repo.Update(catFood);
		}

		public void Delete(int id)
		{
			_repo.Delete<CatFood>(id);
		}
	}
}
