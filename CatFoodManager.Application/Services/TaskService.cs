using CatFoodManager.Application.Common;
using CatFoodManager.Application.Interfaces;
using CatFoodManager.Domain.Entities;
using CatFoodManager.Domain.Enums;
using CatFoodManager.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace CatFoodManager.Application.Services;

/// <summary>
/// 任务服务类，提供任务的增删改查和管理功能。
/// Task service class, providing task CRUD and management functionality.
/// </summary>
public class TaskService : ITaskService
{
    private readonly IRepository<TaskItem> _taskRepository;
    private readonly IRepository<TaskConfiguration> _configRepository;
    private readonly ITaskScheduler _taskScheduler;
    private readonly ILogger<TaskService> _logger;
    private readonly INotificationService? _notificationService;

    /// <summary>
    /// 构造函数。
    /// Constructor.
    /// </summary>
    /// <param name="taskRepository">任务仓储实例 / Task repository instance</param>
    /// <param name="configRepository">配置仓储实例 / Configuration repository instance</param>
    /// <param name="taskScheduler">任务调度器 / Task scheduler</param>
    /// <param name="logger">日志记录器 / Logger</param>
    /// <param name="notificationService">通知服务（可选）/ Notification service (optional)</param>
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

    /// <summary>
    /// 创建任务。
    /// Creates a task.
    /// </summary>
    /// <param name="type">任务类型 / Task type</param>
    /// <param name="name">任务名称 / Task name</param>
    /// <param name="parameters">任务参数 / Task parameters</param>
    /// <param name="description">任务描述 / Task description</param>
    /// <param name="priority">优先级 / Priority</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    /// <returns>创建的任务实体 / Created task entity</returns>
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

    /// <summary>
    /// 根据ID获取任务。
    /// Gets a task by ID.
    /// </summary>
    /// <param name="id">任务ID / Task ID</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    /// <returns>任务实体或null / Task entity or null</returns>
    public async Task<TaskItem?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        return await _taskRepository.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// 分页获取任务列表。
    /// Gets a paged list of tasks.
    /// </summary>
    /// <param name="page">页码 / Page number</param>
    /// <param name="pageSize">每页大小 / Page size</param>
    /// <param name="status">任务状态过滤 / Task status filter</param>
    /// <param name="type">任务类型过滤 / Task type filter</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    /// <returns>分页结果 / Paged result</returns>
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

    /// <summary>
    /// 获取待处理的任务列表。
    /// Gets the list of pending tasks.
    /// </summary>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    /// <returns>待处理任务列表 / List of pending tasks</returns>
    public async Task<IReadOnlyList<TaskItem>> GetPendingTasksAsync(CancellationToken cancellationToken = default)
    {
        return await _taskRepository.FindAsync(t => t.Status == Domain.Enums.TaskStatus.Pending || t.Status == Domain.Enums.TaskStatus.Queued, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// 取消任务。
    /// Cancels a task.
    /// </summary>
    /// <param name="id">任务ID / Task ID</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    /// <returns>是否成功 / Whether successful</returns>
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

    /// <summary>
    /// 重试任务。
    /// Retries a task.
    /// </summary>
    /// <param name="id">任务ID / Task ID</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    /// <returns>是否成功 / Whether successful</returns>
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

    /// <summary>
    /// 删除任务。
    /// Deletes a task.
    /// </summary>
    /// <param name="id">任务ID / Task ID</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    /// <returns>是否成功 / Whether successful</returns>
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

    /// <summary>
    /// 终止任务。
    /// Terminates a task.
    /// </summary>
    /// <param name="id">任务ID / Task ID</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    /// <returns>是否成功 / Whether successful</returns>
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

    /// <summary>
    /// 更新任务状态。
    /// Updates task status.
    /// </summary>
    /// <param name="id">任务ID / Task ID</param>
    /// <param name="status">新状态 / New status</param>
    /// <param name="result">执行结果 / Execution result</param>
    /// <param name="errorMessage">错误信息 / Error message</param>
    /// <param name="responseId">响应ID / Response ID</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    public async Task UpdateStatusAsync(long id, Domain.Enums.TaskStatus status, string? result = null, string? errorMessage = null, string? responseId = null, CancellationToken cancellationToken = default)
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
        task.ResponseId = responseId;
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

    /// <summary>
    /// 获取任务配置。
    /// Gets task configuration.
    /// </summary>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    /// <returns>任务配置 / Task configuration</returns>
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

    /// <summary>
    /// 更新任务配置。
    /// Updates task configuration.
    /// </summary>
    /// <param name="configuration">任务配置 / Task configuration</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    /// <returns>更新后的任务配置 / Updated task configuration</returns>
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
