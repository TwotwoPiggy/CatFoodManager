using CatFoodManager.Application.Interfaces;
using CatFoodManager.Domain.Entities;
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
            var service = scope.ServiceProvider.GetRequiredService<ICatFoodService>();

            var pagedResult = await service.GetPagedAsync(page, pageSize);

            return JsonConvert.SerializeObject(new
            {
                Data = pagedResult.Items,
                Total = pagedResult.TotalCount,
                Page = page,
                PageSize = pageSize
            }, _jsonSettings);
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
            var service = scope.ServiceProvider.GetRequiredService<ICatFoodService>();

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
        /// 删除猫粮
        /// </summary>
        /// <param name="id">猫粮 ID</param>
        /// <returns>JSON 格式的操作结果</returns>
        public async Task<string> DeleteCatFood(long id)
        {
            using var scope = _serviceProvider.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<ICatFoodService>();
            await service.DeleteAsync(id);
            return JsonConvert.SerializeObject(new { Success = true }, _jsonSettings);
        }

        /// <summary>
        /// 查看图片（使用系统默认图片查看器）
        /// </summary>
        /// <param name="picturePath">图片路径</param>
        /// <returns>JSON 格式的操作结果</returns>
        public async Task<string> ViewImageAsync(string picturePath)
        {
            return await Task.Run(() =>
            {
                if (!File.Exists(picturePath))
                {
                    return JsonConvert.SerializeObject(new { Success = false, Message = "图片文件不存在" }, _jsonSettings);
                }

                try
                {
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = picturePath,
                        UseShellExecute = true
                    });
                    return JsonConvert.SerializeObject(new { Success = true }, _jsonSettings);
                }
                catch (Exception ex)
                {
                    return JsonConvert.SerializeObject(new { Success = false, Message = ex.Message }, _jsonSettings);
                }
            });
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
            var service = scope.ServiceProvider.GetRequiredService<ICatFoodService>();

            var entity = await service.GetByIdAsync(id);
            if (entity != null)
            {
                entity.PicturePath = imagePath;
                entity.UpdatedAt = DateTime.Now;
                await service.UpdateAsync(entity);
            }

            return JsonConvert.SerializeObject(new { Success = true }, _jsonSettings);
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

            if (underlyingType == typeof(double))
            {
                return Convert.ToDouble(value);
            }

            return Convert.ChangeType(value, underlyingType);
        }
    }
}
