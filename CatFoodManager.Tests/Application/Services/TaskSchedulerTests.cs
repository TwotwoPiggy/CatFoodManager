using CatFoodManager.Domain.Entities;
using CatFoodManager.Domain.Enums;
using CatFoodManager.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using AppTaskScheduler = CatFoodManager.Application.Services.TaskScheduler;

namespace CatFoodManager.Tests.Application.Services;

public class TaskSchedulerTests
{
    private readonly Mock<IRepository<TaskItem>> _taskRepositoryMock;
    private readonly Mock<IRepository<TaskConfiguration>> _configRepositoryMock;
    private readonly Mock<ILogger<AppTaskScheduler>> _loggerMock;
    private readonly AppTaskScheduler _scheduler;

    public TaskSchedulerTests()
    {
        _taskRepositoryMock = new Mock<IRepository<TaskItem>>();
        _configRepositoryMock = new Mock<IRepository<TaskConfiguration>>();
        _loggerMock = new Mock<ILogger<AppTaskScheduler>>();

        _scheduler = new AppTaskScheduler(
            _taskRepositoryMock.Object,
            _configRepositoryMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task StartAsync_ShouldSetRunningFlag()
    {
        await _scheduler.StartAsync();

        var isRunning = await _scheduler.IsRunningAsync();
        Assert.True(isRunning);
    }

    [Fact]
    public async Task StopAsync_ShouldClearRunningFlag()
    {
        await _scheduler.StartAsync();
        await _scheduler.StopAsync();

        var isRunning = await _scheduler.IsRunningAsync();
        Assert.False(isRunning);
    }

    [Fact]
    public async Task EnqueueAsync_ShouldUpdateTaskStatusToQueued()
    {
        var task = new TaskItem { Id = 1, Status = Domain.Enums.TaskStatus.Pending };
        _taskRepositoryMock.Setup(r => r.GetByIdAsync(1, default))
            .ReturnsAsync(task);

        await _scheduler.StartAsync();
        await _scheduler.EnqueueAsync(1);

        Assert.Equal(Domain.Enums.TaskStatus.Queued, task.Status);
        _taskRepositoryMock.Verify(r => r.UpdateAsync(task, default), Times.Once);
    }

    [Fact]
    public async Task EnqueueAsync_ShouldNotEnqueueRunningTask()
    {
        var task = new TaskItem { Id = 1, Status = Domain.Enums.TaskStatus.Running };
        _taskRepositoryMock.Setup(r => r.GetByIdAsync(1, default))
            .ReturnsAsync(task);

        await _scheduler.StartAsync();
        await _scheduler.EnqueueAsync(1);

        Assert.Equal(Domain.Enums.TaskStatus.Running, task.Status);
        _taskRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<TaskItem>(), default), Times.Never);
    }

    [Fact]
    public async Task EnqueueAsync_ShouldNotEnqueueCompletedTask()
    {
        var task = new TaskItem { Id = 1, Status = Domain.Enums.TaskStatus.Completed };
        _taskRepositoryMock.Setup(r => r.GetByIdAsync(1, default))
            .ReturnsAsync(task);

        await _scheduler.StartAsync();
        await _scheduler.EnqueueAsync(1);

        Assert.Equal(Domain.Enums.TaskStatus.Completed, task.Status);
        _taskRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<TaskItem>(), default), Times.Never);
    }

    [Fact]
    public async Task GetQueueLengthAsync_ShouldReturnQueueLength()
    {
        await _scheduler.StartAsync();

        var task1 = new TaskItem { Id = 1, Status = Domain.Enums.TaskStatus.Pending };
        var task2 = new TaskItem { Id = 2, Status = Domain.Enums.TaskStatus.Pending };
        _taskRepositoryMock.Setup(r => r.GetByIdAsync(1, default)).ReturnsAsync(task1);
        _taskRepositoryMock.Setup(r => r.GetByIdAsync(2, default)).ReturnsAsync(task2);

        await _scheduler.EnqueueAsync(1);
        await _scheduler.EnqueueAsync(2);

        var length = await _scheduler.GetQueueLengthAsync();
        Assert.Equal(2, length);
    }
}
