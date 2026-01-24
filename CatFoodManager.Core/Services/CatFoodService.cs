using System;
using System.Collections.Generic;
using System.Linq;
using CatFoodManager.Core.Interfaces;
using CatFoodManager.Core.Models;

namespace CatFoodManager.Core.Services
{
    public class CatFoodService : GenericServiceBase<CatFood>, IService<CatFood>
    {
        public CatFoodService(IRepository repo, bool needMigrate) : base(repo, needMigrate) { }

        public void Save(CatFood catFood)
        {
            ArgumentNullException.ThrowIfNull(catFood);
            _repo.Add(catFood);
        }

        public void BatchSave(IEnumerable<CatFood> catFoods)
        {
            ArgumentNullException.ThrowIfNull(catFoods);
            var list = catFoods as IList<CatFood> ?? [.. catFoods];
            if (list.Count == 0) return;
            _repo.BatchAdd(list);
        }

        public CatFood? Query(long id) => _repo.Query<CatFood>(catFood => catFood.Id == id, true);

        public CatFood? Query(string catFoodName)
        {
            if (string.IsNullOrWhiteSpace(catFoodName)) return null;
            return _repo.Query<CatFood>(catFood => catFood.Name == catFoodName, true);
        }

        public IEnumerable<CatFood> GetAll() => _repo.QueryList<CatFood>(loadChildren: true);

        public (IEnumerable<CatFood>, int) GetAllWithCount()
        {
            var list = _repo.QueryList<CatFood>(loadChildren: true).ToList();
            return (list, list.Count);
        }

        public IEnumerable<CatFood> FuzzyQuery(string queryString, params object[] args) => _repo.FuzzyQueryWithChildren<CatFood>(queryString, true, false, args);

        public (IEnumerable<CatFood>, int) FuzzyQueryWithCount(string queryString, params object[] args)
        {
            var list = _repo.FuzzyQueryWithChildren<CatFood>(queryString, true, false, args).ToList();
            return (list, list.Count);
        }

        public void Update(CatFood catFood)
        {
            if (catFood is null) throw new ArgumentNullException(nameof(catFood));
            _repo.Update(catFood);
        }

        public void Delete(int id) => _repo.Delete<CatFood>(id);
    }
}
