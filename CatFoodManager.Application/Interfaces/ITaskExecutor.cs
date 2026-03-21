namespace CatFoodManager.Application.Interfaces;

public interface ITaskExecutor
{
    Task ExecuteAsync(long taskId, CancellationToken cancellationToken = default);
    Task<bool> IsRunningAsync(long taskId, CancellationToken cancellationToken = default);
    Task TerminateAsync(long taskId, CancellationToken cancellationToken = default);
    Task<int> GetRunningCountAsync(CancellationToken cancellationToken = default);
}
