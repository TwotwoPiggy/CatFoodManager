using CatFoodManager.Application.Common;
using CatFoodManager.Application.Interfaces;
using CatFoodManager.Domain.Entities;
using CatFoodManager.Domain.Enums;
using CatFoodManager.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace CatFoodManager.Application.Services;

public class TaskService : ITaskService
{
    private readonly IRepository<TaskItem> _taskRepository;
    private readonly IRepository<TaskConfiguration> _configRepository;
    private readonly ITaskScheduler _taskScheduler;
    private readonly ILogger<TaskService> _logger;
    private readonly INotificationService? _notificationService;

    public TaskService(
        IRepository<TaskItem> taskRepository,
        IRepository<TaskConfiguration> configRepository,
        ITaskScheduler taskScheduler,
        ILogger<TaskService> logger,
        INotificationService? notificationService = null)
    {
        _taskRepository = taskRepository;
        _configRepository = configRepository;
        _taskScheduler = taskScheduler;
        _logger = logger;
        _notificationService = notificationService;
    }

    public async Task<TaskItem> CreateAsync(TaskType type, string name, string parameters, string? description = null, int priority = 0, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Creating task: {Name}, Type: {Type}", name, type);

        var task = new TaskItem
        {
            Name = name,
            Type = type,
            Status = Domain.Enums.TaskStatus.Pending,
            Parameters = parameters,
            Description = description,
            Priority = priority,
            MaxRetries = 3,
            RetryCount = 0,
            CreatedAt = DateTimeOffset.UtcNow
        };

        await _taskRepository.AddAsync(task, cancellationToken).ConfigureAwait(false);

        await _taskScheduler.EnqueueAsync(task.Id, cancellationToken).ConfigureAwait(false);

        if (_notificationService != null)
        {
            await _notificationService.PublishTaskStatusChangedAsync(
                task.Id,
                task.Name,
                (int)task.Type,
                0,
                (int)Domain.Enums.TaskStatus.Pending,
                cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        _logger.LogInformation("Task created with ID: {Id}", task.Id);
        return task;
    }

    public async Task<TaskItem?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        return await _taskRepository.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
    }

    public async Task<PagedResult<TaskItem>> GetListAsync(int page, int pageSize, Domain.Enums.TaskStatus? status = null, TaskType? type = null, CancellationToken cancellationToken = default)
    {
        var allTasks = await _taskRepository.GetAllAsync(cancellationToken).ConfigureAwait(false);

        var filtered = allTasks.AsEnumerable();

        if (status.HasValue)
        {
            filtered = filtered.Where(t => t.Status == status.Value);
        }

        if (type.HasValue)
        {
            filtered = filtered.Where(t => t.Type == type.Value);
        }

        var ordered = filtered.OrderByDescending(t => t.CreatedAt).ToList();

        var total = ordered.Count;
        var items = ordered.Skip((page - 1) * pageSize).Take(pageSize).ToList();

        return new PagedResult<TaskItem>
        {
            Items = items,
            TotalCount = total,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<IReadOnlyList<TaskItem>> GetPendingTasksAsync(CancellationToken cancellationToken = default)
    {
        return await _taskRepository.FindAsync(t => t.Status == Domain.Enums.TaskStatus.Pending || t.Status == Domain.Enums.TaskStatus.Queued, cancellationToken).ConfigureAwait(false);
    }

    public async Task<bool> CancelAsync(long id, CancellationToken cancellationToken = default)
    {
        var task = await _taskRepository.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
        if (task == null)
        {
            _logger.LogWarning("Task not found: {Id}", id);
            return false;
        }

        if (task.Status == Domain.Enums.TaskStatus.Running)
        {
            _logger.LogWarning("Cannot cancel running task: {Id}", id);
            return false;
        }

        task.Status = Domain.Enums.TaskStatus.Cancelled;
        task.UpdatedAt = DateTimeOffset.UtcNow;
        await _taskRepository.UpdateAsync(task, cancellationToken).ConfigureAwait(false);

        _logger.LogInformation("Task cancelled: {Id}", id);
        return true;
    }

    public async Task<bool> RetryAsync(long id, CancellationToken cancellationToken = default)
    {
        var task = await _taskRepository.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
        if (task == null)
        {
            _logger.LogWarning("Task not found: {Id}", id);
            return false;
        }

        if (task.Status != Domain.Enums.TaskStatus.Failed && task.Status != Domain.Enums.TaskStatus.Cancelled)
        {
            _logger.LogWarning("Cannot retry task with status: {Status}", task.Status);
            return false;
        }

        task.Status = Domain.Enums.TaskStatus.Retrying;
        task.RetryCount++;
        task.ErrorMessage = null;
        task.UpdatedAt = DateTimeOffset.UtcNow;
        await _taskRepository.UpdateAsync(task, cancellationToken).ConfigureAwait(false);

        await _taskScheduler.EnqueueAsync(task.Id, cancellationToken).ConfigureAwait(false);

        _logger.LogInformation("Task retrying: {Id}, RetryCount: {RetryCount}", id, task.RetryCount);
        return true;
    }

    public async Task<bool> DeleteAsync(long id, CancellationToken cancellationToken = default)
    {
        var task = await _taskRepository.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
        if (task == null)
        {
            _logger.LogWarning("Task not found: {Id}", id);
            return false;
        }

        if (task.Status == Domain.Enums.TaskStatus.Running)
        {
            _logger.LogWarning("Cannot delete running task: {Id}", id);
            return false;
        }

        await _taskRepository.DeleteAsync(id, cancellationToken).ConfigureAwait(false);

        _logger.LogInformation("Task deleted: {Id}", id);
        return true;
    }

    public async Task<bool> TerminateAsync(long id, CancellationToken cancellationToken = default)
    {
        var task = await _taskRepository.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
        if (task == null)
        {
            _logger.LogWarning("Task not found: {Id}", id);
            return false;
        }

        if (task.Status != Domain.Enums.TaskStatus.Running)
        {
            _logger.LogWarning("Cannot terminate task with status: {Status}", task.Status);
            return false;
        }

        task.Status = Domain.Enums.TaskStatus.Cancelled;
        task.ErrorMessage = "Task was terminated by user";
        task.UpdatedAt = DateTimeOffset.UtcNow;
        await _taskRepository.UpdateAsync(task, cancellationToken).ConfigureAwait(false);

        _logger.LogInformation("Task terminated: {Id}", id);
        return true;
    }

    public async Task UpdateStatusAsync(long id, Domain.Enums.TaskStatus status, string? result = null, string? errorMessage = null, CancellationToken cancellationToken = default)
    {
        var task = await _taskRepository.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
        if (task == null)
        {
            _logger.LogWarning("Task not found: {Id}", id);
            return;
        }

        var oldStatus = (int)task.Status;

        task.Status = status;
        task.Result = result;
        task.ErrorMessage = errorMessage;
        task.UpdatedAt = DateTimeOffset.UtcNow;

        if (status == Domain.Enums.TaskStatus.Running)
        {
            task.StartedAt = DateTimeOffset.UtcNow;
        }
        else if (status == Domain.Enums.TaskStatus.Completed || status == Domain.Enums.TaskStatus.Failed)
        {
            task.CompletedAt = DateTimeOffset.UtcNow;
        }

        await _taskRepository.UpdateAsync(task, cancellationToken).ConfigureAwait(false);
        _logger.LogInformation("Task status updated: {Id}, Status: {Status}", id, status);

        if (_notificationService != null && oldStatus != (int)status)
        {
            await _notificationService.PublishTaskStatusChangedAsync(
                task.Id,
                task.Name ?? $"Task {task.Id}",
                (int)task.Type,
                oldStatus,
                (int)status,
                result,
                errorMessage,
                cancellationToken).ConfigureAwait(false);
        }
    }

    public async Task<TaskConfiguration> GetConfigurationAsync(CancellationToken cancellationToken = default)
    {
        var configs = await _configRepository.GetAllAsync(cancellationToken).ConfigureAwait(false);
        var config = configs.FirstOrDefault();

        if (config == null)
        {
            config = new TaskConfiguration
            {
                Name = "Default",
                MaxConcurrentTasks = 2,
                PollingIntervalSeconds = 60,
                EnableScheduling = false,
                CreatedAt = DateTimeOffset.UtcNow
            };
            await _configRepository.AddAsync(config, cancellationToken).ConfigureAwait(false);
        }

        return config;
    }

    public async Task<TaskConfiguration> UpdateConfigurationAsync(TaskConfiguration configuration, CancellationToken cancellationToken = default)
    {
        var existingConfig = await GetConfigurationAsync(cancellationToken).ConfigureAwait(false);

        existingConfig.MaxConcurrentTasks = configuration.MaxConcurrentTasks;
        existingConfig.PollingIntervalSeconds = configuration.PollingIntervalSeconds;
        existingConfig.EnableScheduling = configuration.EnableScheduling;
        existingConfig.DefaultSchedule = configuration.DefaultSchedule;
        existingConfig.UpdatedAt = DateTimeOffset.UtcNow;

        await _configRepository.UpdateAsync(existingConfig, cancellationToken).ConfigureAwait(false);

        _logger.LogInformation("Task configuration updated");
        return existingConfig;
    }
}
