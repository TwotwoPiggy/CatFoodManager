using CatFoodManager.Application.Interfaces;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace CatfoodManagement.Services;

public class NotificationService : INotificationService
{
    private readonly MainForm _mainForm;
    private readonly ILogger<NotificationService> _logger;

    public NotificationService(MainForm mainForm, ILogger<NotificationService> logger)
    {
        _mainForm = mainForm;
        _logger = logger;
    }

    public async Task PublishTaskStatusChangedAsync(
        long taskId,
        string taskName,
        int taskType,
        int oldStatus,
        int newStatus,
        string? result = null,
        string? errorMessage = null,
        CancellationToken cancellationToken = default)
    {
        var eventData = new
        {
            taskId,
            taskName,
            taskType,
            oldStatus,
            newStatus,
            result,
            errorMessage,
            timestamp = DateTimeOffset.UtcNow.ToString("O")
        };

        var json = JsonConvert.SerializeObject(eventData);
        var script = $"if(typeof window.onTaskStatusChanged === 'function') {{ window.onTaskStatusChanged({json}); }}";

        try
        {
            await _mainForm.ExecuteScriptAsync(script).ConfigureAwait(false);
            _logger.LogInformation("Task status change notification sent: TaskId={TaskId}, Status={NewStatus}", taskId, newStatus);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send task status notification: TaskId={TaskId}", taskId);
        }
    }
}
