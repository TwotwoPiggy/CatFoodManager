using System.Collections.Concurrent;
using CatFoodManager.Application.Interfaces;
using CatFoodManager.Domain.Enums;
using CatFoodManager.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace CatFoodManager.Application.Services;

public class TaskExecutor : ITaskExecutor
{
    private readonly IRepository<Domain.Entities.TaskItem> _taskRepository;
    private readonly IRepository<Domain.Entities.TaskConfiguration> _configRepository;
    private readonly IEnumerable<ITaskHandler> _taskHandlers;
    private readonly ITaskService _taskService;
    private readonly ILogger<TaskExecutor> _logger;

    private readonly ConcurrentDictionary<long, CancellationTokenSource> _runningTasks = new();
    private readonly SemaphoreSlim? _concurrencySemaphore;

    public TaskExecutor(
        IRepository<Domain.Entities.TaskItem> taskRepository,
        IRepository<Domain.Entities.TaskConfiguration> configRepository,
        IEnumerable<ITaskHandler> taskHandlers,
        ITaskService taskService,
        ILogger<TaskExecutor> logger)
    {
        _taskRepository = taskRepository;
        _configRepository = configRepository;
        _taskHandlers = taskHandlers;
        _taskService = taskService;
        _logger = logger;

        var config = GetConfigurationAsync().GetAwaiter().GetResult();
        _concurrencySemaphore = new SemaphoreSlim(config.MaxConcurrentTasks, config.MaxConcurrentTasks);
    }

    public async Task ExecuteAsync(long taskId, CancellationToken cancellationToken = default)
    {
        var task = await _taskRepository.GetByIdAsync(taskId, cancellationToken).ConfigureAwait(false);
        if (task == null)
        {
            _logger.LogWarning("Task not found: {Id}", taskId);
            return;
        }

        if (task.Status == Domain.Enums.TaskStatus.Running)
        {
            _logger.LogWarning("Task is already running: {Id}", taskId);
            return;
        }

        var handler = _taskHandlers.FirstOrDefault(h => h.TaskType == task.Type);
        if (handler == null)
        {
            _logger.LogError("No handler found for task type: {Type}", task.Type);
            await _taskService.UpdateStatusAsync(taskId, Domain.Enums.TaskStatus.Failed, errorMessage: $"No handler found for task type: {task.Type}", cancellationToken: cancellationToken).ConfigureAwait(false);
            return;
        }

        await _concurrencySemaphore!.WaitAsync(cancellationToken).ConfigureAwait(false);

        var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        _runningTasks[taskId] = cts;

        try
        {
            await _taskService.UpdateStatusAsync(taskId, Domain.Enums.TaskStatus.Running, cancellationToken: cancellationToken).ConfigureAwait(false);

            _logger.LogInformation("Executing task: {Id}, Type: {Type}", taskId, task.Type);

            var result = await handler.HandleAsync(task.Parameters, cts.Token).ConfigureAwait(false);

            if (result.Success)
            {
                await _taskService.UpdateStatusAsync(taskId, Domain.Enums.TaskStatus.Completed, result: result.Result, cancellationToken: cancellationToken).ConfigureAwait(false);
                _logger.LogInformation("Task completed: {Id}", taskId);
            }
            else
            {
                await _taskService.UpdateStatusAsync(taskId, Domain.Enums.TaskStatus.Failed, errorMessage: result.ErrorMessage, cancellationToken: cancellationToken).ConfigureAwait(false);
                _logger.LogWarning("Task failed: {Id}, Error: {Error}", taskId, result.ErrorMessage);
            }
        }
        catch (OperationCanceledException)
        {
            await _taskService.UpdateStatusAsync(taskId, Domain.Enums.TaskStatus.Cancelled, errorMessage: "Task was cancelled", cancellationToken: cancellationToken).ConfigureAwait(false);
            _logger.LogInformation("Task cancelled: {Id}", taskId);
        }
        catch (Exception ex)
        {
            await _taskService.UpdateStatusAsync(taskId, Domain.Enums.TaskStatus.Failed, errorMessage: ex.Message, cancellationToken: cancellationToken).ConfigureAwait(false);
            _logger.LogError(ex, "Task execution failed: {Id}", taskId);
        }
        finally
        {
            _runningTasks.TryRemove(taskId, out _);
            _concurrencySemaphore.Release();
        }
    }

    public Task<bool> IsRunningAsync(long taskId, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_runningTasks.ContainsKey(taskId));
    }

    public async Task TerminateAsync(long taskId, CancellationToken cancellationToken = default)
    {
        if (_runningTasks.TryGetValue(taskId, out var cts))
        {
            await cts.CancelAsync().ConfigureAwait(false);
            _logger.LogInformation("Task termination requested: {Id}", taskId);
        }
    }

    public Task<int> GetRunningCountAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_runningTasks.Count);
    }

    private async Task<Domain.Entities.TaskConfiguration> GetConfigurationAsync()
    {
        var configs = await _configRepository.GetAllAsync().ConfigureAwait(false);
        return configs.FirstOrDefault() ?? new Domain.Entities.TaskConfiguration
        {
            MaxConcurrentTasks = 2,
            PollingIntervalSeconds = 60
        };
    }
}
