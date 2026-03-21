namespace CatFoodManager.Application.Interfaces;

public interface INotificationService
{
    Task PublishTaskStatusChangedAsync(
        long taskId,
        string taskName,
        int taskType,
        int oldStatus,
        int newStatus,
        string? result = null,
        string? errorMessage = null,
        CancellationToken cancellationToken = default);
}
