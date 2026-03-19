using CatFoodManager.Core.Interfaces;
using CatFoodManager.Core.Models;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace CatfoodManagement.Services.Bridge
{
    public class BrandApi
    {
        private readonly IServiceProvider _serviceProvider;
        private static readonly JsonSerializerSettings _jsonSettings = new()
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            NullValueHandling = NullValueHandling.Ignore
        };

        public BrandApi(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task<string> GetBrands(string? searchKey = null)
        {
            using var scope = _serviceProvider.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<IService<Brand>>();

            IEnumerable<Brand> results;
            int count;
            
            if (string.IsNullOrEmpty(searchKey))
            {
                (results, count) = service.GetAllWithCount();
            }
            else
            {
                (results, count) = service.FuzzyQueryWithCount(BuildSearchQuery(searchKey));
            }

            return await Task.FromResult(JsonConvert.SerializeObject(new
            {
                Data = results.ToList(),
                Total = count
            }, _jsonSettings));
        }

        public async Task<string> AddBrand(string name)
        {
            using var scope = _serviceProvider.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<IService<Brand>>();

            var brand = new Brand { Name = name };
            service.Save(brand);

            return await Task.FromResult(JsonConvert.SerializeObject(new { Success = true, Id = brand.Id }, _jsonSettings));
        }

        public async Task<string> UpdateBrand(long id, string name)
        {
            using var scope = _serviceProvider.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<IService<Brand>>();

            var brand = service.Query(id);
            if (brand != null)
            {
                brand.Name = name;
                service.Update(brand);
            }

            return await Task.FromResult(JsonConvert.SerializeObject(new { Success = true }, _jsonSettings));
        }

        public async Task<string> DeleteBrand(long id)
        {
            using var scope = _serviceProvider.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<IService<Brand>>();
            service.Delete((int)id);
            return await Task.FromResult(JsonConvert.SerializeObject(new { Success = true }, _jsonSettings));
        }

        private string BuildSearchQuery(string searchKey)
        {
            return $"SELECT * FROM Brand WHERE Id LIKE '%{(long.TryParse(searchKey, out long id) ? id : 0)}%' or Name like '%{searchKey}%'";
        }
    }
}
