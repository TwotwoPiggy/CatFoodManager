using CatFoodManager.Domain.Enums;
using SQLite;

namespace CatFoodManager.Domain.Entities;

[Table("Tasks")]
public class TaskItem : BaseEntity
{
    public TaskType Type { get; set; }
    public Domain.Enums.TaskStatus Status { get; set; }
    public string? Description { get; set; }
    public string Parameters { get; set; } = "{}";
    public string? Result { get; set; }
    public string? ErrorMessage { get; set; }
    public int RetryCount { get; set; }
    public int MaxRetries { get; set; } = 3;
    public DateTimeOffset? StartedAt { get; set; }
    public DateTimeOffset? CompletedAt { get; set; }
    public DateTimeOffset? ScheduledAt { get; set; }
    public int Priority { get; set; }
    public long? ParentTaskId { get; set; }
}
