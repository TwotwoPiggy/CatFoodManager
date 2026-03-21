using CatFoodManager.Application.Common;
using CatFoodManager.Domain.Entities;
using CatFoodManager.Domain.Enums;

namespace CatFoodManager.Application.Interfaces;

public interface ITaskService
{
    Task<TaskItem> CreateAsync(TaskType type, string name, string parameters, string? description = null, int priority = 0, CancellationToken cancellationToken = default);
    Task<TaskItem?> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<PagedResult<TaskItem>> GetListAsync(int page, int pageSize, Domain.Enums.TaskStatus? status = null, TaskType? type = null, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TaskItem>> GetPendingTasksAsync(CancellationToken cancellationToken = default);
    Task<bool> CancelAsync(long id, CancellationToken cancellationToken = default);
    Task<bool> RetryAsync(long id, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(long id, CancellationToken cancellationToken = default);
    Task<bool> TerminateAsync(long id, CancellationToken cancellationToken = default);
    Task UpdateStatusAsync(long id, Domain.Enums.TaskStatus status, string? result = null, string? errorMessage = null, CancellationToken cancellationToken = default);
    Task<TaskConfiguration> GetConfigurationAsync(CancellationToken cancellationToken = default);
    Task<TaskConfiguration> UpdateConfigurationAsync(TaskConfiguration configuration, CancellationToken cancellationToken = default);
}
