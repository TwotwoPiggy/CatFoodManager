namespace CatFoodManager.Application.Interfaces;

/// <summary>
/// 任务执行器接口，提供任务执行和管理功能。
/// Task executor interface, providing task execution and management functionality.
/// </summary>
public interface ITaskExecutor
{
    /// <summary>
    /// 执行指定任务。
    /// Executes the specified task.
    /// </summary>
    /// <param name="taskId">任务ID / Task ID</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    Task ExecuteAsync(long taskId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 检查任务是否正在运行。
    /// Checks if a task is running.
    /// </summary>
    /// <param name="taskId">任务ID / Task ID</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    /// <returns>是否正在运行 / Whether running</returns>
    Task<bool> IsRunningAsync(long taskId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 终止指定任务。
    /// Terminates the specified task.
    /// </summary>
    /// <param name="taskId">任务ID / Task ID</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    Task TerminateAsync(long taskId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取正在运行的任务数量。
    /// Gets the count of running tasks.
    /// </summary>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    /// <returns>正在运行的任务数量 / Count of running tasks</returns>
    Task<int> GetRunningCountAsync(CancellationToken cancellationToken = default);
}
