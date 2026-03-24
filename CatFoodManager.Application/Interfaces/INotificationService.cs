namespace CatFoodManager.Application.Interfaces;

/// <summary>
/// 通知服务接口，用于发布任务状态变更通知。
/// Notification service interface, used to publish task status change notifications.
/// </summary>
public interface INotificationService
{
    /// <summary>
    /// 发布任务状态变更通知。
    /// Publishes a task status change notification.
    /// </summary>
    /// <param name="taskId">任务ID / Task ID</param>
    /// <param name="taskName">任务名称 / Task name</param>
    /// <param name="taskType">任务类型 / Task type</param>
    /// <param name="oldStatus">旧状态 / Old status</param>
    /// <param name="newStatus">新状态 / New status</param>
    /// <param name="result">执行结果 / Execution result</param>
    /// <param name="errorMessage">错误信息 / Error message</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
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
