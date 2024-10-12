using CatFoodManager.Core.Interfaces;
using CatFoodManager.Core.Models;
using CatFoodManager.Core.Statics;

namespace CatFoodManager.Core.Services
{
	public class PlatformRegExpService : GenericServiceBase<PlatformRegExp>, IPlatformRegExpService
	{
        public PlatformRegExpService(IRepository repo, bool needMigrate) : base(repo, needMigrate) { }

        public void Save(PlatformRegExp platformRegExp)
		{
			_repo.Add(platformRegExp);
		}

		public PlatformRegExp Query(int id)
		{
			return _repo.Query<PlatformRegExp>(platformRegExp => platformRegExp.Id == id);
		}

		public IEnumerable<PlatformRegExp> GetRegExpByPlatform(PlatformType platformType)
		{
			return _repo.QueryList<PlatformRegExp>(reg => reg.Platform == platformType);
		}

		public IEnumerable<PlatformRegExp> GetAll()
		{
			return _repo.QueryList<PlatformRegExp>();
		}

		public IEnumerable<PlatformRegExp> FuzzyQuery(string queryString)
		{
			return _repo.FuzzyQuery<PlatformRegExp>(queryString);
		}

		public void Update(PlatformRegExp platformRegExp)
		{
			_repo.Update(platformRegExp);
		}

		public void Delete(int id)
		{
			_repo.Delete<PlatformRegExp>(id);
		}
	}
}
