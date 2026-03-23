using System.Text.Json;
using CatFoodManager.Application.Interfaces;
using CatFoodManager.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace CatFoodManager.Application.Services.Handlers;

public class ImageTaskHandler : ITaskHandler
{
    private readonly ILogger<ImageTaskHandler> _logger;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public TaskType TaskType => TaskType.ImageDelete;

    public ImageTaskHandler(ILogger<ImageTaskHandler> logger)
    {
        _logger = logger;
    }

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

    private class ImageParameters
    {
        public string? Action { get; set; }
        public string? ImagePath { get; set; }
        public string? DestinationPath { get; set; }
    }
}
