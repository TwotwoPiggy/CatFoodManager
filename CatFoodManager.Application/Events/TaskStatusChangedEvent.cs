namespace CatFoodManager.Application.Events;

/// <summary>
/// 任务状态变更事件，/// Task status changed event.
/// </summary>
public class TaskStatusChangedEvent
{
    /// <summary>
    /// 任务ID。
    /// Task ID.
    /// </summary>
    public long TaskId { get; set; }

    /// <summary>
    /// 任务名称。
    /// Task name.
    /// </summary>
    public string TaskName { get; set; } = string.Empty;

    /// <summary>
    /// 任务类型。
    /// Task type.
    /// </summary>
    public int TaskType { get; set; }

    /// <summary>
    /// 旧状态。
    /// Old status.
    /// </summary>
    public int OldStatus { get; set; }

    /// <summary>
    /// 新状态。
    /// New status.
    /// </summary>
    public int NewStatus { get; set; }

    /// <summary>
    /// 执行结果。
    /// Execution result.
    /// </summary>
    public string? Result { get; set; }

    /// <summary>
    /// 错误信息。
    /// Error message.
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// 时间戳。
    /// Timestamp.
    /// </summary>
    public DateTimeOffset Timestamp { get; set; }
}
