using CatFoodManager.Application.Common;

namespace CatFoodManager.Application.Interfaces;

/// <summary>
/// 后台服务控制接口，/// Background service control interface.
/// </summary>
public interface IBackgroundServiceControl
{
    /// <summary>
    /// 检查服务是否正在运行。
    /// Checks if the service is running.
    /// </summary>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    /// <returns>是否正在运行 / Whether running</returns>
    Task<bool> IsRunningAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 检查服务是否已暂停。
    /// Checks if the service is paused.
    /// </summary>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    /// <returns>是否已暂停 / Whether paused</returns>
    Task<bool> IsPausedAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 启动服务。
    /// Starts the service.
    /// </summary>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    Task StartServiceAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 暂停服务。
    /// Pauses the service.
    /// </summary>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    Task PauseAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 恢复服务。
    /// Resumes the service.
    /// </summary>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    Task ResumeAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 停止服务。
    /// Stops the service.
    /// </summary>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    Task StopServiceAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取服务状态信息。
    /// Gets the service status information.
    /// </summary>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    /// <returns>服务状态信息 / Service status information</returns>
    Task<ServiceStatusInfo> GetStatusAsync(CancellationToken cancellationToken = default);
}

