using CatFoodManager.Application.Interfaces;
using CatFoodManager.Domain.Entities;
using CatFoodManager.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace CatFoodManager.Application.Services
{
    public class OcrPromptService : IOcrPromptService
    {
        private const string DefaultPromptContent = "提取图片购物信息返回JSON: {\"Name\": \"包含品牌名称+产品名称+规格\", \"PurchasedAt\": \"yyyy-MM-dd HH:mm:ss\", \"FinalPrice\": 0.00}\nName要求: 1. 格式: 品牌+产品名称+规格(分量g/kg*数量),过滤:去除\"全价,猫,高蛋白,鲜肉\"等通用词,保留\"蓝莓兔,脆脆乐\"等独特标识。\nFinalPrice要求: 精确到小数点后两位。";
        private const string DefaultPromptName = "默认 OCR 提示词";
        private const string DefaultPromptDescription = "用于识别猫粮购物信息的默认提示词";

        private readonly IRepository<OcrPrompt> _repository;
        private readonly ILogger<OcrPromptService> _logger;

        public OcrPromptService(IRepository<OcrPrompt> repository, ILogger<OcrPromptService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<OcrPrompt?> GetDefaultAsync()
        {
            _logger.LogInformation("Getting default OCR prompt");
            var prompts = await _repository.FindAsync(p => p.IsDefault);
            var defaultPrompt = prompts.FirstOrDefault();
            
            if (defaultPrompt == null)
            {
                _logger.LogInformation("No default prompt found, returning first available prompt");
                var allPrompts = await _repository.GetAllAsync();
                defaultPrompt = allPrompts.FirstOrDefault();
            }
            
            return defaultPrompt;
        }

        public async Task<OcrPrompt?> GetByIdAsync(long id)
        {
            _logger.LogInformation("Getting OCR prompt by id: {Id}", id);
            return await _repository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<OcrPrompt>> GetAllAsync()
        {
            _logger.LogInformation("Getting all OCR prompts");
            var prompts = await _repository.GetAllAsync();
            return prompts.OrderByDescending(p => p.IsDefault).ThenBy(p => p.Name);
        }

        public async Task<OcrPrompt> CreateAsync(string name, string content, bool isDefault = false, string? description = null)
        {
            _logger.LogInformation("Creating OCR prompt: {Name}", name);

            if (isDefault)
            {
                await ClearDefaultFlagAsync();
            }

            var prompt = new OcrPrompt
            {
                Name = name,
                Content = content,
                IsDefault = isDefault,
                Description = description,
                CreatedAt = DateTimeOffset.UtcNow
            };

            await _repository.AddAsync(prompt);
            return prompt;
        }

        public async Task UpdateAsync(long id, string name, string content, bool isDefault, string? description = null)
        {
            _logger.LogInformation("Updating OCR prompt: {Id}", id);

            var prompt = await _repository.GetByIdAsync(id);
            if (prompt == null)
            {
                throw new InvalidOperationException($"OCR prompt with id {id} not found");
            }

            if (isDefault && !prompt.IsDefault)
            {
                await ClearDefaultFlagAsync();
            }

            prompt.Name = name;
            prompt.Content = content;
            prompt.IsDefault = isDefault;
            prompt.Description = description;
            prompt.UpdatedAt = DateTimeOffset.UtcNow;

            await _repository.UpdateAsync(prompt);
        }

        public async Task DeleteAsync(long id)
        {
            _logger.LogInformation("Deleting OCR prompt: {Id}", id);
            var prompt = await _repository.GetByIdAsync(id);
            
            if (prompt != null && prompt.IsDefault)
            {
                var allPrompts = await _repository.GetAllAsync();
                var nextDefault = allPrompts.FirstOrDefault(p => p.Id != id);
                if (nextDefault != null)
                {
                    nextDefault.IsDefault = true;
                    await _repository.UpdateAsync(nextDefault);
                }
            }
            
            await _repository.DeleteAsync(id);
        }

        public async Task SetDefaultAsync(long id)
        {
            _logger.LogInformation("Setting OCR prompt as default: {Id}", id);

            var prompt = await _repository.GetByIdAsync(id);
            if (prompt == null)
            {
                throw new InvalidOperationException($"OCR prompt with id {id} not found");
            }

            await ClearDefaultFlagAsync();

            prompt.IsDefault = true;
            prompt.UpdatedAt = DateTimeOffset.UtcNow;
            await _repository.UpdateAsync(prompt);
        }

        public async Task InitializeDefaultPromptAsync()
        {
            _logger.LogInformation("Initializing default OCR prompt");

            var existingPrompts = await _repository.GetAllAsync();
            if (existingPrompts.Any())
            {
                _logger.LogInformation("OCR prompts already exist, skipping initialization");
                return;
            }

            var defaultPrompt = new OcrPrompt
            {
                Name = DefaultPromptName,
                Content = DefaultPromptContent,
                IsDefault = true,
                Description = DefaultPromptDescription,
                CreatedAt = DateTimeOffset.UtcNow
            };

            await _repository.AddAsync(defaultPrompt);
            _logger.LogInformation("Default OCR prompt created successfully");
        }

        private async Task ClearDefaultFlagAsync()
        {
            var defaultPrompts = await _repository.FindAsync(p => p.IsDefault);
            foreach (var prompt in defaultPrompts)
            {
                prompt.IsDefault = false;
                prompt.UpdatedAt = DateTimeOffset.UtcNow;
                await _repository.UpdateAsync(prompt);
            }
        }
    }
}
