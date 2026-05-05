using CatFoodManager.Application.Common;
using CatFoodManager.Application.Interfaces;
using CatFoodManager.Domain.Enums;
using CatFoodManager.Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CatFoodManager.Infrastructure.Services;

/// <summary>
/// 任务后台服务，提供任务调度和执行的后台服务�?/// Task background service, providing background service for task scheduling and execution.
/// </summary>
public class TaskBackgroundService : BackgroundService, IBackgroundServiceControl
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<TaskBackgroundService> _logger;

    private int _pollingIntervalSeconds = 60;
    private int _maxConcurrentTasks = 2;

    private volatile bool _isRunning;
    private volatile bool _isPaused;
    private DateTimeOffset? _startedAt;
    private DateTimeOffset? _lastActivityAt;

    private CancellationTokenSource? _internalCts;
    private CancellationTokenSource? _linkedCts;
    private Task? _executionTask;

    /// <summary>
    /// 构造函数�?    /// Constructor.
    /// </summary>
    /// <param name="serviceProvider">服务提供�?/ Service provider</param>
    /// <param name="logger">日志记录�?/ Logger</param>
    public TaskBackgroundService(
        IServiceProvider serviceProvider,
        ILogger<TaskBackgroundService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    /// <summary>
    /// 执行后台服务�?    /// Executes the background service.
    /// </summary>
    /// <param name="stoppingToken">停止令牌 / Stopping token</param>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Task background service starting...");

        await InitializeAsync(stoppingToken).ConfigureAwait(false);

        _linkedCts = CancellationTokenSource.CreateLinkedTokenSource(stoppingToken);
        _isRunning = true;
        _startedAt = DateTimeOffset.UtcNow;
        _lastActivityAt = DateTimeOffset.UtcNow;

        using (var initScope = _serviceProvider.CreateScope())
        {
            var taskScheduler = initScope.ServiceProvider.GetService<ITaskScheduler>();
            if (taskScheduler != null)
            {
                await taskScheduler.StartAsync(stoppingToken).ConfigureAwait(false);
            }
        }

        while (!stoppingToken.IsCancellationRequested && !_linkedCts.Token.IsCancellationRequested)
        {
            try
            {
                if (!_isPaused)
                {
                    await ProcessPendingTasksAsync(_linkedCts.Token).ConfigureAwait(false);
                    _lastActivityAt = DateTimeOffset.UtcNow;
                }

                await Task.Delay(TimeSpan.FromSeconds(_pollingIntervalSeconds), _linkedCts.Token).ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in task background service");
                await Task.Delay(TimeSpan.FromSeconds(10), _linkedCts.Token).ConfigureAwait(false);
            }
        }

        using (var stopScope = _serviceProvider.CreateScope())
        {
            var taskScheduler = stopScope.ServiceProvider.GetService<ITaskScheduler>();
            if (taskScheduler != null)
            {
                await taskScheduler.StopAsync(stoppingToken).ConfigureAwait(false);
            }
        }

        _isRunning = false;
        _logger.LogInformation("Task background service stopped");
    }

    /// <summary>
    /// 初始化服务配置�?    /// Initializes service configuration.
    /// </summary>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    private async Task InitializeAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();

        var configRepository = scope.ServiceProvider.GetService<IRepository<Domain.Entities.TaskConfiguration>>();

        if (configRepository != null)
        {
            var configs = await configRepository.GetAllAsync(cancellationToken).ConfigureAwait(false);
            var config = configs.FirstOrDefault();
            if (config != null)
            {
                _pollingIntervalSeconds = config.PollingIntervalSeconds;
                _maxConcurrentTasks = config.MaxConcurrentTasks;
            }
        }

        _logger.LogInformation("Task background service initialized with polling interval: {Interval}s, max concurrent: {Max}",
            _pollingIntervalSeconds, _maxConcurrentTasks);
    }

    /// <summary>
    /// 处理待执行的任务�?    /// Processes pending tasks.
    /// </summary>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    private async Task ProcessPendingTasksAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();

        var taskRepository = scope.ServiceProvider.GetService<IRepository<Domain.Entities.TaskItem>>();
        var taskExecutor = scope.ServiceProvider.GetService<ITaskExecutor>();

        if (taskRepository == null || taskExecutor == null)
        {
            return;
        }

        var pendingTasks = await taskRepository.FindAsync(
            t => t.Status == Domain.Enums.TaskStatus.Pending || t.Status == Domain.Enums.TaskStatus.Queued,
            cancellationToken).ConfigureAwait(false);

        var runningCount = await taskExecutor.GetRunningCountAsync(cancellationToken).ConfigureAwait(false);
        var availableSlots = Math.Max(0, _maxConcurrentTasks - runningCount);

        if (availableSlots <= 0)
        {
            return;
        }

        var tasksToExecute = pendingTasks
            .OrderByDescending(t => t.Priority)
            .ThenBy(t => t.CreatedAt)
            .Take(availableSlots)
            .ToList();

        foreach (var task in tasksToExecute)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                break;
            }

            _ = taskExecutor.ExecuteAsync(task.Id, cancellationToken);
        }
    }

    /// <summary>
    /// 检查服务是否正在运行�?    /// Checks if the service is running.
    /// </summary>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    /// <returns>是否正在运行 / Whether running</returns>
    public Task<bool> IsRunningAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_isRunning);
    }

    /// <summary>
    /// 检查服务是否已暂停�?    /// Checks if the service is paused.
    /// </summary>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    /// <returns>是否已暂�?/ Whether paused</returns>
    public Task<bool> IsPausedAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_isPaused);
    }

    /// <summary>
    /// 启动服务�?    /// Starts the service.
    /// </summary>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    public Task StartServiceAsync(CancellationToken cancellationToken = default)
    {
        if (_isRunning)
        {
            _logger.LogWarning("Task background service is already running");
            return Task.CompletedTask;
        }

        _logger.LogInformation("Manually starting task background service");

        _internalCts?.Cancel();
        _internalCts?.Dispose();
        _internalCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        _isPaused = false;
        _isRunning = true;
        _startedAt = DateTimeOffset.UtcNow;
        _lastActivityAt = DateTimeOffset.UtcNow;

        _executionTask = ExecuteAsync(_internalCts.Token);

        return Task.CompletedTask;
    }

    /// <summary>
    /// 暂停服务�?    /// Pauses the service.
    /// </summary>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    public Task PauseAsync(CancellationToken cancellationToken = default)
    {
        if (!_isRunning)
        {
            _logger.LogWarning("Cannot pause: task background service is not running");
            return Task.CompletedTask;
        }

        if (_isPaused)
        {
            _logger.LogWarning("Task background service is already paused");
            return Task.CompletedTask;
        }

        _logger.LogInformation("Pausing task background service");
        _isPaused = true;
        _lastActivityAt = DateTimeOffset.UtcNow;

        return Task.CompletedTask;
    }

    /// <summary>
    /// 恢复服务�?    /// Resumes the service.
    /// </summary>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    public Task ResumeAsync(CancellationToken cancellationToken = default)
    {
        if (!_isRunning)
        {
            _logger.LogWarning("Cannot resume: task background service is not running");
            return Task.CompletedTask;
        }

        if (!_isPaused)
        {
            _logger.LogWarning("Task background service is not paused");
            return Task.CompletedTask;
        }

        _logger.LogInformation("Resuming task background service");
        _isPaused = false;
        _lastActivityAt = DateTimeOffset.UtcNow;

        return Task.CompletedTask;
    }

    /// <summary>
    /// 停止服务�?    /// Stops the service.
    /// </summary>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    public async Task StopServiceAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Stopping task background service");

        _linkedCts?.Cancel();
        _isRunning = false;
        _isPaused = false;

        if (_executionTask != null)
        {
            try
            {
                await _executionTask.ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                // 正常的取消操作，忽略 / Normal cancellation, ignore
            }
        }

        _linkedCts?.Dispose();
        _linkedCts = null;
        _executionTask = null;
    }

    /// <summary>
    /// 获取服务状态信息�?    /// Gets the service status information.
    /// </summary>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    /// <returns>服务状态信�?/ Service status information</returns>
    public async Task<ServiceStatusInfo> GetStatusAsync(CancellationToken cancellationToken = default)
    {
        int runningTaskCount = 0;
        int queueLength = 0;

        using var scope = _serviceProvider.CreateScope();
        var taskExecutor = scope.ServiceProvider.GetService<ITaskExecutor>();
        var taskScheduler = scope.ServiceProvider.GetService<ITaskScheduler>();

        if (taskExecutor != null)
        {
            runningTaskCount = await taskExecutor.GetRunningCountAsync(cancellationToken).ConfigureAwait(false);
        }

        if (taskScheduler != null)
        {
            queueLength = await taskScheduler.GetQueueLengthAsync(cancellationToken).ConfigureAwait(false);
        }

        return new ServiceStatusInfo(
            IsRunning: _isRunning,
            IsPaused: _isPaused,
            RunningTaskCount: runningTaskCount,
            QueueLength: queueLength,
            StartedAt: _startedAt,
            LastActivityAt: _lastActivityAt
        );
    }
}
