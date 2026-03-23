namespace CatFoodManager.Domain.Enums;

/// <summary>
/// 任务状态枚举，定义任务的生命周期状态。
/// Task status enum, defining the lifecycle states of a task.
/// </summary>
public enum TaskStatus
{
    /// <summary>
    /// 待处理。
    /// Pending.
    /// </summary>
    Pending = 0,

    /// <summary>
    /// 已入队。
    /// Queued.
    /// </summary>
    Queued = 1,

    /// <summary>
    /// 运行中。
    /// Running.
    /// </summary>
    Running = 2,

    /// <summary>
    /// 已完成。
    /// Completed.
    /// </summary>
    Completed = 3,

    /// <summary>
    /// 失败。
    /// Failed.
    /// </summary>
    Failed = 4,

    /// <summary>
    /// 已取消。
    /// Cancelled.
    /// </summary>
    Cancelled = 5,

    /// <summary>
    /// 重试中。
    /// Retrying.
    /// </summary>
    Retrying = 6
}
