using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using CatFoodManager.Application.Interfaces;
using CatFoodManager.Core.Interfaces;
using CatFoodManager.Core.Models;
using CatFoodManager.Core.Utils;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Twotwo.Agent.Interfaces;
using Twotwo.Agent.Types;
using IApplicationGeminiOcrService = CatFoodManager.Application.Interfaces.IGeminiOcrService;

namespace CatFoodManager.Application.Services;

/// <summary>
/// Gemini OCR服务类，使用Google Gemini AI进行图片文字识别。
/// Gemini OCR service class, using Google Gemini AI for image text recognition.
/// </summary>
public class GeminiOcrService : IApplicationGeminiOcrService
{
    /// <summary>
    /// 允许的图片扩展名集合。
    /// Set of allowed image extensions.
    /// </summary>
    private static readonly HashSet<string> AllowedExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".jpg",
        ".jpeg",
        ".png",
        ".bmp",
        ".webp"
    };

    private const string CacheKeyPrefix = "GeminiModels_";
    private static readonly TimeSpan CacheDuration = TimeSpan.FromHours(1);

    private const string FailedResponseCacheKeyPrefix = "GeminiResponse_Failed_";
    private const string FailedListCacheKey = "GeminiResponse_FailedList";
    private static readonly TimeSpan FailedResponseCacheDuration = TimeSpan.FromHours(24);

    /// <summary>
    /// JSON序列化选项。
    /// JSON serialization options.
    /// </summary>
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new FlexibleDateTimeConverter() }
    };

    private readonly IGeminiAgentService _agentService;
    private readonly IRepository _repository;
    private readonly ILogger<GeminiOcrService> _logger;
    private readonly IMemoryCache _cache;

    /// <summary>
    /// 构造函数。
    /// Constructor.
    /// </summary>
    /// <param name="agentService">Gemini代理服务实例 / Gemini agent service instance</param>
    /// <param name="repository">仓储实例 / Repository instance</param>
    /// <param name="logger">日志记录器 / Logger</param>
    /// <param name="cache">内存缓存实例 / Memory cache instance</param>
    public GeminiOcrService(
        IGeminiAgentService agentService,
        IRepository repository,
        ILogger<GeminiOcrService> logger,
        IMemoryCache cache)
    {
        _agentService = agentService;
        _repository = repository;
        _logger = logger;
        _cache = cache;
    }

    /// <summary>
    /// 验证模型是否可用。
    /// Validates whether the model is available.
    /// </summary>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    /// <returns>模型是否可用 / Whether the model is available</returns>
    public async Task<bool> ValidateModelAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Validating Gemini model");
        return await _agentService.ValidateModelAsync(cancellationToken);
    }

    /// <summary>
    /// 获取可用模型列表。
    /// Gets the list of available models.
    /// </summary>
    /// <param name="apiKey">API密钥（可选）/ API key (optional)</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    /// <returns>模型信息列表 / List of model information</returns>
    public async Task<IReadOnlyList<ModelInfo>> GetModelsAsync(string? apiKey = null, CancellationToken cancellationToken = default)
    {
        var cacheKey = GetCacheKey(apiKey);

        if (_cache.TryGetValue(cacheKey, out IReadOnlyList<ModelInfo>? cachedModels))
        {
            _logger.LogInformation("Returning cached models for key: {CacheKey}", cacheKey);
            return cachedModels!;
        }

        _logger.LogInformation("Getting available models from API");
        var models = await _agentService.GetModelsAsync(cancellationToken);
        var result = models.Select(m => new ModelInfo(m.Name, m.DisplayName)).ToList();

        var cacheOptions = new MemoryCacheEntryOptions()
            .SetAbsoluteExpiration(CacheDuration)
            .SetSlidingExpiration(TimeSpan.FromMinutes(30));

        _cache.Set(cacheKey, result, cacheOptions);
        _logger.LogInformation("Cached {Count} models for key: {CacheKey}", result.Count, cacheKey);

        return result;
    }

    /// <summary>
    /// 清除模型缓存。
    /// Clears the model cache.
    /// </summary>
    /// <param name="apiKey">API密钥（可选）/ API key (optional)</param>
    public void ClearModelsCache(string? apiKey = null)
    {
        var cacheKey = GetCacheKey(apiKey);
        _cache.Remove(cacheKey);
        _logger.LogInformation("Cleared models cache for key: {CacheKey}", cacheKey);
    }

    /// <summary>
    /// 获取缓存键。
    /// Gets the cache key.
    /// </summary>
    /// <param name="apiKey">API密钥 / API key</param>
    /// <returns>缓存键 / Cache key</returns>
    private static string GetCacheKey(string? apiKey)
    {
        if (string.IsNullOrEmpty(apiKey))
        {
            return $"{CacheKeyPrefix}default";
        }

        var hash = ComputeApiKeyHash(apiKey);
        return $"{CacheKeyPrefix}{hash}";
    }

    /// <summary>
    /// 计算API密钥哈希值。
    /// Computes the API key hash.
    /// </summary>
    /// <param name="apiKey">API密钥 / API key</param>
    /// <returns>哈希字符串 / Hash string</returns>
    private static string ComputeApiKeyHash(string apiKey)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(apiKey));
        return Convert.ToHexString(bytes)[..16];
    }

    /// <summary>
    /// 获取失败响应缓存键。
    /// Gets the failed response cache key.
    /// </summary>
    /// <param name="responseId">响应ID / Response ID</param>
    /// <returns>缓存键 / Cache key</returns>
    private static string GetFailedResponseCacheKey(string responseId)
    {
        return $"{FailedResponseCacheKeyPrefix}{responseId}";
    }

    /// <summary>
    /// 添加到失败列表。
    /// Adds to the failed list.
    /// </summary>
    /// <param name="responseId">响应ID / Response ID</param>
    /// <param name="taskId">任务ID / Task ID</param>
    /// <param name="folderPath">文件夹路径 / Folder path</param>
    /// <param name="promptText">提示文本 / Prompt text</param>
    /// <param name="errorMessage">错误信息 / Error message</param>
    private void AddToFailedList(string responseId, long taskId, string folderPath, string promptText, string? errorMessage)
    {
        var list = _cache.TryGetValue(FailedListCacheKey, out List<FailedResponseCacheItem>? existingList)
            ? existingList!
            : new List<FailedResponseCacheItem>();

        list.RemoveAll(x => x.ResponseId == responseId);

        list.Add(new FailedResponseCacheItem(responseId, taskId, folderPath, promptText, DateTime.Now, errorMessage));
        _cache.Set(FailedListCacheKey, list, FailedResponseCacheDuration);
    }

    /// <summary>
    /// 从失败列表中移除。
    /// Removes from the failed list.
    /// </summary>
    /// <param name="responseId">响应ID / Response ID</param>
    private void RemoveFromFailedList(string responseId)
    {
        if (_cache.TryGetValue(FailedListCacheKey, out List<FailedResponseCacheItem>? list))
        {
            list.RemoveAll(x => x.ResponseId == responseId);
        }
    }

    /// <summary>
    /// 处理图片文件夹中的图片并返回识别结果。
    /// Processes images in the folder and returns recognition results.
    /// </summary>
    /// <typeparam name="T">返回的DTO类型 / The DTO type to return</typeparam>
    /// <param name="folderPath">图片文件夹路径 / Image folder path</param>
    /// <param name="promptText">提示文本 / Prompt text</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    /// <returns>处理结果 / Processing result</returns>
    public async Task<ProcessPicturesResult<T>> ProcessPicturesAsync<T>(string folderPath, string promptText, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Processing pictures from folder: {FolderPath}", folderPath);

        if (string.IsNullOrEmpty(folderPath))
        {
            _logger.LogWarning("Folder path is null or empty");
            throw new ArgumentNullException(nameof(folderPath));
        }

        if (!Directory.Exists(folderPath))
        {
            _logger.LogWarning("Directory not found: {FolderPath}", folderPath);
            throw new DirectoryNotFoundException(folderPath);
        }

        var files = Directory.GetFiles(folderPath)
            .Where(f => AllowedExtensions.Contains(Path.GetExtension(f)))
            .ToList();

        _logger.LogInformation("Found {Count} image files", files.Count);

        var fileBytes = new List<byte[]>();
        foreach (var f in files)
        {
            fileBytes.Add(await File.ReadAllBytesAsync(f, cancellationToken));
        }

        var request = new AIRequest(Text: promptText, Files: fileBytes, MimeType: "image/jpeg");

        string? responseId = null;
        GeminiResponseEntity? entity = null;

        try
        {
            var response = await _agentService.GenerateContentAsync(request);
            if (response == null)
            {
                _logger.LogWarning("Received null response from Gemini agent");
                return new ProcessPicturesResult<T>([], null);
            }

            responseId = response.ResponseId ?? Guid.NewGuid().ToString();
            var cacheKey = GetFailedResponseCacheKey(responseId);

            entity = new GeminiResponseEntity
            {
                Name = responseId,
                ResponseJson = JsonSerializer.Serialize(response),
                ResponseText = response.Text ?? string.Empty,
                ModelVersion = response.ModelVersion ?? string.Empty,
                TotalToken = response.UsageMetadata?.TotalTokenCount ?? 0,
                PromptToken = response.UsageMetadata?.PromptTokenCount ?? 0,
                CreatedAt = DateTime.Now
            };

            _cache.Set(cacheKey, entity, FailedResponseCacheDuration);

            _repository.Add(entity);

            _cache.Remove(cacheKey);
            RemoveFromFailedList(responseId);

            if (string.IsNullOrEmpty(response.Text))
            {
                _logger.LogWarning("Received empty text response");
                return new ProcessPicturesResult<T>([], responseId);
            }

            var jsonText = CleanJsonMarkdown(response.Text);
            var dtos = JsonSerializer.Deserialize<List<T>>(jsonText, JsonOptions) ?? [];
            _logger.LogInformation("Successfully processed pictures, returning {Count} items", dtos.Count);
            return new ProcessPicturesResult<T>(dtos, responseId);
        }
        catch (Exception ex)
        {
            if (!string.IsNullOrEmpty(responseId) && entity != null)
            {
                var cacheKey = GetFailedResponseCacheKey(responseId);
                _cache.Set(cacheKey, entity, FailedResponseCacheDuration);
                AddToFailedList(responseId, 0, folderPath, promptText, ex.Message);
                _logger.LogError(ex, "Error processing pictures. Entity cached with responseId: {ResponseId}", responseId);
            }
            else
            {
                _logger.LogError(ex, "Error processing pictures");
            }
            return new ProcessPicturesResult<T>([], responseId);
        }
    }

    /// <summary>
    /// 清理JSON字符串中的Markdown标记。
    /// Cleans Markdown tags from JSON string.
    /// </summary>
    /// <param name="text">原始文本 / Original text</param>
    /// <returns>清理后的JSON字符串 / Cleaned JSON string</returns>
    private static string CleanJsonMarkdown(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return text;

        var trimmed = text.Trim();
        if (trimmed.StartsWith("```json", StringComparison.OrdinalIgnoreCase))
        {
            trimmed = trimmed.Substring(7);
        }
        else if (trimmed.StartsWith("```", StringComparison.OrdinalIgnoreCase))
        {
            trimmed = trimmed.Substring(3);
        }

        if (trimmed.EndsWith("```", StringComparison.OrdinalIgnoreCase))
        {
            trimmed = trimmed.Substring(0, trimmed.Length - 3);
        }

        return trimmed.Trim();
    }

    /// <summary>
    /// 获取失败的响应列表。
    /// Gets the list of failed responses.
    /// </summary>
    /// <returns>失败响应缓存项列表 / List of failed response cache items</returns>
    public IReadOnlyList<FailedResponseCacheItem> GetFailedResponses()
    {
        if (_cache.TryGetValue(FailedListCacheKey, out List<FailedResponseCacheItem>? list))
        {
            return list.AsReadOnly();
        }

        return [];
    }

    /// <summary>
    /// 重试保存失败的响应数据到数据库。
    /// 当 OCR 处理成功但数据库保存失败时，响应数据会被缓存到内存中，
    /// 调用此方法可重新尝试将缓存的数据持久化到数据库。
    /// Retries saving failed response data to the database.
    /// When OCR processing succeeds but database save fails, response data is cached in memory.
    /// Calling this method attempts to persist the cached data to the database again.
    /// </summary>
    /// <param name="responseId">响应的唯一标识符 / Unique identifier of the response</param>
    /// <returns>true 表示保存成功或缓存已清除；false 表示缓存不存在或保存失败
    /// true if save succeeded or cache cleared; false if cache doesn't exist or save failed</returns>
    public Task<bool> RetrySaveResponseAsync(string responseId)
    {
        var cacheKey = GetFailedResponseCacheKey(responseId);

        if (!_cache.TryGetValue(cacheKey, out GeminiResponseEntity? entity) || entity == null)
        {
            _logger.LogWarning("No cached response found for responseId: {ResponseId}", responseId);
            return Task.FromResult(false);
        }

        try
        {
            _repository.Add(entity);

            _cache.Remove(cacheKey);
            RemoveFromFailedList(responseId);

            _logger.LogInformation("Successfully saved cached response for responseId: {ResponseId}", responseId);
            return Task.FromResult(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save cached response for responseId: {ResponseId}", responseId);
            return Task.FromResult(false);
        }
    }

    /// <summary>
    /// 移除失败的响应记录。
    /// Removes a failed response record.
    /// </summary>
    /// <param name="responseId">响应ID / Response ID</param>
    public void RemoveFailedResponse(string responseId)
    {
        var cacheKey = GetFailedResponseCacheKey(responseId);

        _cache.Remove(cacheKey);
        RemoveFromFailedList(responseId);
        _logger.LogInformation("Removed cached response for responseId: {ResponseId}", responseId);
    }
}
