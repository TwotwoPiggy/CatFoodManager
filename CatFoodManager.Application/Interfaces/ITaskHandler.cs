using CatFoodManager.Domain.Enums;

namespace CatFoodManager.Application.Interfaces;

/// <summary>
/// 任务处理器接口，定义任务处理的标准。
/// Task handler interface, defining standards for task handling.
/// </summary>
public interface ITaskHandler
{
    /// <summary>
    /// 任务类型。
    /// Task type.
    /// </summary>
    TaskType TaskType { get; }

    /// <summary>
    /// 处理任务。
    /// Handles the task.
    /// </summary>
    /// <param name="parameters">任务参数 / Task parameters</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    /// <returns>任务处理结果 / Task handling result</returns>
    Task<TaskResult> HandleAsync(string parameters, CancellationToken cancellationToken = default);
}

/// <summary>
/// 任务处理结果类。
/// Task result class.
/// </summary>
public class TaskResult
{
    /// <summary>
    /// 是否成功。
    /// Whether successful.
    /// </summary>
    public bool Success { get; set; }

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
    /// 响应ID。
    /// Response ID.
    /// </summary>
    public string? ResponseId { get; set; }

    /// <summary>
    /// 创建成功结果。
    /// Creates a successful result.
    /// </summary>
    /// <param name="result">执行结果 / Execution result</param>
    /// <param name="responseId">响应ID / Response ID</param>
    /// <returns>任务结果 / Task result</returns>
    public static TaskResult Succeeded(string? result = null, string? responseId = null) => new() { Success = true, Result = result, ResponseId = responseId };

    /// <summary>
    /// 创建失败结果。
    /// Creates a failed result.
    /// </summary>
    /// <param name="errorMessage">错误信息 / Error message</param>
    /// <param name="responseId">响应ID / Response ID</param>
    /// <returns>任务结果 / Task result</returns>
    public static TaskResult Failed(string errorMessage, string? responseId = null) => new() { Success = false, ErrorMessage = errorMessage, ResponseId = responseId };
}
