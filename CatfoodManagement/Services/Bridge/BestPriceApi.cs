using CatFoodManager.Core.Interfaces;
using CatFoodManager.Core.Models;
using CatFoodManager.Core.Statics;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace CatfoodManagement.Services.Bridge
{
    public class BestPriceApi
    {
        private readonly IServiceProvider _serviceProvider;
        private static readonly JsonSerializerSettings _jsonSettings = new()
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            NullValueHandling = NullValueHandling.Ignore
        };

        public BestPriceApi(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task<string> GetBestPrices(int page, int pageSize, string? searchKey = null)
        {
            using var scope = _serviceProvider.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<IService<BestPrice>>();

            IEnumerable<BestPrice> results;
            int count;
            
            if (string.IsNullOrEmpty(searchKey))
            {
                (results, count) = service.GetAllWithCount();
            }
            else
            {
                (results, count) = service.FuzzyQueryWithCount(BuildSearchQuery(searchKey), BuildSearchArgs(searchKey));
            }

            var pagedResults = results.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            return await Task.FromResult(JsonConvert.SerializeObject(new
            {
                Data = pagedResults,
                Total = count,
                Page = page,
                PageSize = pageSize
            }, _jsonSettings));
        }

        public async Task<string> AddBestPrice(dynamic dto)
        {
            using var scope = _serviceProvider.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<IService<BestPrice>>();

            var bestPrice = new BestPrice
            {
                Name = dto.Name,
                Type = (ProductType)dto.Type,
                Platform = (PlatformType)dto.Platform,
                LowestPrice = dto.LowestPrice,
                FinalPrice = dto.FinalPrice,
                PurchasedAt = dto.PurchasedAt,
                PicturePath = dto.PicturePath,
                HasPurchased = dto.HasPurchased
            };

            service.Save(bestPrice);

            return await Task.FromResult(JsonConvert.SerializeObject(new { Success = true, Id = bestPrice.Id }, _jsonSettings));
        }

        public async Task<string> UpdateBestPrice(long id, string field, object value)
        {
            using var scope = _serviceProvider.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<IService<BestPrice>>();

            var entity = service.Query(id);
            if (entity == null)
            {
                return JsonConvert.SerializeObject(new { Success = false, Message = "Entity not found" }, _jsonSettings);
            }

            var propertyToUpdate = entity.GetType().GetProperty(field);
            if (propertyToUpdate != null)
            {
                var convertedValue = ConvertValue(value, propertyToUpdate.PropertyType);
                propertyToUpdate.SetValue(entity, convertedValue);
                entity.UpdatedAt = DateTime.Now;
                service.Update(entity);
            }

            return await Task.FromResult(JsonConvert.SerializeObject(new { Success = true }, _jsonSettings));
        }

        public async Task<string> DeleteBestPrice(long id)
        {
            using var scope = _serviceProvider.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<IService<BestPrice>>();
            service.Delete((int)id);
            return await Task.FromResult(JsonConvert.SerializeObject(new { Success = true }, _jsonSettings));
        }

        private string BuildSearchQuery(string searchKey)
        {
            return "SELECT DISTINCT a.*\r\nFROM BestPrice a\r\nWHERE a.Name like ?";
        }

        private object[] BuildSearchArgs(string searchKey)
        {
            return new object[] { $"%{searchKey}%" };
        }

        private static object ConvertValue(object value, Type targetType)
        {
            if (targetType.IsEnum)
            {
                return Enum.Parse(targetType, value?.ToString() ?? "0");
            }
            
            if (targetType == typeof(decimal) && value is int intValue)
            {
                return (decimal)intValue;
            }

            return Convert.ChangeType(value, targetType);
        }
    }
}
