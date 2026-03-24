using CatFoodManager.Application.Interfaces;
using CatFoodManager.Domain.Entities;
using CatFoodManager.Domain.Enums;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace CatfoodManagement.Services.Bridge
{
    public class TaskApi
    {
        private readonly IServiceProvider _serviceProvider;

        private static readonly JsonSerializerSettings _jsonSettings = new()
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            NullValueHandling = NullValueHandling.Ignore,
            DateFormatString = "yyyy-MM-ddTHH:mm:ss.fffZ"
        };

        public TaskApi(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task<string> GetTasksAsync(int page, int pageSize, int? status = null, int? type = null)
        {
            using var scope = _serviceProvider.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<ITaskService>();

            CatFoodManager.Domain.Enums.TaskStatus? taskStatus = status.HasValue ? (CatFoodManager.Domain.Enums.TaskStatus)status.Value : null;
            TaskType? taskType = type.HasValue ? (TaskType)type.Value : null;

            var result = await service.GetListAsync(page, pageSize, taskStatus, taskType);

            return JsonConvert.SerializeObject(new
            {
                Success = true,
                Data = result.Items,
                Total = result.TotalCount,
                Page = result.Page,
                PageSize = result.PageSize,
                TotalPages = result.TotalPages
            }, _jsonSettings);
        }

        public async Task<string> GetTaskByIdAsync(long id)
        {
            using var scope = _serviceProvider.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<ITaskService>();

            var task = await service.GetByIdAsync(id);

            if (task == null)
            {
                return JsonConvert.SerializeObject(new { Success = false, Message = "Task not found" }, _jsonSettings);
            }

            return JsonConvert.SerializeObject(new { Success = true, Data = task }, _jsonSettings);
        }

        public async Task<string> CreateTaskAsync(int type, string name, string parameters, string? description = null, int priority = 0)
        {
            using var scope = _serviceProvider.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<ITaskService>();

            try
            {
                var taskType = (TaskType)type;
                var task = await service.CreateAsync(taskType, name, parameters, description, priority);

                return JsonConvert.SerializeObject(new { Success = true, Data = task, Message = "Task created successfully" }, _jsonSettings);
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(new { Success = false, Message = ex.Message }, _jsonSettings);
            }
        }

        public async Task<string> CancelTaskAsync(long id)
        {
            using var scope = _serviceProvider.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<ITaskService>();

            var result = await service.CancelAsync(id);

            return JsonConvert.SerializeObject(new
            {
                Success = result,
                Message = result ? "Task cancelled successfully" : "Failed to cancel task"
            }, _jsonSettings);
        }

        public async Task<string> RetryTaskAsync(long id)
        {
            using var scope = _serviceProvider.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<ITaskService>();

            var result = await service.RetryAsync(id);

            return JsonConvert.SerializeObject(new
            {
                Success = result,
                Message = result ? "Task retry initiated" : "Failed to retry task"
            }, _jsonSettings);
        }

        public async Task<string> DeleteTaskAsync(long id)
        {
            using var scope = _serviceProvider.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<ITaskService>();

            var result = await service.DeleteAsync(id);

            return JsonConvert.SerializeObject(new
            {
                Success = result,
                Message = result ? "Task deleted successfully" : "Failed to delete task"
            }, _jsonSettings);
        }

        public async Task<string> TerminateTaskAsync(long id)
        {
            using var scope = _serviceProvider.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<ITaskService>();

            var result = await service.TerminateAsync(id);

            return JsonConvert.SerializeObject(new
            {
                Success = result,
                Message = result ? "Task terminated successfully" : "Failed to terminate task"
            }, _jsonSettings);
        }

        public async Task<string> GetTaskConfigurationAsync()
        {
            using var scope = _serviceProvider.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<ITaskService>();

            var config = await service.GetConfigurationAsync();

            return JsonConvert.SerializeObject(new { Success = true, Data = config }, _jsonSettings);
        }

        public async Task<string> UpdateTaskConfigurationAsync(string configuration)
        {
            using var scope = _serviceProvider.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<ITaskService>();

            try
            {
                var config = JsonConvert.DeserializeObject<TaskConfiguration>(configuration);
                if (config == null)
                {
                    return JsonConvert.SerializeObject(new { Success = false, Message = "Invalid configuration" }, _jsonSettings);
                }

                var result = await service.UpdateConfigurationAsync(config);

                return JsonConvert.SerializeObject(new { Success = true, Data = result, Message = "Configuration updated successfully" }, _jsonSettings);
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(new { Success = false, Message = ex.Message }, _jsonSettings);
            }
        }

        public async Task<string> GetRunningTaskCountAsync()
        {
            using var scope = _serviceProvider.CreateScope();
            var executor = scope.ServiceProvider.GetRequiredService<ITaskExecutor>();

            var count = await executor.GetRunningCountAsync();

            return JsonConvert.SerializeObject(new { Success = true, Count = count }, _jsonSettings);
        }

        public async Task<string> GetQueueLengthAsync()
        {
            using var scope = _serviceProvider.CreateScope();
            var scheduler = scope.ServiceProvider.GetRequiredService<ITaskScheduler>();

            var count = await scheduler.GetQueueLengthAsync();

            return JsonConvert.SerializeObject(new { Success = true, Count = count }, _jsonSettings);
        }
    }
}
