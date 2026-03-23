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
    public string? ResponseId { get; set; }

    public static TaskResult Succeeded(string? result = null, string? responseId = null) => new() { Success = true, Result = result, ResponseId = responseId };
    public static TaskResult Failed(string errorMessage, string? responseId = null) => new() { Success = false, ErrorMessage = errorMessage, ResponseId = responseId };
}
