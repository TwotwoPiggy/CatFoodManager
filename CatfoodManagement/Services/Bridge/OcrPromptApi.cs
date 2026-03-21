using CatFoodManager.Application.Interfaces;
using CatFoodManager.Domain.Entities;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace CatfoodManagement.Services.Bridge
{
    public class OcrPromptApi
    {
        private readonly IServiceProvider _serviceProvider;

        private static readonly JsonSerializerSettings _jsonSettings = new()
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            NullValueHandling = NullValueHandling.Ignore
        };

        public OcrPromptApi(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task<string> GetDefaultAsync()
        {
            using var scope = _serviceProvider.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<IOcrPromptService>();

            try
            {
                var prompt = await service.GetDefaultAsync();
                return JsonConvert.SerializeObject(new { Success = true, Data = prompt }, _jsonSettings);
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(new { Success = false, Message = ex.Message }, _jsonSettings);
            }
        }

        public async Task<string> GetByIdAsync(long id)
        {
            using var scope = _serviceProvider.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<IOcrPromptService>();

            try
            {
                var prompt = await service.GetByIdAsync(id);
                if (prompt == null)
                {
                    return JsonConvert.SerializeObject(new { Success = false, Message = "Prompt not found" }, _jsonSettings);
                }
                return JsonConvert.SerializeObject(new { Success = true, Data = prompt }, _jsonSettings);
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(new { Success = false, Message = ex.Message }, _jsonSettings);
            }
        }

        public async Task<string> GetAllAsync()
        {
            using var scope = _serviceProvider.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<IOcrPromptService>();

            try
            {
                var prompts = await service.GetAllAsync();
                return JsonConvert.SerializeObject(new { Success = true, Data = prompts.ToList() }, _jsonSettings);
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(new { Success = false, Message = ex.Message }, _jsonSettings);
            }
        }

        public async Task<string> CreateAsync(string name, string content, bool isDefault = false, string? description = null)
        {
            using var scope = _serviceProvider.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<IOcrPromptService>();

            try
            {
                var prompt = await service.CreateAsync(name, content, isDefault, description);
                return JsonConvert.SerializeObject(new { Success = true, Data = prompt }, _jsonSettings);
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(new { Success = false, Message = ex.Message }, _jsonSettings);
            }
        }

        public async Task<string> UpdateAsync(long id, string name, string content, bool isDefault, string? description = null)
        {
            using var scope = _serviceProvider.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<IOcrPromptService>();

            try
            {
                await service.UpdateAsync(id, name, content, isDefault, description);
                return JsonConvert.SerializeObject(new { Success = true }, _jsonSettings);
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(new { Success = false, Message = ex.Message }, _jsonSettings);
            }
        }

        public async Task<string> DeleteAsync(long id)
        {
            using var scope = _serviceProvider.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<IOcrPromptService>();

            try
            {
                await service.DeleteAsync(id);
                return JsonConvert.SerializeObject(new { Success = true }, _jsonSettings);
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(new { Success = false, Message = ex.Message }, _jsonSettings);
            }
        }

        public async Task<string> SetDefaultAsync(long id)
        {
            using var scope = _serviceProvider.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<IOcrPromptService>();

            try
            {
                await service.SetDefaultAsync(id);
                return JsonConvert.SerializeObject(new { Success = true }, _jsonSettings);
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(new { Success = false, Message = ex.Message }, _jsonSettings);
            }
        }

        public async Task<string> InitializeDefaultPromptAsync()
        {
            using var scope = _serviceProvider.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<IOcrPromptService>();

            try
            {
                await service.InitializeDefaultPromptAsync();
                return JsonConvert.SerializeObject(new { Success = true }, _jsonSettings);
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(new { Success = false, Message = ex.Message }, _jsonSettings);
            }
        }
    }
}
