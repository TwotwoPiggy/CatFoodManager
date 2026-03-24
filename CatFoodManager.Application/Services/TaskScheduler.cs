using System.Threading.Channels;
using CatFoodManager.Application.Interfaces;
using CatFoodManager.Domain.Enums;
using CatFoodManager.Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CatFoodManager.Application.Services;

/// <summary>
/// 任务调度器类，提供任务调度和队列管理功能。
/// Task scheduler class, providing task scheduling and queue management functionality.
/// </summary>
public class TaskScheduler : ITaskScheduler
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<TaskScheduler> _logger;

    /// <summary>
    /// 最大队列容量。
    /// Maximum queue capacity.
    /// </summary>
    private const int MaxQueueCapacity = 10000;

    private readonly Channel<long> _taskQueue;
    private CancellationTokenSource? _cancellationTokenSource;
    private Task? _processingTask;
    private volatile bool _isRunning;

    /// <summary>
    /// 构造函数。
    /// Constructor.
    /// </summary>
    /// <param name="serviceProvider">服务提供者 / Service provider</param>
    /// <param name="logger">日志记录器 / Logger</param>
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

    /// <summary>
    /// 启动调度器。
    /// Starts the scheduler.
    /// </summary>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
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

    /// <summary>
    /// 停止调度器。
    /// Stops the scheduler.
    /// </summary>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
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
                // 正常的取消操作，忽略 / Normal cancellation, ignore
            }
        }

        _logger.LogInformation("Task scheduler stopped");
    }

    /// <summary>
    /// 将任务加入队列。
    /// Enqueues a task.
    /// </summary>
    /// <param name="taskId">任务ID / Task ID</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
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

    /// <summary>
    /// 获取队列长度。
    /// Gets the queue length.
    /// </summary>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    /// <returns>队列长度 / Queue length</returns>
    public Task<int> GetQueueLengthAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_taskQueue.Reader.Count);
    }

    /// <summary>
    /// 检查调度器是否正在运行。
    /// Checks if the scheduler is running.
    /// </summary>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    /// <returns>是否正在运行 / Whether running</returns>
    public Task<bool> IsRunningAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_isRunning);
    }

    /// <summary>
    /// 处理队列中的任务。
    /// Processes tasks in the queue.
    /// </summary>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
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

    /// <summary>
    /// 获取任务配置。
    /// Gets task configuration.
    /// </summary>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    /// <returns>任务配置 / Task configuration</returns>
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
