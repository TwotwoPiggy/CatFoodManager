using System.Text.Json;
using CatFoodManager.Application.Interfaces;
using CatFoodManager.Core.Models.Dtos;
using CatFoodManager.Domain.Entities;
using CatFoodManager.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace CatFoodManager.Application.Services.Handlers;

public class SyncTaskHandler : ITaskHandler
{
    private readonly IGeminiOcrService _ocrService;
    private readonly IBestPriceService _bestPriceService;
    private readonly ILogger<SyncTaskHandler> _logger;

    public TaskType TaskType => TaskType.ImageSync;

    public SyncTaskHandler(
        IGeminiOcrService ocrService,
        IBestPriceService bestPriceService,
        ILogger<SyncTaskHandler> logger)
    {
        _ocrService = ocrService;
        _bestPriceService = bestPriceService;
        _logger = logger;
    }

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public async Task<TaskResult> HandleAsync(string parameters, CancellationToken cancellationToken = default)
    {
        try
        {
            var syncParams = JsonSerializer.Deserialize<SyncParameters>(parameters, JsonOptions);
            if (syncParams == null || string.IsNullOrEmpty(syncParams.FolderPath))
            {
                return TaskResult.Failed("Invalid parameters: FolderPath is required");
            }

            _logger.LogInformation("Starting image sync from: {FolderPath}", syncParams.FolderPath);

            if (!Directory.Exists(syncParams.FolderPath))
            {
                return TaskResult.Failed($"Directory not found: {syncParams.FolderPath}");
            }

            var results = await _ocrService.ProcessPicturesAsync<BestPriceSyncDto>(
                syncParams.FolderPath,
                syncParams.PromptText ?? string.Empty,
                cancellationToken).ConfigureAwait(false);

            if (results.Items.Count == 0)
            {
                _logger.LogWarning("No results returned from OCR service");
                return TaskResult.Succeeded(JsonSerializer.Serialize(new
                {
                    FolderPath = syncParams.FolderPath,
                    ProcessedCount = 0,
                    SavedCount = 0,
                    ProcessedAt = DateTimeOffset.UtcNow
                }), results.ResponseId);
            }

            var bestPrices = results.Items.Select(dto => new BestPrice
            {
                Name = dto.Name,
                Type = (ProductType)dto.Type,
                Platform = (PlatformType)(dto.Platform > 0 ? dto.Platform : syncParams.Platform),
                LowestPrice = dto.LowestPrice,
                FinalPrice = dto.FinalPrice,
                PicturePath = dto.PicturePath,
                FactoryName = dto.FactoryName,
                HasTestReport = dto.HasTestReport,
                IsWorthRepurchasing = dto.IsWorthRepurchasing,
                PurchasedAt = dto.PurchasedAt,
                HasPurchased = dto.FinalPrice.HasValue,
                CreatedAt = DateTimeOffset.UtcNow
            }).ToList();

            await _bestPriceService.AddRangeAsync(bestPrices, cancellationToken).ConfigureAwait(false);

            var resultJson = JsonSerializer.Serialize(new
            {
                FolderPath = syncParams.FolderPath,
                ProcessedCount = results.Items.Count,
                SavedCount = bestPrices.Count,
                ProcessedAt = DateTimeOffset.UtcNow
            });

            _logger.LogInformation("Image sync completed: {Count} items processed and saved", bestPrices.Count);

            return TaskResult.Succeeded(resultJson, results.ResponseId);
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
        public int Platform { get; set; }
    }
}
