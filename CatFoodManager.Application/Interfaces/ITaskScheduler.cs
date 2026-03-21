namespace CatFoodManager.Application.Interfaces;

public interface ITaskScheduler
{
    Task StartAsync(CancellationToken cancellationToken = default);
    Task StopAsync(CancellationToken cancellationToken = default);
    Task EnqueueAsync(long taskId, CancellationToken cancellationToken = default);
    Task<int> GetQueueLengthAsync(CancellationToken cancellationToken = default);
    Task<bool> IsRunningAsync(CancellationToken cancellationToken = default);
}
