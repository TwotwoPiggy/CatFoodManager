using System.Text.Json;
using CatFoodManager.Core.Interfaces;
using CatFoodManager.Core.Models;
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

    private readonly IGeminiAgentService _agentService;
    private readonly IRepository _repository;
    private readonly ILogger<GeminiOcrService> _logger;

    public GeminiOcrService(
        IGeminiAgentService agentService,
        IRepository repository,
        ILogger<GeminiOcrService> logger)
    {
        _agentService = agentService;
        _repository = repository;
        _logger = logger;
    }

    public async Task<bool> ValidateModelAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Validating Gemini model");
        return await _agentService.ValidateModelAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<T>> ProcessPicturesAsync<T>(string folderPath, string promptText, CancellationToken cancellationToken = default)
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

        try
        {
            var response = await _agentService.GenerateContentAsync(request);
            if (response == null)
            {
                _logger.LogWarning("Received null response from Gemini agent");
                return [];
            }

            var entity = new GeminiResponseEntity
            {
                Name = response.ResponseId,
                ResponseJson = JsonSerializer.Serialize(response),
                ResponseText = response.Text ?? string.Empty,
                ModelVersion = response.ModelVersion ?? string.Empty,
                TotalToken = response.UsageMetadata?.TotalTokenCount ?? 0,
                PromptToken = response.UsageMetadata?.PromptTokenCount ?? 0,
                CreatedAt = DateTime.Now
            };

            _repository.Add(entity);

            if (string.IsNullOrEmpty(response.Text))
            {
                _logger.LogWarning("Received empty text response");
                return [];
            }

            var jsonText = CleanJsonMarkdown(response.Text);
            var dtos = JsonSerializer.Deserialize<List<T>>(jsonText) ?? [];
            _logger.LogInformation("Successfully processed pictures, returning {Count} items", dtos.Count);
            return dtos;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing pictures");
            return [];
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
}
