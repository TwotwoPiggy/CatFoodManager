using System.Text.Json;
using CatFoodManager.Application.Interfaces;
using CatFoodManager.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace CatFoodManager.Application.Services.Handlers;

/// <summary>
/// 图片处理任务处理器，处理图片AI识别任务�?/// Image process task handler, handling image AI recognition tasks.
/// </summary>
public class ImageProcessHandler : ITaskHandler
{
    private readonly ILogger<ImageProcessHandler> _logger;

    /// <summary>
    /// JSON序列化选项�?    /// JSON serialization options.
    /// </summary>
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    /// <summary>
    /// 任务类型�?    /// Task type.
    /// </summary>
    public TaskType TaskType => TaskType.ImageProcess;

    /// <summary>
    /// 构造函数�?    /// Constructor.
    /// </summary>
    /// <param name="logger">日志记录�?/ Logger</param>
    public ImageProcessHandler(ILogger<ImageProcessHandler> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// 处理图片处理任务�?    /// Handles the image process task.
    /// </summary>
    /// <param name="parameters">任务参数 / Task parameters</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    /// <returns>任务处理结果 / Task handling result</returns>
    public async Task<TaskResult> HandleAsync(string parameters, CancellationToken cancellationToken = default)
    {
        try
        {
            var processParams = JsonSerializer.Deserialize<ImageProcessParameters>(parameters, JsonOptions);
            if (processParams == null || string.IsNullOrEmpty(processParams.ImagePath))
            {
                return TaskResult.Failed("Invalid parameters: ImagePath is required");
            }

            _logger.LogInformation("Processing image: {Path} with prompt: {Prompt}",
                processParams.ImagePath,
                processParams.PromptText?[..Math.Min(50, processParams.PromptText.Length)]);

            if (!File.Exists(processParams.ImagePath))
            {
                return TaskResult.Failed($"Image not found: {processParams.ImagePath}");
            }

            await Task.Delay(100, cancellationToken).ConfigureAwait(false);

            var resultJson = JsonSerializer.Serialize(new
            {
                ImagePath = processParams.ImagePath,
                PromptText = processParams.PromptText,
                ProcessedAt = DateTimeOffset.UtcNow,
                Message = "Image processing placeholder - implement actual AI processing here"
            });

            _logger.LogInformation("Image processing completed: {Path}", processParams.ImagePath);

            return TaskResult.Succeeded(resultJson);
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Image processing was cancelled");
            return TaskResult.Failed("Task was cancelled");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Image processing failed");
            return TaskResult.Failed(ex.Message);
        }
    }

    /// <summary>
    /// 图片处理参数类�?    /// Image process parameters class.
    /// </summary>
    private class ImageProcessParameters
    {
        /// <summary>
        /// 图片路径�?        /// Image path.
        /// </summary>
        public string? ImagePath { get; set; }

        /// <summary>
        /// 提示文本�?        /// Prompt text.
        /// </summary>
        public string? PromptText { get; set; }

        /// <summary>
        /// 输出路径�?        /// Output path.
        /// </summary>
        public string? OutputPath { get; set; }
    }
}
