using CatFoodManager.Application.Interfaces;
using CatFoodManager.Application.Services;
using CatFoodManager.Domain.Entities;
using CatFoodManager.Domain.Enums;
using CatFoodManager.Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using TaskStatus = CatFoodManager.Domain.Enums.TaskStatus;

namespace CatFoodManager.Tests.Application.Services;

public class TaskExecutorTests
{
    private readonly Mock<IRepository<TaskItem>> _taskRepositoryMock;
    private readonly Mock<IRepository<TaskConfiguration>> _configRepositoryMock;
    private readonly Mock<ITaskHandler> _handlerMock;
    private readonly Mock<ITaskService> _taskServiceMock;
    private readonly Mock<ILogger<TaskExecutor>> _loggerMock;
    private readonly ServiceProvider _serviceProvider;
    private readonly TaskExecutor _executor;

    public TaskExecutorTests()
    {
        _taskRepositoryMock = new Mock<IRepository<TaskItem>>();
        _configRepositoryMock = new Mock<IRepository<TaskConfiguration>>();
        _handlerMock = new Mock<ITaskHandler>();
        _taskServiceMock = new Mock<ITaskService>();
        _loggerMock = new Mock<ILogger<TaskExecutor>>();

        _handlerMock.SetupGet(h => h.TaskType).Returns(TaskType.ImageSync);

        _configRepositoryMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<TaskConfiguration>
            {
                new() { MaxConcurrentTasks = 2, PollingIntervalSeconds = 60 }
            });

        var services = new ServiceCollection();
        services.AddSingleton(_taskRepositoryMock.Object);
        services.AddSingleton(_configRepositoryMock.Object);
        services.AddSingleton(_handlerMock.Object);
        services.AddSingleton(_taskServiceMock.Object);
        _serviceProvider = services.BuildServiceProvider();

        _executor = new TaskExecutor(
            _serviceProvider,
            _loggerMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldExecuteTaskSuccessfully()
    {
        var task = new TaskItem
        {
            Id = 1,
            Type = TaskType.ImageSync,
            Status = TaskStatus.Pending,
            Parameters = "{}"
        };

        _taskRepositoryMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(task);

        _handlerMock.Setup(h => h.HandleAsync("{}", It.IsAny<CancellationToken>()))
            .ReturnsAsync(TaskResult.Succeeded("{\"count\":10}"));

        await _executor.ExecuteAsync(1);

        _taskServiceMock.Verify(s => s.UpdateStatusAsync(1, TaskStatus.Running, null, null, null, It.IsAny<CancellationToken>()), Times.Once);
        _taskServiceMock.Verify(s => s.UpdateStatusAsync(1, TaskStatus.Completed, "{\"count\":10}", null, null, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldHandleFailure()
    {
        var task = new TaskItem
        {
            Id = 1,
            Type = TaskType.ImageSync,
            Status = TaskStatus.Pending,
            Parameters = "{}"
        };

        _taskRepositoryMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(task);

        _handlerMock.Setup(h => h.HandleAsync("{}", It.IsAny<CancellationToken>()))
            .ReturnsAsync(TaskResult.Failed("Test error"));

        await _executor.ExecuteAsync(1);

        _taskServiceMock.Verify(s => s.UpdateStatusAsync(1, TaskStatus.Failed, null, "Test error", null, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldHandleException()
    {
        var task = new TaskItem
        {
            Id = 1,
            Type = TaskType.ImageSync,
            Status = TaskStatus.Pending,
            Parameters = "{}"
        };

        _taskRepositoryMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(task);

        _handlerMock.Setup(h => h.HandleAsync("{}", It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Unexpected error"));

        await _executor.ExecuteAsync(1);

        _taskServiceMock.Verify(s => s.UpdateStatusAsync(1, TaskStatus.Failed, null, "Unexpected error", null, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldNotExecuteIfNoHandlerFound()
    {
        var task = new TaskItem
        {
            Id = 1,
            Type = TaskType.ImageDelete,
            Status = TaskStatus.Pending,
            Parameters = "{}"
        };

        _taskRepositoryMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(task);

        await _executor.ExecuteAsync(1);

        _handlerMock.Verify(h => h.HandleAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
        _taskServiceMock.Verify(s => s.UpdateStatusAsync(1, TaskStatus.Failed, null, It.IsAny<string>(), null, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task IsRunningAsync_ShouldReturnTrue_WhenTaskIsRunning()
    {
        var task = new TaskItem
        {
            Id = 1,
            Type = TaskType.ImageSync,
            Status = TaskStatus.Pending,
            Parameters = "{}"
        };

        _taskRepositoryMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(task);

        var tcs = new TaskCompletionSource<TaskResult>();
        _handlerMock.Setup(h => h.HandleAsync("{}", It.IsAny<CancellationToken>()))
            .Returns(tcs.Task);

        var executeTask = _executor.ExecuteAsync(1);

        await Task.Delay(100);
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
            Status = TaskStatus.Pending,
            Parameters = "{}"
        };

        _taskRepositoryMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(task);

        var tcs = new TaskCompletionSource<TaskResult>();
        _handlerMock.Setup(h => h.HandleAsync("{}", It.IsAny<CancellationToken>()))
            .Returns(tcs.Task);

        var executeTask = _executor.ExecuteAsync(1);

        await Task.Delay(100);
        Assert.Equal(1, await _executor.GetRunningCountAsync());

        tcs.SetResult(TaskResult.Succeeded());
        await executeTask;

        Assert.Equal(0, await _executor.GetRunningCountAsync());
    }
}
