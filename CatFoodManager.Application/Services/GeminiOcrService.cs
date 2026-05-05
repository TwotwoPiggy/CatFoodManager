using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Reflection;
using CatFoodManager.Application.Interfaces;
using CatFoodManager.Domain.Entities;
using CatFoodManager.Domain.Interfaces;
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
    private const string ImageIdPropertyName = "ImageId";
    private const string PicturePathPropertyName = "PicturePath";

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

    private sealed record ImageInput(string ImageId, string FilePath, byte[] Bytes);

    private sealed class StructuredGeminiResponse<T>
    {
        public List<T>? Items { get; set; }
    }

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
    private readonly IRepository<GeminiResponseEntity> _repository;
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
        IRepository<GeminiResponseEntity> repository,
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
        var result = models.Select(m => new ModelInfo(
            RemoveModelsPrefix(m.Name),
            m.DisplayName)).ToList();

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
    /// 移除模型名称的 "models/" 前缀。
    /// Removes the "models/" prefix from model name.
    /// </summary>
    /// <param name="modelName">模型名称 / Model name</param>
    /// <returns>处理后的模型名称 / Processed model name</returns>
    private static string RemoveModelsPrefix(string modelName)
    {
        const string modelsPrefix = "models/";
        if (modelName.StartsWith(modelsPrefix, StringComparison.OrdinalIgnoreCase))
        {
            return modelName.Substring(modelsPrefix.Length);
        }
        return modelName;
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
        _logger.LogInformation("[OCR] Starting process, folder: {FolderPath}, type: {Type}", folderPath, typeof(T).Name);

        if (string.IsNullOrEmpty(folderPath))
        {
            _logger.LogWarning("[OCR] Folder path is null or empty");
            throw new ArgumentNullException(nameof(folderPath));
        }

        if (!Directory.Exists(folderPath))
        {
            _logger.LogWarning("[OCR] Directory not found: {FolderPath}", folderPath);
            throw new DirectoryNotFoundException(folderPath);
        }

        var files = Directory.GetFiles(folderPath)
            .Where(f => AllowedExtensions.Contains(Path.GetExtension(f)))
            .OrderBy(f => f, StringComparer.OrdinalIgnoreCase)
            .ToList();

        _logger.LogInformation("[OCR] Found {Count} image files in folder", files.Count);

        var imageInputs = new List<ImageInput>(files.Count);
        for (var index = 0; index < files.Count; index++)
        {
            var filePath = files[index];
            var imageId = $"img-{index + 1:D3}";
            var fileBytes = await File.ReadAllBytesAsync(filePath, cancellationToken);
            imageInputs.Add(new ImageInput(imageId, filePath, fileBytes));
            _logger.LogDebug("[OCR] Loaded image #{Index}: {ImageId} ← {FileName} ({Size} bytes)",
                index + 1, imageId, Path.GetFileName(filePath), fileBytes.Length);
        }

        var finalPrompt = BuildStructuredPrompt(promptText, imageInputs);
        _logger.LogInformation("[OCR] Prompt built, length: {PromptLength}, images: {ImageCount}",
            finalPrompt.Length, imageInputs.Count);

        var request = new AIRequest(
            Text: finalPrompt,
            Files: imageInputs.Select(x => x.Bytes).ToList(),
            MimeType: "image/jpeg");

        _logger.LogInformation("[OCR] Sending request to Gemini, images: {ImageCount}, model: {Model}, prompt length: {Length}",
            imageInputs.Count, _agentService.GetType().Name, finalPrompt.Length);

        string? responseId = null;
        GeminiResponseEntity? entity = null;

        try
        {
            var response = await _agentService.GenerateContentAsync(request);
            if (response == null)
            {
                _logger.LogWarning("[OCR] Received null response from Gemini agent");
                return new ProcessPicturesResult<T>([], null);
            }

            responseId = response.ResponseId ?? Guid.NewGuid().ToString();
            _logger.LogInformation(
                "[OCR] Response received: id={ResponseId}, textLen={TextLen}, model={Model}, tokens(total={Total}, prompt={Prompt})",
                responseId, response.Text?.Length ?? 0, response.ModelVersion,
                response.UsageMetadata?.TotalTokenCount ?? 0, response.UsageMetadata?.PromptTokenCount ?? 0);

            // Log first 300 chars of response text for debugging
            if (!string.IsNullOrEmpty(response.Text))
            {
                var preview = response.Text.Length > 300 ? response.Text[..300] + "…" : response.Text;
                _logger.LogDebug("[OCR] Response text preview: {Preview}", preview);
            }

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

            await _repository.AddAsync(entity, cancellationToken);
            _logger.LogDebug("[OCR] Response entity saved to repository, id: {ResponseId}", responseId);

            _cache.Remove(cacheKey);
            RemoveFromFailedList(responseId);

            if (string.IsNullOrEmpty(response.Text))
            {
                _logger.LogWarning("[OCR] Empty text in response, id: {ResponseId}", responseId);
                return new ProcessPicturesResult<T>([], responseId);
            }

            var jsonText = CleanJsonMarkdown(response.Text);
            _logger.LogDebug("[OCR] Cleaned JSON text, length: {Length}", jsonText.Length);

            var dtos = DeserializeItems<T>(jsonText);
            _logger.LogInformation("[OCR] Deserialized {Count} items of type {Type}", dtos.Count, typeof(T).Name);

            ApplyImageMappings(dtos, imageInputs);
            _logger.LogInformation("[OCR] Image mappings applied (reverse order), items: {Count}", dtos.Count);

            // Log mapping result summary
            for (var i = 0; i < dtos.Count && i < imageInputs.Count; i++)
            {
                var dto = dtos[i];
                if (dto == null) continue;
                var mappedId = typeof(T).GetProperty(ImageIdPropertyName)?.GetValue(dto) as string;
                var mappedPath = typeof(T).GetProperty(PicturePathPropertyName)?.GetValue(dto) as string;
                _logger.LogDebug("[OCR]   items[{Index}] → {ImageId} → {FileName}",
                    i, mappedId, mappedPath != null ? Path.GetFileName(mappedPath) : "N/A");
            }

            _logger.LogInformation("[OCR] SUCCESS — {Count} items returned", dtos.Count);
            return new ProcessPicturesResult<T>(dtos, responseId);
        }
        catch (Exception ex)
        {
            if (!string.IsNullOrEmpty(responseId) && entity != null)
            {
                var cacheKey = GetFailedResponseCacheKey(responseId);
                _cache.Set(cacheKey, entity, FailedResponseCacheDuration);
                AddToFailedList(responseId, 0, folderPath, promptText, ex.Message);
                _logger.LogError(ex, "[OCR] FAILED — response cached, id: {ResponseId}, error: {Error}", responseId, ex.Message);
            }
            else
            {
                _logger.LogError(ex, "[OCR] FAILED — no response captured, error: {Error}", ex.Message);
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
    private static string BuildStructuredPrompt(string promptText, IReadOnlyList<ImageInput> imageInputs)
    {
        var builder = new StringBuilder();

        if (!string.IsNullOrWhiteSpace(promptText))
        {
            builder.AppendLine(promptText.Trim());
            builder.AppendLine();
        }

        builder.AppendLine("=== IMAGE PROCESSING INSTRUCTIONS ===");
        builder.AppendLine();
        builder.AppendLine($"You will receive {imageInputs.Count} images. Process each image and extract the requested data.");
        builder.AppendLine();
        builder.AppendLine("CRITICAL OUTPUT RULES:");
        builder.AppendLine("1. Return ONLY valid JSON (no markdown fences, no explanation text)");
        builder.AppendLine("2. JSON structure: {{\"items\": [...]}}");
        builder.AppendLine($"3. The \"items\" array MUST contain exactly {imageInputs.Count} entries — one per image, no skipping");
        builder.AppendLine("4. DO NOT include an \"ImageId\" field — it is handled automatically");
        builder.AppendLine("5. If a field cannot be recognized, use null or empty string (never guess)");
        builder.AppendLine();
        builder.AppendLine("SELF-VERIFICATION (check BEFORE returning):");
        builder.AppendLine($"✓ Does items array have exactly {imageInputs.Count} entries?");
        builder.AppendLine("✓ Did I process every image without skipping?");

        return builder.ToString().TrimEnd();
    }

    private static List<T> DeserializeItems<T>(string jsonText)
    {
        using var document = JsonDocument.Parse(jsonText);

        if (document.RootElement.ValueKind == JsonValueKind.Array)
        {
            return JsonSerializer.Deserialize<List<T>>(jsonText, JsonOptions) ?? [];
        }

        if (document.RootElement.ValueKind == JsonValueKind.Object)
        {
            var wrapper = JsonSerializer.Deserialize<StructuredGeminiResponse<T>>(jsonText, JsonOptions);
            return wrapper?.Items ?? [];
        }

        return [];
    }

    private static void ApplyImageMappings<T>(IReadOnlyList<T> items, IReadOnlyList<ImageInput> imageInputs)
    {
        if (items.Count == 0 || imageInputs.Count == 0)
        {
            return;
        }

        var itemType = typeof(T);
        var imageIdProperty = itemType.GetProperty(ImageIdPropertyName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
        var picturePathProperty = itemType.GetProperty(PicturePathPropertyName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);

        if (imageIdProperty?.PropertyType != typeof(string) || picturePathProperty?.CanWrite != true || picturePathProperty.PropertyType != typeof(string))
        {
            return;
        }

        // The LLM consistently returns items in REVERSE order (last image first).
        // Map in reverse: items[0] → imageInputs[last], items[last] → imageInputs[0].
        var count = Math.Min(items.Count, imageInputs.Count);
        for (var i = 0; i < count; i++)
        {
            var item = items[i];
            if (item == null)
            {
                continue;
            }

            var imageIndex = imageInputs.Count - 1 - i;
            imageIdProperty.SetValue(item, imageInputs[imageIndex].ImageId);
            picturePathProperty.SetValue(item, imageInputs[imageIndex].FilePath);
        }
    }

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
    /// 当 OCR 处理成功但数据库保存失败时，响应数据会被缓存到内存中。
    /// 调用此方法可重新尝试将缓存的数据持久化到数据库。
    /// Retries saving failed response data to the database.
    /// When OCR processing succeeds but database save fails, response data is cached in memory.
    /// Calling this method attempts to persist the cached data to the database again.
    /// </summary>
    /// <param name="responseId">响应的唯一标识 / Unique identifier of the response</param>
    /// <returns>true 表示保存成功或缓存已清除；false 表示缓存不存在或保存失败
    /// true if save succeeded or cache cleared; false if cache doesn't exist or save failed</returns>
    public async Task<bool> RetrySaveResponseAsync(string responseId)
    {
        var cacheKey = GetFailedResponseCacheKey(responseId);

        if (!_cache.TryGetValue(cacheKey, out GeminiResponseEntity? entity) || entity == null)
        {
            _logger.LogWarning("No cached response found for responseId: {ResponseId}", responseId);
            return false;
        }

        try
        {
            await _repository.AddAsync(entity);

            _cache.Remove(cacheKey);
            RemoveFromFailedList(responseId);

            _logger.LogInformation("Successfully saved cached response for responseId: {ResponseId}", responseId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save cached response for responseId: {ResponseId}", responseId);
            return false;
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
