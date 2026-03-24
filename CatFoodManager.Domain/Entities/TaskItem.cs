using CatFoodManager.Domain.Enums;
using SQLite;

namespace CatFoodManager.Domain.Entities;

/// <summary>
/// 任务项实体，表示一个后台任务。
/// Task item entity, representing a background task.
/// </summary>
[Table("Tasks")]
public class TaskItem : BaseEntity
{
    /// <summary>
    /// 任务类型。
    /// Task type.
    /// </summary>
    public TaskType Type { get; set; }

    /// <summary>
    /// 任务状态。
    /// Task status.
    /// </summary>
    public Domain.Enums.TaskStatus Status { get; set; }

    /// <summary>
    /// 任务描述。
    /// Task description.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// 任务参数（JSON格式）。
    /// Task parameters (JSON format).
    /// </summary>
    public string Parameters { get; set; } = "{}";

    /// <summary>
    /// 任务执行结果。
    /// Task execution result.
    /// </summary>
    public string? Result { get; set; }

    /// <summary>
    /// 错误信息。
    /// Error message.
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// 重试次数。
    /// Number of retries.
    /// </summary>
    public int RetryCount { get; set; }

    /// <summary>
    /// 最大重试次数。
    /// Maximum number of retries.
    /// </summary>
    public int MaxRetries { get; set; } = 3;

    /// <summary>
    /// 任务开始时间。
    /// Task start time.
    /// </summary>
    public DateTimeOffset? StartedAt { get; set; }

    /// <summary>
    /// 任务完成时间。
    /// Task completion time.
    /// </summary>
    public DateTimeOffset? CompletedAt { get; set; }

    /// <summary>
    /// 计划执行时间。
    /// Scheduled execution time.
    /// </summary>
    public DateTimeOffset? ScheduledAt { get; set; }

    /// <summary>
    /// 任务优先级。
    /// Task priority.
    /// </summary>
    public int Priority { get; set; }

    /// <summary>
    /// 父任务ID。
    /// Parent task ID.
    /// </summary>
    public long? ParentTaskId { get; set; }

    /// <summary>
    /// 响应ID。
    /// Response ID.
    /// </summary>
    public string? ResponseId { get; set; }
}
