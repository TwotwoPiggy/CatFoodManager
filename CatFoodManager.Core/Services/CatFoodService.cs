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

		public void BatchSave(IEnumerable<CatFood> catFoods)
		{
			_repo.BatchAdd(catFoods);
		}

		public CatFood Query(long id)
		{
			return _repo.Query<CatFood>(catFood => catFood.Id == id);
		}

		public CatFood Query(string catFoodName)
		{
			return _repo.Query<CatFood>(catFood => catFood.Name == catFoodName);
		}

		public IEnumerable<CatFood> GetAll()
		{
			return _repo.QueryList<CatFood>();
		}

		public (IEnumerable<CatFood>, int) GetAllWithCount()
		{
			var list = GetAll();
			return (list, list.Count());
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
