using System.Collections.Concurrent;
using CatFoodManager.Application.Interfaces;
using CatFoodManager.Domain.Enums;
using CatFoodManager.Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CatFoodManager.Application.Services;

/// <summary>
/// 任务执行器类，提供任务执行和管理功能。
/// Task executor class, providing task execution and management functionality.
/// </summary>
public class TaskExecutor : ITaskExecutor
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<TaskExecutor> _logger;

    private readonly ConcurrentDictionary<long, CancellationTokenSource> _runningTasks = new();
    private readonly SemaphoreSlim _concurrencySemaphore;
    private readonly int _maxConcurrentTasks;

    /// <summary>
    /// 构造函数。
    /// Constructor.
    /// </summary>
    /// <param name="serviceProvider">服务提供者 / Service provider</param>
    /// <param name="logger">日志记录器 / Logger</param>
    public TaskExecutor(
        IServiceProvider serviceProvider,
        ILogger<TaskExecutor> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;

        var config = GetConfigurationAsync().GetAwaiter().GetResult();
        _maxConcurrentTasks = config.MaxConcurrentTasks;
        _concurrencySemaphore = new SemaphoreSlim(_maxConcurrentTasks, _maxConcurrentTasks);
    }

    /// <summary>
    /// 执行指定任务。
    /// Executes the specified task.
    /// </summary>
    /// <param name="taskId">任务ID / Task ID</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    public async Task ExecuteAsync(long taskId, CancellationToken cancellationToken = default)
    {
        using var scope = _serviceProvider.CreateScope();
        var taskRepository = scope.ServiceProvider.GetRequiredService<IRepository<Domain.Entities.TaskItem>>();
        var taskHandlers = scope.ServiceProvider.GetServices<ITaskHandler>();
        var taskService = scope.ServiceProvider.GetRequiredService<ITaskService>();

        var task = await taskRepository.GetByIdAsync(taskId, cancellationToken).ConfigureAwait(false);
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

        var handler = taskHandlers.FirstOrDefault(h => h.TaskType == task.Type);
        if (handler == null)
        {
            _logger.LogError("No handler found for task type: {Type}", task.Type);
            await taskService.UpdateStatusAsync(taskId, Domain.Enums.TaskStatus.Failed, errorMessage: $"No handler found for task type: {task.Type}", cancellationToken: cancellationToken).ConfigureAwait(false);
            return;
        }

        await _concurrencySemaphore.WaitAsync(cancellationToken).ConfigureAwait(false);

        var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        _runningTasks[taskId] = cts;

        try
        {
            await taskService.UpdateStatusAsync(taskId, Domain.Enums.TaskStatus.Running, cancellationToken: cancellationToken).ConfigureAwait(false);

            _logger.LogInformation("Executing task: {Id}, Type: {Type}", taskId, task.Type);

            var result = await handler.HandleAsync(task.Parameters, cts.Token).ConfigureAwait(false);

            if (result.Success)
            {
                await taskService.UpdateStatusAsync(taskId, Domain.Enums.TaskStatus.Completed, result: result.Result, responseId: result.ResponseId, cancellationToken: cancellationToken).ConfigureAwait(false);
                _logger.LogInformation("Task completed: {Id}", taskId);
            }
            else
            {
                await taskService.UpdateStatusAsync(taskId, Domain.Enums.TaskStatus.Failed, errorMessage: result.ErrorMessage, responseId: result.ResponseId, cancellationToken: cancellationToken).ConfigureAwait(false);
                _logger.LogWarning("Task failed: {Id}, Error: {Error}", taskId, result.ErrorMessage);
            }
        }
        catch (OperationCanceledException)
        {
            await taskService.UpdateStatusAsync(taskId, Domain.Enums.TaskStatus.Cancelled, errorMessage: "Task was cancelled", cancellationToken: cancellationToken).ConfigureAwait(false);
            _logger.LogInformation("Task cancelled: {Id}", taskId);
        }
        catch (Exception ex)
        {
            await taskService.UpdateStatusAsync(taskId, Domain.Enums.TaskStatus.Failed, errorMessage: ex.Message, cancellationToken: cancellationToken).ConfigureAwait(false);
            _logger.LogError(ex, "Task execution failed: {Id}", taskId);
        }
        finally
        {
            _runningTasks.TryRemove(taskId, out _);
            _concurrencySemaphore.Release();
        }
    }

    /// <summary>
    /// 检查任务是否正在运行。
    /// Checks if a task is running.
    /// </summary>
    /// <param name="taskId">任务ID / Task ID</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    /// <returns>是否正在运行 / Whether running</returns>
    public Task<bool> IsRunningAsync(long taskId, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_runningTasks.ContainsKey(taskId));
    }

    /// <summary>
    /// 终止指定任务。
    /// Terminates the specified task.
    /// </summary>
    /// <param name="taskId">任务ID / Task ID</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    public async Task TerminateAsync(long taskId, CancellationToken cancellationToken = default)
    {
        if (_runningTasks.TryGetValue(taskId, out var cts))
        {
            await cts.CancelAsync().ConfigureAwait(false);
            _logger.LogInformation("Task termination requested: {Id}", taskId);
        }
    }

    /// <summary>
    /// 获取正在运行的任务数量。
    /// Gets the count of running tasks.
    /// </summary>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    /// <returns>正在运行的任务数量 / Count of running tasks</returns>
    public Task<int> GetRunningCountAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_runningTasks.Count);
    }

    /// <summary>
    /// 获取任务配置。
    /// Gets task configuration.
    /// </summary>
    /// <returns>任务配置 / Task configuration</returns>
    private async Task<Domain.Entities.TaskConfiguration> GetConfigurationAsync()
    {
        using var scope = _serviceProvider.CreateScope();
        var configRepository = scope.ServiceProvider.GetRequiredService<IRepository<Domain.Entities.TaskConfiguration>>();

        var configs = await configRepository.GetAllAsync().ConfigureAwait(false);
        return configs.FirstOrDefault() ?? new Domain.Entities.TaskConfiguration
        {
            MaxConcurrentTasks = 2,
            PollingIntervalSeconds = 60
        };
    }
}
