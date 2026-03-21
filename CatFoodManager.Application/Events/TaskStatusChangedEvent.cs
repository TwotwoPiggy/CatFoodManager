namespace CatFoodManager.Application.Events;

public class TaskStatusChangedEvent
{
    public long TaskId { get; set; }
    public string TaskName { get; set; } = string.Empty;
    public int TaskType { get; set; }
    public int OldStatus { get; set; }
    public int NewStatus { get; set; }
    public string? Result { get; set; }
    public string? ErrorMessage { get; set; }
    public DateTimeOffset Timestamp { get; set; }
}
