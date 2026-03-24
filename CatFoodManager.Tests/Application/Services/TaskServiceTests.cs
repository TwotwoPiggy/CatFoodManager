using CatFoodManager.Application.Interfaces;
using CatFoodManager.Application.Services;
using CatFoodManager.Domain.Entities;
using CatFoodManager.Domain.Enums;
using CatFoodManager.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using TaskStatus = CatFoodManager.Domain.Enums.TaskStatus;

namespace CatFoodManager.Tests.Application.Services;

public class TaskServiceTests
{
    private readonly Mock<IRepository<TaskItem>> _taskRepositoryMock;
    private readonly Mock<IRepository<TaskConfiguration>> _configRepositoryMock;
    private readonly Mock<ITaskScheduler> _schedulerMock;
    private readonly Mock<ILogger<TaskService>> _loggerMock;
    private readonly TaskService _service;

    public TaskServiceTests()
    {
        _taskRepositoryMock = new Mock<IRepository<TaskItem>>();
        _configRepositoryMock = new Mock<IRepository<TaskConfiguration>>();
        _schedulerMock = new Mock<ITaskScheduler>();
        _loggerMock = new Mock<ILogger<TaskService>>();

        _service = new TaskService(
            _taskRepositoryMock.Object,
            _configRepositoryMock.Object,
            _schedulerMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task CreateAsync_ShouldCreateTaskAndEnqueue()
    {
        var taskType = TaskType.ImageSync;
        var name = "Test Task";
        var parameters = "{\"folderPath\":\"/test\"}";
        var description = "Test description";

        _taskRepositoryMock.Setup(r => r.AddAsync(It.IsAny<TaskItem>(), default))
            .Callback<TaskItem, CancellationToken>((t, _) => t.Id = 1);

        var result = await _service.CreateAsync(taskType, name, parameters, description);

        Assert.Equal(taskType, result.Type);
        Assert.Equal(name, result.Name);
        Assert.Equal(parameters, result.Parameters);
        Assert.Equal(description, result.Description);
        Assert.Equal(TaskStatus.Pending, result.Status);

        _taskRepositoryMock.Verify(r => r.AddAsync(It.IsAny<TaskItem>(), default), Times.Once);
        _schedulerMock.Verify(s => s.EnqueueAsync(1, default), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnTask_WhenExists()
    {
        var task = new TaskItem { Id = 1, Name = "Test Task" };
        _taskRepositoryMock.Setup(r => r.GetByIdAsync(1, default))
            .ReturnsAsync(task);

        var result = await _service.GetByIdAsync(1);

        Assert.NotNull(result);
        Assert.Equal(1, result!.Id);
        Assert.Equal("Test Task", result.Name);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenNotExists()
    {
        _taskRepositoryMock.Setup(r => r.GetByIdAsync(999, default))
            .ReturnsAsync((TaskItem?)null);

        var result = await _service.GetByIdAsync(999);

        Assert.Null(result);
    }

    [Fact]
    public async Task CancelAsync_ShouldCancelTask_WhenTaskIsNotRunning()
    {
        var task = new TaskItem { Id = 1, Status = TaskStatus.Pending };
        _taskRepositoryMock.Setup(r => r.GetByIdAsync(1, default))
            .ReturnsAsync(task);

        var result = await _service.CancelAsync(1);

        Assert.True(result);
        Assert.Equal(TaskStatus.Cancelled, task.Status);
        _taskRepositoryMock.Verify(r => r.UpdateAsync(task, default), Times.Once);
    }

    [Fact]
    public async Task CancelAsync_ShouldReturnFalse_WhenTaskIsRunning()
    {
        var task = new TaskItem { Id = 1, Status = TaskStatus.Running };
        _taskRepositoryMock.Setup(r => r.GetByIdAsync(1, default))
            .ReturnsAsync(task);

        var result = await _service.CancelAsync(1);

        Assert.False(result);
        Assert.Equal(TaskStatus.Running, task.Status);
        _taskRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<TaskItem>(), default), Times.Never);
    }

    [Fact]
    public async Task RetryAsync_ShouldRetryTask_WhenTaskIsFailed()
    {
        var task = new TaskItem { Id = 1, Status = TaskStatus.Failed, RetryCount = 0 };
        _taskRepositoryMock.Setup(r => r.GetByIdAsync(1, default))
            .ReturnsAsync(task);

        var result = await _service.RetryAsync(1);

        Assert.True(result);
        Assert.Equal(TaskStatus.Retrying, task.Status);
        Assert.Equal(1, task.RetryCount);
        _schedulerMock.Verify(s => s.EnqueueAsync(1, default), Times.Once);
    }

    [Fact]
    public async Task RetryAsync_ShouldReturnFalse_WhenTaskIsNotFailed()
    {
        var task = new TaskItem { Id = 1, Status = TaskStatus.Running };
        _taskRepositoryMock.Setup(r => r.GetByIdAsync(1, default))
            .ReturnsAsync(task);

        var result = await _service.RetryAsync(1);

        Assert.False(result);
    }

    [Fact]
    public async Task DeleteAsync_ShouldDeleteTask_WhenTaskIsNotRunning()
    {
        var task = new TaskItem { Id = 1, Status = TaskStatus.Completed };
        _taskRepositoryMock.Setup(r => r.GetByIdAsync(1, default))
            .ReturnsAsync(task);

        var result = await _service.DeleteAsync(1);

        Assert.True(result);
        _taskRepositoryMock.Verify(r => r.DeleteAsync(1, default), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnFalse_WhenTaskIsRunning()
    {
        var task = new TaskItem { Id = 1, Status = TaskStatus.Running };
        _taskRepositoryMock.Setup(r => r.GetByIdAsync(1, default))
            .ReturnsAsync(task);

        var result = await _service.DeleteAsync(1);

        Assert.False(result);
        _taskRepositoryMock.Verify(r => r.DeleteAsync(It.IsAny<long>(), default), Times.Never);
    }

    [Fact]
    public async Task TerminateAsync_ShouldTerminateRunningTask()
    {
        var task = new TaskItem { Id = 1, Status = TaskStatus.Running };
        _taskRepositoryMock.Setup(r => r.GetByIdAsync(1, default))
            .ReturnsAsync(task);

        var result = await _service.TerminateAsync(1);

        Assert.True(result);
        Assert.Equal(TaskStatus.Cancelled, task.Status);
        Assert.NotNull(task.ErrorMessage);
        _taskRepositoryMock.Verify(r => r.UpdateAsync(task, default), Times.Once);
    }

    [Fact]
    public async Task TerminateAsync_ShouldReturnFalse_WhenTaskIsNotRunning()
    {
        var task = new TaskItem { Id = 1, Status = TaskStatus.Pending };
        _taskRepositoryMock.Setup(r => r.GetByIdAsync(1, default))
            .ReturnsAsync(task);

        var result = await _service.TerminateAsync(1);

        Assert.False(result);
    }

    [Fact]
    public async Task UpdateStatusAsync_ShouldUpdateTaskStatus()
    {
        var task = new TaskItem { Id = 1, Status = TaskStatus.Pending };
        _taskRepositoryMock.Setup(r => r.GetByIdAsync(1, default))
            .ReturnsAsync(task);

        await _service.UpdateStatusAsync(1, TaskStatus.Running, null, null, null);

        Assert.Equal(TaskStatus.Running, task.Status);
        Assert.NotNull(task.StartedAt);
        _taskRepositoryMock.Verify(r => r.UpdateAsync(task, default), Times.Once);
    }

    [Fact]
    public async Task UpdateStatusAsync_ShouldSetCompletedAt_WhenStatusIsCompleted()
    {
        var task = new TaskItem { Id = 1, Status = TaskStatus.Running };
        _taskRepositoryMock.Setup(r => r.GetByIdAsync(1, default))
            .ReturnsAsync(task);

        await _service.UpdateStatusAsync(1, TaskStatus.Completed, "result", null, "test-response-id");

        Assert.Equal(TaskStatus.Completed, task.Status);
        Assert.Equal("result", task.Result);
        Assert.Equal("test-response-id", task.ResponseId);
        Assert.NotNull(task.CompletedAt);
    }

    [Fact]
    public async Task GetConfigurationAsync_ShouldReturnDefault_WhenNotExists()
    {
        _configRepositoryMock.Setup(r => r.GetAllAsync(default))
            .ReturnsAsync(new List<TaskConfiguration>());

        var result = await _service.GetConfigurationAsync();

        Assert.NotNull(result);
        Assert.Equal(2, result.MaxConcurrentTasks);
        Assert.Equal(60, result.PollingIntervalSeconds);
        _configRepositoryMock.Verify(r => r.AddAsync(It.IsAny<TaskConfiguration>(), default), Times.Once);
    }

    [Fact]
    public async Task GetConfigurationAsync_ShouldReturnExisting_WhenExists()
    {
        var config = new TaskConfiguration
        {
            Id = 1,
            MaxConcurrentTasks = 5,
            PollingIntervalSeconds = 30
        };
        _configRepositoryMock.Setup(r => r.GetAllAsync(default))
            .ReturnsAsync(new List<TaskConfiguration> { config });

        var result = await _service.GetConfigurationAsync();

        Assert.NotNull(result);
        Assert.Equal(5, result.MaxConcurrentTasks);
        Assert.Equal(30, result.PollingIntervalSeconds);
    }

    [Fact]
    public async Task UpdateConfigurationAsync_ShouldUpdateConfiguration()
    {
        var existingConfig = new TaskConfiguration
        {
            Id = 1,
            MaxConcurrentTasks = 2,
            PollingIntervalSeconds = 60
        };
        _configRepositoryMock.Setup(r => r.GetAllAsync(default))
            .ReturnsAsync(new List<TaskConfiguration> { existingConfig });

        var newConfig = new TaskConfiguration
        {
            MaxConcurrentTasks = 5,
            PollingIntervalSeconds = 30,
            EnableScheduling = true
        };

        var result = await _service.UpdateConfigurationAsync(newConfig);

        Assert.Equal(5, existingConfig.MaxConcurrentTasks);
        Assert.Equal(30, existingConfig.PollingIntervalSeconds);
        Assert.True(existingConfig.EnableScheduling);
        _configRepositoryMock.Verify(r => r.UpdateAsync(existingConfig, default), Times.Once);
    }
}
