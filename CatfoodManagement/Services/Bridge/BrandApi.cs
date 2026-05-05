using CatFoodManager.Application.Interfaces;
using CatFoodManager.Domain.Entities;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace CatfoodManagement.Services.Bridge
{
    /// <summary>
    /// 品牌管理 API，提供品牌数据的增删改查操作
    /// 通过 CefSharp 暴露给前端 JavaScript 调用
    /// </summary>
    public class BrandApi
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
        public BrandApi(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        /// <summary>
        /// 获取品牌列表（支持搜索）
        /// </summary>
        /// <param name="searchKey">搜索关键词（可选）</param>
        /// <returns>JSON 格式的品牌列表数据</returns>
        public async Task<string> GetBrands(string? searchKey = null)
        {
            using var scope = _serviceProvider.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<IBrandService>();

            var results = await service.SearchAsync(searchKey ?? string.Empty);

            return JsonConvert.SerializeObject(new
            {
                Data = results.ToList(),
                Total = results.Count
            }, _jsonSettings);
        }

        /// <summary>
        /// 添加新品牌
        /// </summary>
        /// <param name="name">品牌名称</param>
        /// <returns>JSON 格式的操作结果，包含新品牌的 ID</returns>
        public async Task<string> AddBrand(string name)
        {
            using var scope = _serviceProvider.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<IBrandService>();

            var brand = new Brand { Name = name };
            await service.AddAsync(brand);

            return JsonConvert.SerializeObject(new { Success = true, Id = brand.Id }, _jsonSettings);
        }

        /// <summary>
        /// 更新品牌名称
        /// </summary>
        /// <param name="id">品牌 ID</param>
        /// <param name="name">新品牌名称</param>
        /// <returns>JSON 格式的操作结果</returns>
        public async Task<string> UpdateBrand(long id, string name)
        {
            using var scope = _serviceProvider.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<IBrandService>();

            var brand = await service.GetByIdAsync(id);
            if (brand != null)
            {
                brand.Name = name;
                await service.UpdateAsync(brand);
            }

            return JsonConvert.SerializeObject(new { Success = true }, _jsonSettings);
        }

        /// <summary>
        /// 删除品牌
        /// </summary>
        /// <param name="id">品牌 ID</param>
        /// <returns>JSON 格式的操作结果</returns>
        public async Task<string> DeleteBrand(long id)
        {
            using var scope = _serviceProvider.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<IBrandService>();
            await service.DeleteAsync(id);
            return JsonConvert.SerializeObject(new { Success = true }, _jsonSettings);
        }
    }
}
