using SQLite;

namespace CatFoodManager.Domain.Entities;

[Table("TaskConfigurations")]
public class TaskConfiguration : BaseEntity
{
    public int MaxConcurrentTasks { get; set; } = 2;
    public int PollingIntervalSeconds { get; set; } = 60;
    public bool EnableScheduling { get; set; }
    public string? DefaultSchedule { get; set; }
}
