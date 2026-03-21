using System.Text.Json;
using CatFoodManager.Application.Interfaces;
using CatFoodManager.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace CatFoodManager.Application.Services.Handlers;

public class SyncTaskHandler : ITaskHandler
{
    private readonly IGeminiOcrService _ocrService;
    private readonly ILogger<SyncTaskHandler> _logger;

    public TaskType TaskType => TaskType.ImageSync;

    public SyncTaskHandler(IGeminiOcrService ocrService, ILogger<SyncTaskHandler> logger)
    {
        _ocrService = ocrService;
        _logger = logger;
    }

    public async Task<TaskResult> HandleAsync(string parameters, CancellationToken cancellationToken = default)
    {
        try
        {
            var syncParams = JsonSerializer.Deserialize<SyncParameters>(parameters);
            if (syncParams == null || string.IsNullOrEmpty(syncParams.FolderPath))
            {
                return TaskResult.Failed("Invalid parameters: FolderPath is required");
            }

            _logger.LogInformation("Starting image sync from: {FolderPath}", syncParams.FolderPath);

            if (!Directory.Exists(syncParams.FolderPath))
            {
                return TaskResult.Failed($"Directory not found: {syncParams.FolderPath}");
            }

            var results = await _ocrService.ProcessPicturesAsync<object>(
                syncParams.FolderPath,
                syncParams.PromptText ?? string.Empty,
                cancellationToken).ConfigureAwait(false);

            var resultJson = JsonSerializer.Serialize(new
            {
                FolderPath = syncParams.FolderPath,
                ProcessedCount = results.Count,
                ProcessedAt = DateTimeOffset.UtcNow
            });

            _logger.LogInformation("Image sync completed: {Count} items processed", results.Count);

            return TaskResult.Succeeded(resultJson);
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Image sync was cancelled");
            return TaskResult.Failed("Task was cancelled");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Image sync failed");
            return TaskResult.Failed(ex.Message);
        }
    }

    private class SyncParameters
    {
        public string? FolderPath { get; set; }
        public string? PromptText { get; set; }
    }
}
