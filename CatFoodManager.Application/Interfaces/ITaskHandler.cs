using CatFoodManager.Domain.Enums;

namespace CatFoodManager.Application.Interfaces;

public interface ITaskHandler
{
    TaskType TaskType { get; }
    Task<TaskResult> HandleAsync(string parameters, CancellationToken cancellationToken = default);
}

public class TaskResult
{
    public bool Success { get; set; }
    public string? Result { get; set; }
    public string? ErrorMessage { get; set; }

    public static TaskResult Succeeded(string? result = null) => new() { Success = true, Result = result };
    public static TaskResult Failed(string errorMessage) => new() { Success = false, ErrorMessage = errorMessage };
}
