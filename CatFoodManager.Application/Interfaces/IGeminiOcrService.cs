namespace CatFoodManager.Application.Interfaces;

/// <summary>
/// Gemini OCR服务接口，提供AI模型验证和图片处理功能。
/// Gemini OCR service interface, providing AI model validation and image processing functionality.
/// </summary>
public interface IGeminiOcrService
{
    /// <summary>
    /// 验证模型是否可用。
    /// Validates whether the model is available.
    /// </summary>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    /// <returns>模型是否可用 / Whether the model is available</returns>
    Task<bool> ValidateModelAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 处理图片文件夹中的图片并返回识别结果。
    /// Processes images in the folder and returns recognition results.
    /// </summary>
    /// <typeparam name="T">返回的DTO类型 / The DTO type to return</typeparam>
    /// <param name="folderPath">图片文件夹路径 / Image folder path</param>
    /// <param name="promptText">提示文本 / Prompt text</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    /// <returns>处理结果 / Processing result</returns>
    Task<ProcessPicturesResult<T>> ProcessPicturesAsync<T>(string folderPath, string promptText, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取可用模型列表。
    /// Gets the list of available models.
    /// </summary>
    /// <param name="apiKey">API密钥（可选）/ API key (optional)</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    /// <returns>模型信息列表 / List of model information</returns>
    Task<IReadOnlyList<ModelInfo>> GetModelsAsync(string? apiKey = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// 清除模型缓存。
    /// Clears the model cache.
    /// </summary>
    /// <param name="apiKey">API密钥（可选）/ API key (optional)</param>
    void ClearModelsCache(string? apiKey = null);

    /// <summary>
    /// 获取失败的响应列表。
    /// Gets the list of failed responses.
    /// </summary>
    /// <returns>失败响应缓存项列表 / List of failed response cache items</returns>
    IReadOnlyList<FailedResponseCacheItem> GetFailedResponses();

    /// <summary>
    /// 重试保存失败的响应。
    /// Retries saving a failed response.
    /// </summary>
    /// <param name="responseId">响应ID / Response ID</param>
    /// <returns>是否成功 / Whether successful</returns>
    Task<bool> RetrySaveResponseAsync(string responseId);

    /// <summary>
    /// 移除失败的响应记录。
    /// Removes a failed response record.
    /// </summary>
    /// <param name="responseId">响应ID / Response ID</param>
    void RemoveFailedResponse(string responseId);
}

/// <summary>
/// 模型信息记录。
/// Model information record.
/// </summary>
/// <param name="Name">模型名称 / Model name</param>
/// <param name="DisplayName">显示名称 / Display name</param>
public record ModelInfo(string Name, string DisplayName);

/// <summary>
/// 图片处理结果记录。
/// Process pictures result record.
/// </summary>
/// <typeparam name="T">DTO类型 / DTO type</typeparam>
/// <param name="Items">处理结果列表 / List of processing results</param>
/// <param name="ResponseId">响应ID / Response ID</param>
public record ProcessPicturesResult<T>(
    List<T> Items,
    string? ResponseId
);

/// <summary>
/// 失败响应缓存项记录。
/// Failed response cache item record.
/// </summary>
/// <param name="ResponseId">响应ID / Response ID</param>
/// <param name="TaskId">任务ID / Task ID</param>
/// <param name="FolderPath">文件夹路径 / Folder path</param>
/// <param name="PromptText">提示文本 / Prompt text</param>
/// <param name="FailedAt">失败时间 / Failure time</param>
/// <param name="ErrorMessage">错误信息 / Error message</param>
public record FailedResponseCacheItem(
    string ResponseId,
    long TaskId,
    string FolderPath,
    string PromptText,
    DateTime FailedAt,
    string? ErrorMessage
);
