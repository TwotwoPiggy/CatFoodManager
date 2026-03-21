namespace CatFoodManager.Domain.Enums;

public enum TaskStatus
{
    Pending = 0,
    Queued = 1,
    Running = 2,
    Completed = 3,
    Failed = 4,
    Cancelled = 5,
    Retrying = 6
}
