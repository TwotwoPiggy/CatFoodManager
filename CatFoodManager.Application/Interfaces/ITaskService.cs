using CatFoodManager.Application.Common;
using CatFoodManager.Domain.Entities;
using CatFoodManager.Domain.Enums;

namespace CatFoodManager.Application.Interfaces;

/// <summary>
/// 任务服务接口，提供任务的增删改查和管理功能。
/// Task service interface, providing task CRUD and management functionality.
/// </summary>
public interface ITaskService
{
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
    Task<TaskItem> CreateAsync(TaskType type, string name, string parameters, string? description = null, int priority = 0, CancellationToken cancellationToken = default);

    /// <summary>
    /// 根据ID获取任务。
    /// Gets a task by ID.
    /// </summary>
    /// <param name="id">任务ID / Task ID</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    /// <returns>任务实体或null / Task entity or null</returns>
    Task<TaskItem?> GetByIdAsync(long id, CancellationToken cancellationToken = default);

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
    Task<PagedResult<TaskItem>> GetListAsync(int page, int pageSize, Domain.Enums.TaskStatus? status = null, TaskType? type = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取待处理的任务列表。
    /// Gets the list of pending tasks.
    /// </summary>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    /// <returns>待处理任务列表 / List of pending tasks</returns>
    Task<IReadOnlyList<TaskItem>> GetPendingTasksAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 取消任务。
    /// Cancels a task.
    /// </summary>
    /// <param name="id">任务ID / Task ID</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    /// <returns>是否成功 / Whether successful</returns>
    Task<bool> CancelAsync(long id, CancellationToken cancellationToken = default);

    /// <summary>
    /// 重试任务。
    /// Retries a task.
    /// </summary>
    /// <param name="id">任务ID / Task ID</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    /// <returns>是否成功 / Whether successful</returns>
    Task<bool> RetryAsync(long id, CancellationToken cancellationToken = default);

    /// <summary>
    /// 删除任务。
    /// Deletes a task.
    /// </summary>
    /// <param name="id">任务ID / Task ID</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    /// <returns>是否成功 / Whether successful</returns>
    Task<bool> DeleteAsync(long id, CancellationToken cancellationToken = default);

    /// <summary>
    /// 终止任务。
    /// Terminates a task.
    /// </summary>
    /// <param name="id">任务ID / Task ID</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    /// <returns>是否成功 / Whether successful</returns>
    Task<bool> TerminateAsync(long id, CancellationToken cancellationToken = default);

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
    Task UpdateStatusAsync(long id, Domain.Enums.TaskStatus status, string? result = null, string? errorMessage = null, string? responseId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取任务配置。
    /// Gets task configuration.
    /// </summary>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    /// <returns>任务配置 / Task configuration</returns>
    Task<TaskConfiguration> GetConfigurationAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 更新任务配置。
    /// Updates task configuration.
    /// </summary>
    /// <param name="configuration">任务配置 / Task configuration</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    /// <returns>更新后的任务配置 / Updated task configuration</returns>
    Task<TaskConfiguration> UpdateConfigurationAsync(TaskConfiguration configuration, CancellationToken cancellationToken = default);
}
