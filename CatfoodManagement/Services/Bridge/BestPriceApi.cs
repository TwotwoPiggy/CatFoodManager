using CatFoodManager.Core.Interfaces;
using CatFoodManager.Core.Models;
using CatFoodManager.Core.Statics;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace CatfoodManagement.Services.Bridge
{
    /// <summary>
    /// 最佳价格管理 API，提供价格记录的增删改查操作
    /// 通过 CefSharp 暴露给前端 JavaScript 调用
    /// </summary>
    public class BestPriceApi
    {
        /// <summary>
        /// 服务提供者，用于创建作用域并获取服务
        /// </summary>
        private readonly IServiceProvider _serviceProvider;
        
        /// <summary>
        /// JSON 序列化设置，忽略循环引用和空值
        /// </summary>
        private static readonly JsonSerializerSettings _jsonSettings = new()
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            NullValueHandling = NullValueHandling.Ignore
        };

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="serviceProvider">服务提供者</param>
        public BestPriceApi(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        /// <summary>
        /// 获取最佳价格列表（支持分页和搜索）
        /// </summary>
        /// <param name="page">页码（从 1 开始）</param>
        /// <param name="pageSize">每页数量</param>
        /// <param name="searchKey">搜索关键词（可选）</param>
        /// <returns>JSON 格式的价格列表数据</returns>
        public async Task<string> GetBestPrices(int page, int pageSize, string? searchKey = null)
        {
            using var scope = _serviceProvider.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<IService<BestPrice>>();

            IEnumerable<BestPrice> results;
            int count;
            
            // 根据是否有搜索关键词选择查询方式
            if (string.IsNullOrEmpty(searchKey))
            {
                (results, count) = service.GetAllWithCount();
            }
            else
            {
                (results, count) = service.FuzzyQueryWithCount(BuildSearchQuery(searchKey), BuildSearchArgs(searchKey));
            }

            // 分页处理
            var pagedResults = results.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            return await Task.FromResult(JsonConvert.SerializeObject(new
            {
                Data = pagedResults,
                Total = count,
                Page = page,
                PageSize = pageSize
            }, _jsonSettings));
        }

        /// <summary>
        /// 添加新的价格记录
        /// </summary>
        /// <param name="dto">价格数据传输对象</param>
        /// <returns>JSON 格式的操作结果，包含新记录的 ID</returns>
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

        /// <summary>
        /// 更新价格记录
        /// </summary>
        /// <param name="id">价格记录 ID</param>
        /// <param name="field">要更新的字段名</param>
        /// <param name="value">新值</param>
        /// <returns>JSON 格式的操作结果</returns>
        public async Task<string> UpdateBestPrice(long id, string field, object value)
        {
            using var scope = _serviceProvider.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<IService<BestPrice>>();

            var entity = service.Query(id);
            if (entity == null)
            {
                return JsonConvert.SerializeObject(new { Success = false, Message = "Entity not found" }, _jsonSettings);
            }

            // 通过反射更新指定字段
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

        /// <summary>
        /// 删除价格记录
        /// </summary>
        /// <param name="id">价格记录 ID</param>
        /// <returns>JSON 格式的操作结果</returns>
        public async Task<string> DeleteBestPrice(long id)
        {
            using var scope = _serviceProvider.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<IService<BestPrice>>();
            service.Delete((int)id);
            return await Task.FromResult(JsonConvert.SerializeObject(new { Success = true }, _jsonSettings));
        }

        /// <summary>
        /// 构建搜索 SQL 查询语句
        /// </summary>
        /// <param name="searchKey">搜索关键词</param>
        /// <returns>SQL 查询语句</returns>
        private string BuildSearchQuery(string searchKey)
        {
            return "SELECT DISTINCT a.*\r\nFROM BestPrice a\r\nWHERE a.Name like ?";
        }

        /// <summary>
        /// 构建搜索参数
        /// </summary>
        /// <param name="searchKey">搜索关键词</param>
        /// <returns>参数数组</returns>
        private object[] BuildSearchArgs(string searchKey)
        {
            return new object[] { $"%{searchKey}%" };
        }

        /// <summary>
        /// 将值转换为目标类型
        /// </summary>
        /// <param name="value">原始值</param>
        /// <param name="targetType">目标类型</param>
        /// <returns>转换后的值</returns>
        private static object ConvertValue(object value, Type targetType)
        {
            // 处理枚举类型
            if (targetType.IsEnum)
            {
                return Enum.Parse(targetType, value?.ToString() ?? "0");
            }
            
            // 处理 int 到 decimal 的转换
            if (targetType == typeof(decimal) && value is int intValue)
            {
                return (decimal)intValue;
            }

            return Convert.ChangeType(value, targetType);
        }
    }
}
