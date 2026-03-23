namespace CatFoodManager.Application.Interfaces;

public interface IBackgroundServiceControl
{
    Task<bool> IsRunningAsync(CancellationToken cancellationToken = default);
    Task<bool> IsPausedAsync(CancellationToken cancellationToken = default);
    Task StartServiceAsync(CancellationToken cancellationToken = default);
    Task PauseAsync(CancellationToken cancellationToken = default);
    Task ResumeAsync(CancellationToken cancellationToken = default);
    Task StopServiceAsync(CancellationToken cancellationToken = default);
    Task<ServiceStatusInfo> GetStatusAsync(CancellationToken cancellationToken = default);
}

public record ServiceStatusInfo(
    bool IsRunning,
    bool IsPaused,
    int RunningTaskCount,
    int QueueLength,
    DateTimeOffset? StartedAt,
    DateTimeOffset? LastActivityAt
);
