using CatFoodManager.Core.Interfaces;
using CatFoodManager.Core.Models;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace CatfoodManagement.Services.Bridge
{
    public class CatFoodApi
    {
        private readonly IServiceProvider _serviceProvider;
        private static readonly JsonSerializerSettings _jsonSettings = new()
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            NullValueHandling = NullValueHandling.Ignore
        };

        public CatFoodApi(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task<string> GetCatFoods(int page, int pageSize, string? searchKey = null)
        {
            using var scope = _serviceProvider.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<IService<CatFood>>();

            IEnumerable<CatFood> results;
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

        public async Task<string> UpdateCatFood(long id, string field, object value)
        {
            using var scope = _serviceProvider.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<IService<CatFood>>();

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

        public async Task<string> DeleteCatFood(long id)
        {
            using var scope = _serviceProvider.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<IService<CatFood>>();
            service.Delete((int)id);
            return await Task.FromResult(JsonConvert.SerializeObject(new { Success = true }, _jsonSettings));
        }

        public void ViewImage(string picturePath)
        {
            if (File.Exists(picturePath))
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = picturePath,
                    UseShellExecute = true
                });
            }
        }

        public async Task<string> UploadImage(long id, string imagePath)
        {
            using var scope = _serviceProvider.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<IService<CatFood>>();

            var entity = service.Query(id);
            if (entity != null)
            {
                entity.PicturePath = imagePath;
                entity.UpdatedAt = DateTime.Now;
                service.Update(entity);
            }

            return await Task.FromResult(JsonConvert.SerializeObject(new { Success = true }, _jsonSettings));
        }

        private string BuildSearchQuery(string searchKey)
        {
            var baseQuery = "SELECT DISTINCT a.*\r\nFROM CatFood a \r\nLEFT JOIN Brand b ON a.BrandId = b.Id \r\nWHERE b.Name like";
            return $"{baseQuery} ?";
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
            
            if (targetType == typeof(double) && value is int intValue)
            {
                return (double)intValue;
            }

            return Convert.ChangeType(value, targetType);
        }
    }
}
