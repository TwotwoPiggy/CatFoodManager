using CatFoodManager.Core.Interfaces;
using CatFoodManager.Core.Models;
using CatFoodManager.Core.Statics;

namespace CatFoodManager.Core.Services
{
	public class FactoryService : GenericServiceBase<Factory>, IService<Factory>
	{
		public FactoryService(IRepository repo, bool needMigrate) : base(repo, needMigrate) { }
		public void Save(Factory factory)
		{
			_repo.Add(factory);
		}

		public Factory Query(int id)
		{
			return _repo.Query<Factory>(factory => factory.Id == id);
		}

		public IEnumerable<Factory> GetAll()
		{
			return _repo.QueryList<Factory>();
		}

		public IEnumerable<Factory> FuzzyQuery(string queryString)
		{
			return _repo.FuzzyQuery<Factory>(queryString);
		}

		public void Update(Factory factory)
		{
			_repo.Update(factory);
		}

		public void Delete(int id)
		{
			_repo.Delete<Factory>(id);
		}
	}
}
