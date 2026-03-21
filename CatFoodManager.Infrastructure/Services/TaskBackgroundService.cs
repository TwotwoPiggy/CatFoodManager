using CatFoodManager.Application.Interfaces;
using CatFoodManager.Domain.Enums;
using CatFoodManager.Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CatFoodManager.Infrastructure.Services;

public class TaskBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<TaskBackgroundService> _logger;

    private ITaskScheduler? _taskScheduler;
    private ITaskExecutor? _taskExecutor;
    private IRepository<Domain.Entities.TaskItem>? _taskRepository;
    private IRepository<Domain.Entities.TaskConfiguration>? _configRepository;

    private int _pollingIntervalSeconds = 60;
    private int _maxConcurrentTasks = 2;

    public TaskBackgroundService(
        IServiceProvider serviceProvider,
        ILogger<TaskBackgroundService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Task background service starting...");

        await InitializeAsync(stoppingToken).ConfigureAwait(false);

        if (_taskScheduler != null)
        {
            await _taskScheduler.StartAsync(stoppingToken).ConfigureAwait(false);
        }

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessPendingTasksAsync(stoppingToken).ConfigureAwait(false);
                await Task.Delay(TimeSpan.FromSeconds(_pollingIntervalSeconds), stoppingToken).ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in task background service");
                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken).ConfigureAwait(false);
            }
        }

        if (_taskScheduler != null)
        {
            await _taskScheduler.StopAsync(stoppingToken).ConfigureAwait(false);
        }

        _logger.LogInformation("Task background service stopped");
    }

    private async Task InitializeAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();

        _taskScheduler = scope.ServiceProvider.GetService<ITaskScheduler>();
        _taskExecutor = scope.ServiceProvider.GetService<ITaskExecutor>();
        _taskRepository = scope.ServiceProvider.GetService<IRepository<Domain.Entities.TaskItem>>();
        _configRepository = scope.ServiceProvider.GetService<IRepository<Domain.Entities.TaskConfiguration>>();

        if (_configRepository != null)
        {
            var configs = await _configRepository.GetAllAsync(cancellationToken).ConfigureAwait(false);
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

    private async Task ProcessPendingTasksAsync(CancellationToken cancellationToken)
    {
        if (_taskRepository == null || _taskExecutor == null)
        {
            return;
        }

        var pendingTasks = await _taskRepository.FindAsync(
            t => t.Status == Domain.Enums.TaskStatus.Pending || t.Status == Domain.Enums.TaskStatus.Queued,
            cancellationToken).ConfigureAwait(false);

        var runningCount = await _taskExecutor.GetRunningCountAsync(cancellationToken).ConfigureAwait(false);
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

            _ = _taskExecutor.ExecuteAsync(task.Id, cancellationToken);
        }
    }
}
