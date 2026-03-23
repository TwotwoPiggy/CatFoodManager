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

public class GeminiOcrService : IApplicationGeminiOcrService
{
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

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new FlexibleDateTimeConverter() }
    };

    private readonly IGeminiAgentService _agentService;
    private readonly IRepository _repository;
    private readonly ILogger<GeminiOcrService> _logger;
    private readonly IMemoryCache _cache;

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

    public async Task<bool> ValidateModelAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Validating Gemini model");
        return await _agentService.ValidateModelAsync(cancellationToken);
    }

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

    public void ClearModelsCache(string? apiKey = null)
    {
        var cacheKey = GetCacheKey(apiKey);
        _cache.Remove(cacheKey);
        _logger.LogInformation("Cleared models cache for key: {CacheKey}", cacheKey);
    }

    private static string GetCacheKey(string? apiKey)
    {
        if (string.IsNullOrEmpty(apiKey))
        {
            return $"{CacheKeyPrefix}default";
        }

        var hash = ComputeApiKeyHash(apiKey);
        return $"{CacheKeyPrefix}{hash}";
    }

    private static string ComputeApiKeyHash(string apiKey)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(apiKey));
        return Convert.ToHexString(bytes)[..16];
    }

    private static string GetFailedResponseCacheKey(string responseId)
    {
        return $"{FailedResponseCacheKeyPrefix}{responseId}";
    }

    private void AddToFailedList(string responseId, long taskId, string folderPath, string promptText, string? errorMessage)
    {
        var list = _cache.TryGetValue(FailedListCacheKey, out List<FailedResponseCacheItem>? existingList)
            ? existingList!
            : new List<FailedResponseCacheItem>();

        list.RemoveAll(x => x.ResponseId == responseId);

        list.Add(new FailedResponseCacheItem(responseId, taskId, folderPath, promptText, DateTime.Now, errorMessage));
        _cache.Set(FailedListCacheKey, list, FailedResponseCacheDuration);
    }

    private void RemoveFromFailedList(string responseId)
    {
        if (_cache.TryGetValue(FailedListCacheKey, out List<FailedResponseCacheItem>? list))
        {
            list.RemoveAll(x => x.ResponseId == responseId);
        }
    }

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

    public IReadOnlyList<FailedResponseCacheItem> GetFailedResponses()
    {
        if (_cache.TryGetValue(FailedListCacheKey, out List<FailedResponseCacheItem>? list))
        {
            return list.AsReadOnly();
        }

        return [];
    }

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

    public void RemoveFailedResponse(string responseId)
    {
        var cacheKey = GetFailedResponseCacheKey(responseId);

        _cache.Remove(cacheKey);
        RemoveFromFailedList(responseId);
        _logger.LogInformation("Removed cached response for responseId: {ResponseId}", responseId);
    }
}
