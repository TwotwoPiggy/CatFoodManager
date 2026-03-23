using SQLite;

namespace CatFoodManager.Domain.Entities;

/// <summary>
/// 任务配置实体，用于存储后台任务的配置信息。
/// Task configuration entity, used to store configuration information for background tasks.
/// </summary>
[Table("TaskConfigurations")]
public class TaskConfiguration : BaseEntity
{
    /// <summary>
    /// 最大并发任务数。
    /// Maximum number of concurrent tasks.
    /// </summary>
    public int MaxConcurrentTasks { get; set; } = 2;

    /// <summary>
    /// 轮询间隔（秒）。
    /// Polling interval in seconds.
    /// </summary>
    public int PollingIntervalSeconds { get; set; } = 60;

    /// <summary>
    /// 是否启用调度。
    /// Whether scheduling is enabled.
    /// </summary>
    public bool EnableScheduling { get; set; }

    /// <summary>
    /// 默认调度配置。
    /// Default schedule configuration.
    /// </summary>
    public string? DefaultSchedule { get; set; }
}
