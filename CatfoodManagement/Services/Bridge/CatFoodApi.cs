using CatFoodManager.Core.Interfaces;
using CatFoodManager.Core.Models;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace CatfoodManagement.Services.Bridge
{
    /// <summary>
    /// 猫粮管理 API，提供猫粮数据的增删改查操作
    /// 通过 CefSharp 暴露给前端 JavaScript 调用
    /// </summary>
    public class CatFoodApi
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
        public CatFoodApi(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        /// <summary>
        /// 获取猫粮列表（支持分页和搜索）
        /// </summary>
        /// <param name="page">页码（从 1 开始）</param>
        /// <param name="pageSize">每页数量</param>
        /// <param name="searchKey">搜索关键词（可选）</param>
        /// <returns>JSON 格式的猫粮列表数据</returns>
        public async Task<string> GetCatFoods(int page, int pageSize, string? searchKey = null)
        {
            using var scope = _serviceProvider.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<IService<CatFood>>();

            IEnumerable<CatFood> results;
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
        /// 更新猫粮信息
        /// </summary>
        /// <param name="id">猫粮 ID</param>
        /// <param name="field">要更新的字段名</param>
        /// <param name="value">新值</param>
        /// <returns>JSON 格式的操作结果</returns>
        public async Task<string> UpdateCatFood(long id, string field, object value)
        {
            using var scope = _serviceProvider.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<IService<CatFood>>();

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
        /// 删除猫粮
        /// </summary>
        /// <param name="id">猫粮 ID</param>
        /// <returns>JSON 格式的操作结果</returns>
        public async Task<string> DeleteCatFood(long id)
        {
            using var scope = _serviceProvider.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<IService<CatFood>>();
            service.Delete((int)id);
            return await Task.FromResult(JsonConvert.SerializeObject(new { Success = true }, _jsonSettings));
        }

        /// <summary>
        /// 查看图片（使用系统默认图片查看器）
        /// </summary>
        /// <param name="picturePath">图片路径</param>
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

        /// <summary>
        /// 上传图片并更新猫粮记录
        /// </summary>
        /// <param name="id">猫粮 ID</param>
        /// <param name="imagePath">图片路径</param>
        /// <returns>JSON 格式的操作结果</returns>
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

        /// <summary>
        /// 构建搜索 SQL 查询语句
        /// </summary>
        /// <param name="searchKey">搜索关键词</param>
        /// <returns>SQL 查询语句</returns>
        private string BuildSearchQuery(string searchKey)
        {
            var baseQuery = "SELECT DISTINCT a.*\r\nFROM CatFood a \r\nLEFT JOIN Brand b ON a.BrandId = b.Id \r\nWHERE b.Name like";
            return $"{baseQuery} ?";
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
            
            // 处理 int 到 double 的转换
            if (targetType == typeof(double) && value is int intValue)
            {
                return (double)intValue;
            }

            return Convert.ChangeType(value, targetType);
        }
    }
}
