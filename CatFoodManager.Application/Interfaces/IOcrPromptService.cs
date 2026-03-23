using CatFoodManager.Domain.Entities;

namespace CatFoodManager.Application.Interfaces
{
    /// <summary>
    /// OCR提示词服务接口，提供OCR提示词的管理功能。
    /// OCR prompt service interface, providing OCR prompt management functionality.
    /// </summary>
    public interface IOcrPromptService
    {
        /// <summary>
        /// 获取默认提示词。
        /// Gets the default prompt.
        /// </summary>
        /// <returns>默认提示词实体或null / Default prompt entity or null</returns>
        Task<OcrPrompt?> GetDefaultAsync();

        /// <summary>
        /// 根据ID获取提示词。
        /// Gets a prompt by ID.
        /// </summary>
        /// <param name="id">提示词ID / Prompt ID</param>
        /// <returns>提示词实体或null / Prompt entity or null</returns>
        Task<OcrPrompt?> GetByIdAsync(long id);

        /// <summary>
        /// 获取所有提示词。
        /// Gets all prompts.
        /// </summary>
        /// <returns>提示词列表 / List of prompts</returns>
        Task<IEnumerable<OcrPrompt>> GetAllAsync();

        /// <summary>
        /// 创建提示词。
        /// Creates a prompt.
        /// </summary>
        /// <param name="name">提示词名称 / Prompt name</param>
        /// <param name="content">提示词内容 / Prompt content</param>
        /// <param name="isDefault">是否为默认 / Whether it is default</param>
        /// <param name="description">描述 / Description</param>
        /// <returns>创建的提示词实体 / Created prompt entity</returns>
        Task<OcrPrompt> CreateAsync(string name, string content, bool isDefault = false, string? description = null);

        /// <summary>
        /// 更新提示词。
        /// Updates a prompt.
        /// </summary>
        /// <param name="id">提示词ID / Prompt ID</param>
        /// <param name="name">提示词名称 / Prompt name</param>
        /// <param name="content">提示词内容 / Prompt content</param>
        /// <param name="isDefault">是否为默认 / Whether it is default</param>
        /// <param name="description">描述 / Description</param>
        Task UpdateAsync(long id, string name, string content, bool isDefault, string? description = null);

        /// <summary>
        /// 删除提示词。
        /// Deletes a prompt.
        /// </summary>
        /// <param name="id">提示词ID / Prompt ID</param>
        Task DeleteAsync(long id);

        /// <summary>
        /// 设置默认提示词。
        /// Sets the default prompt.
        /// </summary>
        /// <param name="id">提示词ID / Prompt ID</param>
        Task SetDefaultAsync(long id);

        /// <summary>
        /// 初始化默认提示词。
        /// Initializes the default prompt.
        /// </summary>
        Task InitializeDefaultPromptAsync();
    }
}
