using System.Text.Json;
using CatFoodManager.Application.Interfaces;
using CatFoodManager.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace CatFoodManager.Application.Services.Handlers;

/// <summary>
/// 图片任务处理器，处理图片删除和移动任务。
/// Image task handler, handling image delete and move tasks.
/// </summary>
public class ImageTaskHandler : ITaskHandler
{
    private readonly ILogger<ImageTaskHandler> _logger;

    /// <summary>
    /// JSON序列化选项。
    /// JSON serialization options.
    /// </summary>
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    /// <summary>
    /// 任务类型。
    /// Task type.
    /// </summary>
    public TaskType TaskType => TaskType.ImageDelete;

    /// <summary>
    /// 构造函数。
    /// Constructor.
    /// </summary>
    /// <param name="logger">日志记录器 / Logger</param>
    public ImageTaskHandler(ILogger<ImageTaskHandler> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// 处理图片任务。
    /// Handles the image task.
    /// </summary>
    /// <param name="parameters">任务参数 / Task parameters</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    /// <returns>任务处理结果 / Task handling result</returns>
    public async Task<TaskResult> HandleAsync(string parameters, CancellationToken cancellationToken = default)
    {
        try
        {
            var imageParams = JsonSerializer.Deserialize<ImageParameters>(parameters, JsonOptions);
            if (imageParams == null || string.IsNullOrEmpty(imageParams.ImagePath))
            {
                return TaskResult.Failed("Invalid parameters: ImagePath is required");
            }

            _logger.LogInformation("Processing image task: {Action} - {Path}", imageParams.Action, imageParams.ImagePath);

            if (!File.Exists(imageParams.ImagePath))
            {
                return TaskResult.Failed($"Image not found: {imageParams.ImagePath}");
            }

            switch (imageParams.Action?.ToLowerInvariant())
            {
                case "delete":
                    await Task.Run(() => File.Delete(imageParams.ImagePath), cancellationToken).ConfigureAwait(false);
                    _logger.LogInformation("Image deleted: {Path}", imageParams.ImagePath);
                    return TaskResult.Succeeded(JsonSerializer.Serialize(new { Action = "delete", Path = imageParams.ImagePath, DeletedAt = DateTimeOffset.UtcNow }));

                case "move":
                    if (string.IsNullOrEmpty(imageParams.DestinationPath))
                    {
                        return TaskResult.Failed("DestinationPath is required for move action");
                    }

                    var destDir = Path.GetDirectoryName(imageParams.DestinationPath);
                    if (!string.IsNullOrEmpty(destDir) && !Directory.Exists(destDir))
                    {
                        Directory.CreateDirectory(destDir);
                    }

                    await Task.Run(() => File.Move(imageParams.ImagePath, imageParams.DestinationPath), cancellationToken).ConfigureAwait(false);
                    _logger.LogInformation("Image moved: {From} -> {To}", imageParams.ImagePath, imageParams.DestinationPath);
                    return TaskResult.Succeeded(JsonSerializer.Serialize(new { Action = "move", From = imageParams.ImagePath, To = imageParams.DestinationPath, MovedAt = DateTimeOffset.UtcNow }));

                default:
                    return TaskResult.Failed($"Unknown action: {imageParams.Action}");
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Image task was cancelled");
            return TaskResult.Failed("Task was cancelled");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Image task failed");
            return TaskResult.Failed(ex.Message);
        }
    }

    /// <summary>
    /// 图片参数类。
    /// Image parameters class.
    /// </summary>
    private class ImageParameters
    {
        /// <summary>
        /// 操作类型（delete/move）。
        /// Action type (delete/move).
        /// </summary>
        public string? Action { get; set; }

        /// <summary>
        /// 图片路径。
        /// Image path.
        /// </summary>
        public string? ImagePath { get; set; }

        /// <summary>
        /// 目标路径（移动操作时使用）。
        /// Destination path (used for move action).
        /// </summary>
        public string? DestinationPath { get; set; }
    }
}
