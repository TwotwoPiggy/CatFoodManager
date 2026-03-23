using CatFoodManager.Application.Interfaces;
using CatFoodManager.Domain.Entities;
using CatFoodManager.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace CatFoodManager.Application.Services
{
    /// <summary>
    /// OCR提示词服务类，提供OCR提示词的管理功能。
    /// OCR prompt service class, providing OCR prompt management functionality.
    /// </summary>
    public class OcrPromptService : IOcrPromptService
    {
        /// <summary>
        /// 默认提示词内容。
        /// Default prompt content.
        /// </summary>
        private const string DefaultPromptContent = "提取图片购物信息返回JSON: {\"Name\": \"包含品牌名称+产品名称+规格\", \"PurchasedAt\": \"yyyy-MM-dd HH:mm:ss\", \"FinalPrice\": 0.00}\nName要求: 1. 格式: 品牌+产品名称+规格(分量g/kg*数量),过滤:去除\"全价,猫,高蛋白,鲜肉\"等通用词,保留\"蓝莓兔,脆脆乐\"等独特标识。\nFinalPrice要求: 精确到小数点后两位。";

        /// <summary>
        /// 默认提示词名称。
        /// Default prompt name.
        /// </summary>
        private const string DefaultPromptName = "默认 OCR 提示词";

        /// <summary>
        /// 默认提示词描述。
        /// Default prompt description.
        /// </summary>
        private const string DefaultPromptDescription = "用于识别猫粮购物信息的默认提示词";

        private readonly IRepository<OcrPrompt> _repository;
        private readonly ILogger<OcrPromptService> _logger;

        /// <summary>
        /// 构造函数。
        /// Constructor.
        /// </summary>
        /// <param name="repository">仓储实例 / Repository instance</param>
        /// <param name="logger">日志记录器 / Logger</param>
        public OcrPromptService(IRepository<OcrPrompt> repository, ILogger<OcrPromptService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        /// <summary>
        /// 获取默认提示词。
        /// Gets the default prompt.
        /// </summary>
        /// <returns>默认提示词实体或null / Default prompt entity or null</returns>
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

        /// <summary>
        /// 根据ID获取提示词。
        /// Gets a prompt by ID.
        /// </summary>
        /// <param name="id">提示词ID / Prompt ID</param>
        /// <returns>提示词实体或null / Prompt entity or null</returns>
        public async Task<OcrPrompt?> GetByIdAsync(long id)
        {
            _logger.LogInformation("Getting OCR prompt by id: {Id}", id);
            return await _repository.GetByIdAsync(id);
        }

        /// <summary>
        /// 获取所有提示词。
        /// Gets all prompts.
        /// </summary>
        /// <returns>提示词列表 / List of prompts</returns>
        public async Task<IEnumerable<OcrPrompt>> GetAllAsync()
        {
            _logger.LogInformation("Getting all OCR prompts");
            var prompts = await _repository.GetAllAsync();
            return prompts.OrderByDescending(p => p.IsDefault).ThenBy(p => p.Name);
        }

        /// <summary>
        /// 创建提示词。
        /// Creates a prompt.
        /// </summary>
        /// <param name="name">提示词名称 / Prompt name</param>
        /// <param name="content">提示词内容 / Prompt content</param>
        /// <param name="isDefault">是否为默认 / Whether it is default</param>
        /// <param name="description">描述 / Description</param>
        /// <returns>创建的提示词实体 / Created prompt entity</returns>
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

        /// <summary>
        /// 更新提示词。
        /// Updates a prompt.
        /// </summary>
        /// <param name="id">提示词ID / Prompt ID</param>
        /// <param name="name">提示词名称 / Prompt name</param>
        /// <param name="content">提示词内容 / Prompt content</param>
        /// <param name="isDefault">是否为默认 / Whether it is default</param>
        /// <param name="description">描述 / Description</param>
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

        /// <summary>
        /// 删除提示词。
        /// Deletes a prompt.
        /// </summary>
        /// <param name="id">提示词ID / Prompt ID</param>
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

        /// <summary>
        /// 设置默认提示词。
        /// Sets the default prompt.
        /// </summary>
        /// <param name="id">提示词ID / Prompt ID</param>
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

        /// <summary>
        /// 初始化默认提示词。
        /// Initializes the default prompt.
        /// </summary>
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

        /// <summary>
        /// 清除默认标记。
        /// Clears the default flag.
        /// </summary>
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
