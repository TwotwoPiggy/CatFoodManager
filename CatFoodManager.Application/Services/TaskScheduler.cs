using System.Threading.Channels;
using CatFoodManager.Application.Interfaces;
using CatFoodManager.Domain.Enums;
using CatFoodManager.Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CatFoodManager.Application.Services;

public class TaskScheduler : ITaskScheduler
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<TaskScheduler> _logger;

    private const int MaxQueueCapacity = 10000;

    private readonly Channel<long> _taskQueue;
    private CancellationTokenSource? _cancellationTokenSource;
    private Task? _processingTask;
    private volatile bool _isRunning;

    public TaskScheduler(
        IServiceProvider serviceProvider,
        ILogger<TaskScheduler> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;

        _taskQueue = Channel.CreateBounded<long>(new BoundedChannelOptions(MaxQueueCapacity)
        {
            SingleReader = true,
            SingleWriter = false,
            FullMode = BoundedChannelFullMode.Wait
        });
    }

    public async Task StartAsync(CancellationToken cancellationToken = default)
    {
        if (_isRunning)
        {
            _logger.LogWarning("Task scheduler is already running");
            return;
        }

        _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        _isRunning = true;

        _processingTask = ProcessQueueAsync(_cancellationTokenSource.Token);

        _logger.LogInformation("Task scheduler started");
        await Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken cancellationToken = default)
    {
        if (!_isRunning)
        {
            return;
        }

        _cancellationTokenSource?.Cancel();
        _isRunning = false;

        if (_processingTask != null)
        {
            try
            {
                await _processingTask.ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                // 正常的取消操作，忽略
            }
        }

        _logger.LogInformation("Task scheduler stopped");
    }

    public async Task EnqueueAsync(long taskId, CancellationToken cancellationToken = default)
    {
        using var scope = _serviceProvider.CreateScope();
        var taskRepository = scope.ServiceProvider.GetRequiredService<IRepository<Domain.Entities.TaskItem>>();

        var task = await taskRepository.GetByIdAsync(taskId, cancellationToken).ConfigureAwait(false);
        if (task == null)
        {
            _logger.LogWarning("Task not found: {Id}", taskId);
            return;
        }

        if (task.Status == Domain.Enums.TaskStatus.Running || task.Status == Domain.Enums.TaskStatus.Completed)
        {
            _logger.LogWarning("Cannot enqueue task with status: {Status}", task.Status);
            return;
        }

        task.Status = Domain.Enums.TaskStatus.Queued;
        task.UpdatedAt = DateTimeOffset.UtcNow;
        await taskRepository.UpdateAsync(task, cancellationToken).ConfigureAwait(false);

        await _taskQueue.Writer.WriteAsync(taskId, cancellationToken).ConfigureAwait(false);

        _logger.LogInformation("Task enqueued: {Id}", taskId);
    }

    public Task<int> GetQueueLengthAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_taskQueue.Reader.Count);
    }

    public Task<bool> IsRunningAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_isRunning);
    }

    private async Task ProcessQueueAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Task queue processing started");

        await foreach (var taskId in _taskQueue.Reader.ReadAllAsync(cancellationToken))
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var taskRepository = scope.ServiceProvider.GetRequiredService<IRepository<Domain.Entities.TaskItem>>();

                var task = await taskRepository.GetByIdAsync(taskId, cancellationToken).ConfigureAwait(false);
                if (task == null || task.Status == Domain.Enums.TaskStatus.Cancelled)
                {
                    _logger.LogDebug("Skipping task: {Id} (not found or cancelled)", taskId);
                    continue;
                }

                task.Status = Domain.Enums.TaskStatus.Pending;
                task.UpdatedAt = DateTimeOffset.UtcNow;
                await taskRepository.UpdateAsync(task, cancellationToken).ConfigureAwait(false);

                _logger.LogDebug("Task {Id} moved to pending status, waiting for executor", taskId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing task: {Id}", taskId);
            }
        }
    }

    private async Task<Domain.Entities.TaskConfiguration> GetConfigurationAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var configRepository = scope.ServiceProvider.GetRequiredService<IRepository<Domain.Entities.TaskConfiguration>>();

        var configs = await configRepository.GetAllAsync(cancellationToken).ConfigureAwait(false);
        return configs.FirstOrDefault() ?? new Domain.Entities.TaskConfiguration
        {
            MaxConcurrentTasks = 2,
            PollingIntervalSeconds = 60
        };
    }
}
