using CatFoodManager.Application.Interfaces;
using CatFoodManager.Application.Services;
using CatFoodManager.Domain.Entities;
using CatFoodManager.Domain.Enums;
using CatFoodManager.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace CatFoodManager.Tests.Application.Services;

public class TaskExecutorTests
{
    private readonly Mock<IRepository<TaskItem>> _taskRepositoryMock;
    private readonly Mock<IRepository<TaskConfiguration>> _configRepositoryMock;
    private readonly Mock<ITaskHandler> _handlerMock;
    private readonly Mock<ITaskService> _taskServiceMock;
    private readonly Mock<ILogger<TaskExecutor>> _loggerMock;
    private readonly TaskExecutor _executor;

    public TaskExecutorTests()
    {
        _taskRepositoryMock = new Mock<IRepository<TaskItem>>();
        _configRepositoryMock = new Mock<IRepository<TaskConfiguration>>();
        _handlerMock = new Mock<ITaskHandler>();
        _taskServiceMock = new Mock<ITaskService>();
        _loggerMock = new Mock<ILogger<TaskExecutor>>();

        _handlerMock.SetupGet(h => h.TaskType).Returns(TaskType.ImageSync);

        _configRepositoryMock.Setup(r => r.GetAllAsync(default))
            .ReturnsAsync(new List<TaskConfiguration>
            {
                new() { MaxConcurrentTasks = 2, PollingIntervalSeconds = 60 }
            });

        _executor = new TaskExecutor(
            _taskRepositoryMock.Object,
            _configRepositoryMock.Object,
            new List<ITaskHandler> { _handlerMock.Object },
            _taskServiceMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldExecuteTaskSuccessfully()
    {
        var task = new TaskItem
        {
            Id = 1,
            Type = TaskType.ImageSync,
            Status = Domain.Enums.TaskStatus.Pending,
            Parameters = "{}"
        };

        _taskRepositoryMock.Setup(r => r.GetByIdAsync(1, default))
            .ReturnsAsync(task);

        _handlerMock.Setup(h => h.HandleAsync("{}", default))
            .ReturnsAsync(TaskResult.Succeeded("{\"count\":10}"));

        await _executor.ExecuteAsync(1);

        _taskServiceMock.Verify(s => s.UpdateStatusAsync(1, Domain.Enums.TaskStatus.Running, null, null, default), Times.Once);
        _taskServiceMock.Verify(s => s.UpdateStatusAsync(1, Domain.Enums.TaskStatus.Completed, "{\"count\":10}", null, default), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldHandleFailure()
    {
        var task = new TaskItem
        {
            Id = 1,
            Type = TaskType.ImageSync,
            Status = Domain.Enums.TaskStatus.Pending,
            Parameters = "{}"
        };

        _taskRepositoryMock.Setup(r => r.GetByIdAsync(1, default))
            .ReturnsAsync(task);

        _handlerMock.Setup(h => h.HandleAsync("{}", default))
            .ReturnsAsync(TaskResult.Failed("Test error"));

        await _executor.ExecuteAsync(1);

        _taskServiceMock.Verify(s => s.UpdateStatusAsync(1, Domain.Enums.TaskStatus.Failed, null, "Test error", default), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldHandleException()
    {
        var task = new TaskItem
        {
            Id = 1,
            Type = TaskType.ImageSync,
            Status = Domain.Enums.TaskStatus.Pending,
            Parameters = "{}"
        };

        _taskRepositoryMock.Setup(r => r.GetByIdAsync(1, default))
            .ReturnsAsync(task);

        _handlerMock.Setup(h => h.HandleAsync("{}", default))
            .ThrowsAsync(new Exception("Unexpected error"));

        await _executor.ExecuteAsync(1);

        _taskServiceMock.Verify(s => s.UpdateStatusAsync(1, Domain.Enums.TaskStatus.Failed, null, "Unexpected error", default), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldNotExecuteIfNoHandlerFound()
    {
        var task = new TaskItem
        {
            Id = 1,
            Type = TaskType.ImageDelete,
            Status = Domain.Enums.TaskStatus.Pending,
            Parameters = "{}"
        };

        _taskRepositoryMock.Setup(r => r.GetByIdAsync(1, default))
            .ReturnsAsync(task);

        await _executor.ExecuteAsync(1);

        _handlerMock.Verify(h => h.HandleAsync(It.IsAny<string>(), default), Times.Never);
        _taskServiceMock.Verify(s => s.UpdateStatusAsync(1, Domain.Enums.TaskStatus.Failed, null, It.IsAny<string>(), default), Times.Once);
    }

    [Fact]
    public async Task IsRunningAsync_ShouldReturnTrue_WhenTaskIsRunning()
    {
        var task = new TaskItem
        {
            Id = 1,
            Type = TaskType.ImageSync,
            Status = Domain.Enums.TaskStatus.Pending,
            Parameters = "{}"
        };

        _taskRepositoryMock.Setup(r => r.GetByIdAsync(1, default))
            .ReturnsAsync(task);

        var tcs = new TaskCompletionSource<TaskResult>();
        _handlerMock.Setup(h => h.HandleAsync("{}", default))
            .Returns(tcs.Task);

        var executeTask = _executor.ExecuteAsync(1);

        var isRunning = await _executor.IsRunningAsync(1);
        Assert.True(isRunning);

        tcs.SetResult(TaskResult.Succeeded());
        await executeTask;

        isRunning = await _executor.IsRunningAsync(1);
        Assert.False(isRunning);
    }

    [Fact]
    public async Task GetRunningCountAsync_ShouldReturnCorrectCount()
    {
        Assert.Equal(0, await _executor.GetRunningCountAsync());

        var task = new TaskItem
        {
            Id = 1,
            Type = TaskType.ImageSync,
            Status = Domain.Enums.TaskStatus.Pending,
            Parameters = "{}"
        };

        _taskRepositoryMock.Setup(r => r.GetByIdAsync(1, default))
            .ReturnsAsync(task);

        var tcs = new TaskCompletionSource<TaskResult>();
        _handlerMock.Setup(h => h.HandleAsync("{}", default))
            .Returns(tcs.Task);

        var executeTask = _executor.ExecuteAsync(1);

        await Task.Delay(50);
        Assert.Equal(1, await _executor.GetRunningCountAsync());

        tcs.SetResult(TaskResult.Succeeded());
        await executeTask;

        Assert.Equal(0, await _executor.GetRunningCountAsync());
    }
}
