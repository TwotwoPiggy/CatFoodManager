using CatFoodManager.Application.Interfaces;
using CatFoodManager.Domain.Entities;
using CatFoodManager.Domain.Enums;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace CatfoodManagement.Services.Bridge
{
    /// <summary>
    /// 最佳价格管理API，提供价格记录的增删改查操作
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
            var service = scope.ServiceProvider.GetRequiredService<IBestPriceService>();

            var pagedResult = await service.GetPagedAsync(page, pageSize, searchKey);

            return JsonConvert.SerializeObject(new
            {
                Data = pagedResult.Items,
                Total = pagedResult.TotalCount,
                Page = page,
                PageSize = pageSize
            }, _jsonSettings);
        }

        /// <summary>
        /// 添加新的价格记录
        /// </summary>
        /// <param name="dto">价格数据传输对象</param>
        /// <returns>JSON 格式的操作结果，包含新记录的 ID</returns>
        public async Task<string> AddBestPrice(dynamic dto)
        {
            using var scope = _serviceProvider.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<IBestPriceService>();

            var bestPrice = new BestPrice
            {
                Name = dto.Name,
                Type = (ProductType)dto.Type,
                Platform = (PlatformType)dto.Platform,
                LowestPrice = dto.LowestPrice,
                FinalPrice = dto.FinalPrice,
                PurchasedAt = dto.PurchasedAt,
                PicturePath = dto.PicturePath,
                HasPurchased = dto.HasPurchased,
                TestingAgency = dto.TestingAgency
            };

            await service.AddAsync(bestPrice);

            return JsonConvert.SerializeObject(new { Success = true, Id = bestPrice.Id }, _jsonSettings);
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
            var service = scope.ServiceProvider.GetRequiredService<IBestPriceService>();

            var entity = await service.GetByIdAsync(id);
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
                await service.UpdateAsync(entity);
            }

            return JsonConvert.SerializeObject(new { Success = true }, _jsonSettings);
        }

        /// <summary>
        /// 删除价格记录
        /// </summary>
        /// <param name="id">价格记录 ID</param>
        /// <returns>JSON 格式的操作结果</returns>
        public async Task<string> DeleteBestPrice(long id)
        {
            using var scope = _serviceProvider.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<IBestPriceService>();
            await service.DeleteAsync(id);
            return JsonConvert.SerializeObject(new { Success = true }, _jsonSettings);
        }

        /// <summary>
        /// 批量删除价格记录
        /// </summary>
        /// <param name="ids">价格记录ID数组</param>
        /// <returns>JSON格式的操作结果，包含删除数量</returns>
        public async Task<string> DeleteBestPrices(long[] ids)
        {
            using var scope = _serviceProvider.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<IBestPriceService>();
            var count = await service.DeleteRangeAsync(ids);
            return JsonConvert.SerializeObject(new { Success = true, Count = count }, _jsonSettings);
        }

        /// <summary>
        /// 上传图片到指定路径并更新记录
        /// </summary>
        /// <param name="id">价格记录 ID</param>
        /// <param name="imagePath">源图片路径</param>
        /// <param name="targetPath">目标保存路径</param>
        /// <returns>JSON 格式的操作结果，包含新的图片路径</returns>
        public async Task<string> UploadImageAsync(long id, string imagePath, string targetPath)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var service = scope.ServiceProvider.GetRequiredService<IBestPriceService>();

                var entity = await service.GetByIdAsync(id);
                if (entity == null)
                {
                    return JsonConvert.SerializeObject(new { Success = false, Message = "记录不存在" }, _jsonSettings);
                }

                if (string.IsNullOrEmpty(imagePath) || !File.Exists(imagePath))
                {
                    return JsonConvert.SerializeObject(new { Success = false, Message = "图片文件不存在" }, _jsonSettings);
                }

                if (string.IsNullOrEmpty(targetPath))
                {
                    return JsonConvert.SerializeObject(new { Success = false, Message = "目标路径不能为空" }, _jsonSettings);
                }

                if (!Directory.Exists(targetPath))
                {
                    Directory.CreateDirectory(targetPath);
                }

                var fileName = Path.GetFileNameWithoutExtension(imagePath);
                var extension = Path.GetExtension(imagePath);
                var timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
                var newFileName = $"{fileName}_{timestamp}{extension}";
                var targetFilePath = Path.Combine(targetPath, newFileName);

                if (!string.IsNullOrEmpty(entity.PicturePath) && File.Exists(entity.PicturePath))
                {
                    try
                    {
                        File.Delete(entity.PicturePath);
                    }
                    catch
                    {
                        // 忽略删除失败，不影响主流程
                    }
                }

                File.Move(imagePath, targetFilePath, overwrite: true);

                entity.PicturePath = targetFilePath;
                entity.UpdatedAt = DateTime.Now;
                await service.UpdateAsync(entity);

                return JsonConvert.SerializeObject(new 
                { 
                    Success = true, 
                    Data = new { PicturePath = targetFilePath } 
                }, _jsonSettings);
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(new { Success = false, Message = ex.Message }, _jsonSettings);
            }
        }

        /// <summary>
        /// 将值转换为目标类型
        /// </summary>
        /// <param name="value">原始值</param>
        /// <param name="targetType">目标类型</param>
        /// <returns>转换后的值</returns>
        private static object ConvertValue(object value, Type targetType)
        {
            if (value == null)
            {
                return targetType.IsValueType && Nullable.GetUnderlyingType(targetType) == null
                    ? Activator.CreateInstance(targetType)
                    : null;
            }

            var underlyingType = Nullable.GetUnderlyingType(targetType) ?? targetType;

            if (underlyingType.IsEnum)
            {
                return Enum.Parse(underlyingType, value.ToString() ?? "0");
            }

            if (underlyingType == typeof(decimal))
            {
                return Convert.ToDecimal(value);
            }

            if (underlyingType == typeof(DateTime))
            {
                return DateTime.Parse(value.ToString()!);
            }

            if (underlyingType == typeof(DateTimeOffset))
            {
                return DateTimeOffset.Parse(value.ToString()!);
            }

            return Convert.ChangeType(value, underlyingType);
        }
    }
}
