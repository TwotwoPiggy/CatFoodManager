namespace CatFoodManager.Application.Interfaces;

/// <summary>
/// 任务调度器接口，提供任务调度和队列管理功能。
/// Task scheduler interface, providing task scheduling and queue management functionality.
/// </summary>
public interface ITaskScheduler
{
    /// <summary>
    /// 启动调度器。
    /// Starts the scheduler.
    /// </summary>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    Task StartAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 停止调度器。
    /// Stops the scheduler.
    /// </summary>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    Task StopAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 将任务加入队列。
    /// Enqueues a task.
    /// </summary>
    /// <param name="taskId">任务ID / Task ID</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    Task EnqueueAsync(long taskId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取队列长度。
    /// Gets the queue length.
    /// </summary>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    /// <returns>队列长度 / Queue length</returns>
    Task<int> GetQueueLengthAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 检查调度器是否正在运行。
    /// Checks if the scheduler is running.
    /// </summary>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    /// <returns>是否正在运行 / Whether running</returns>
    Task<bool> IsRunningAsync(CancellationToken cancellationToken = default);
}
