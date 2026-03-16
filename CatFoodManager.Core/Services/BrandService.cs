using CatFoodManager.Core.Interfaces;
using CatFoodManager.Core.Models;

namespace CatFoodManager.Core.Services
{
    public class BrandService : GenericServiceBase<Brand>, IService<Brand>
    {
        public BrandService(IRepository repo, bool needMigrate) : base(repo, needMigrate) { }

        public void Save(Brand brand)
        {
            if (brand is null) throw new ArgumentNullException(nameof(brand));
            _repo.Add(brand);
        }

        public void BatchSave(IEnumerable<Brand> brands)
        {
            if (brands is null) throw new ArgumentNullException(nameof(brands));
            var list = brands as IList<Brand> ?? brands.ToList();
            if (list.Count == 0) return;
            _repo.BatchAdd(list);
        }

        public Brand? Query(long id) => _repo.Query<Brand>(brand => brand.Id == id);

        public Brand? Query(string brandName)
        {
            if (string.IsNullOrWhiteSpace(brandName)) return null;
            return _repo.Query<Brand>(brand => brand.Name == brandName);
        }

        public IEnumerable<Brand> GetAll() => _repo.QueryList<Brand>();

        public (IEnumerable<Brand>, int) GetAllWithCount()
        {
            var list = _repo.QueryList<Brand>().ToList();
            return (list, list.Count);
        }

        public IEnumerable<Brand> FuzzyQuery(string queryString, params object[] args) => _repo.FuzzyQueryWithChildren<Brand>(queryString, false, false, args);

        public (IEnumerable<Brand>, int) FuzzyQueryWithCount(string queryString, params object[] args)
        {
            var list = _repo.FuzzyQueryWithChildren<Brand>(queryString, false, false, args).ToList();
            return (list, list.Count);
        }

        public void Update(Brand brand)
        {
            if (brand is null) throw new ArgumentNullException(nameof(brand));
            _repo.Update(brand);
        }

        public void Delete(int id) => _repo.Delete<Brand>(id);
    }
}
