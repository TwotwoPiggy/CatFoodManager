using System.Text.Json;
using CatFoodManager.Application.Interfaces;
using CatFoodManager.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace CatFoodManager.Application.Services.Handlers;

public class ImageProcessHandler : ITaskHandler
{
    private readonly ILogger<ImageProcessHandler> _logger;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public TaskType TaskType => TaskType.ImageProcess;

    public ImageProcessHandler(ILogger<ImageProcessHandler> logger)
    {
        _logger = logger;
    }

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

    private class ImageProcessParameters
    {
        public string? ImagePath { get; set; }
        public string? PromptText { get; set; }
        public string? OutputPath { get; set; }
    }
}
