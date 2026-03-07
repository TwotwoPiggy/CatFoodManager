using CatFoodManager.Core.Interfaces;
using CatFoodManager.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace CatFoodManager.Core.Services
{
    public class LowestPriceService: GenericServiceBase<BestPrice>, IService<BestPrice>
    {
        public LowestPriceService(IRepository repo, bool needMigrate): base(repo, needMigrate) { } 
        
        public void Save(BestPrice bestPrice)
        {
            ArgumentNullException.ThrowIfNull(bestPrice);
            _repo.Add(bestPrice);
        }

        public void BatchSave(IEnumerable<BestPrice> bestPrices)
        {
            ArgumentNullException.ThrowIfNull(bestPrices);
            var list = bestPrices as IList<BestPrice> ?? [.. bestPrices];
            if (list.Count == 0) return;
            _repo.BatchAdd(list);
        }


        public BestPrice? Query(long id) => _repo.Query<BestPrice>(price => price.Id == id, true);

        public BestPrice? Query(string catFoodName)
        {
            if (string.IsNullOrWhiteSpace(catFoodName)) return null;
            return _repo.Query<BestPrice>(price => price.Name == catFoodName, true);
        }


        public IEnumerable<BestPrice> GetAll() => _repo.QueryList<BestPrice>(loadChildren: true);

        public (IEnumerable<BestPrice>, int) GetAllWithCount()
        {
            var list = _repo.QueryList<BestPrice>(loadChildren: true).ToList();
            return (list, list.Count);
        }

        public IEnumerable<BestPrice> FuzzyQuery(string queryString, params object[] args) => _repo.FuzzyQueryWithChildren<BestPrice>(queryString, true, false, args);

        public (IEnumerable<BestPrice>, int) FuzzyQueryWithCount(string queryString, params object[] args)
        {
            var list = _repo.FuzzyQueryWithChildren<BestPrice>(queryString, true, false, args).ToList();
            return (list, list.Count);
        }

        public void Update(BestPrice price)
        {
            ArgumentNullException.ThrowIfNull(price);
            _repo.Update(price);
        }

        public void Delete(int id) => _repo.Delete<BestPrice>(id);

    }
}
