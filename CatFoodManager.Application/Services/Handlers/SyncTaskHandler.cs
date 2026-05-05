using System.Text.Json;
using CatFoodManager.Application.Interfaces;
using CatFoodManager.Core.Models.Dtos;
using CatFoodManager.Domain.Entities;
using CatFoodManager.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace CatFoodManager.Application.Services.Handlers;

/// <summary>
/// 同步任务处理器，处理图片同步和OCR识别任务�?/// Sync task handler, handling image sync and OCR recognition tasks.
/// </summary>
public class SyncTaskHandler : ITaskHandler
{
    private readonly IGeminiOcrService _ocrService;
    private readonly IBestPriceService _bestPriceService;
    private readonly ILogger<SyncTaskHandler> _logger;

    /// <summary>
    /// 任务类型�?    /// Task type.
    /// </summary>
    public TaskType TaskType => TaskType.ImageSync;

    /// <summary>
    /// 构造函数�?    /// Constructor.
    /// </summary>
    /// <param name="ocrService">OCR服务实例 / OCR service instance</param>
    /// <param name="bestPriceService">最低价格服务实�?/ Best price service instance</param>
    /// <param name="logger">日志记录�?/ Logger</param>
    public SyncTaskHandler(
        IGeminiOcrService ocrService,
        IBestPriceService bestPriceService,
        ILogger<SyncTaskHandler> logger)
    {
        _ocrService = ocrService;
        _bestPriceService = bestPriceService;
        _logger = logger;
    }

    /// <summary>
    /// JSON序列化选项�?    /// JSON serialization options.
    /// </summary>
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    /// <summary>
    /// 处理同步任务�?    /// Handles the sync task.
    /// </summary>
    /// <param name="parameters">任务参数 / Task parameters</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    /// <returns>任务处理结果 / Task handling result</returns>
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

    /// <summary>
    /// 同步参数类�?    /// Sync parameters class.
    /// </summary>
    private class SyncParameters
    {
        /// <summary>
        /// 文件夹路径�?        /// Folder path.
        /// </summary>
        public string? FolderPath { get; set; }

        /// <summary>
        /// 提示文本�?        /// Prompt text.
        /// </summary>
        public string? PromptText { get; set; }

        /// <summary>
        /// 平台类型�?        /// Platform type.
        /// </summary>
        public int Platform { get; set; }
    }
}
