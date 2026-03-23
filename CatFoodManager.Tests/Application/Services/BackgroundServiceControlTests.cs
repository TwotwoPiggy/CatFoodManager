using CatFoodManager.Application.Interfaces;
using CatFoodManager.Domain.Entities;
using CatFoodManager.Domain.Interfaces;
using CatFoodManager.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace CatFoodManager.Tests.Application.Services;

public class BackgroundServiceControlTests
{
    private readonly Mock<IServiceProvider> _serviceProviderMock;
    private readonly Mock<IServiceScope> _serviceScopeMock;
    private readonly Mock<IServiceScopeFactory> _serviceScopeFactoryMock;
    private readonly Mock<ILogger<TaskBackgroundService>> _loggerMock;
    private readonly Mock<ITaskScheduler> _taskSchedulerMock;
    private readonly Mock<ITaskExecutor> _taskExecutorMock;
    private readonly Mock<IRepository<TaskItem>> _taskRepositoryMock;
    private readonly Mock<IRepository<TaskConfiguration>> _configRepositoryMock;

    public BackgroundServiceControlTests()
    {
        _serviceProviderMock = new Mock<IServiceProvider>();
        _serviceScopeMock = new Mock<IServiceScope>();
        _serviceScopeFactoryMock = new Mock<IServiceScopeFactory>();
        _loggerMock = new Mock<ILogger<TaskBackgroundService>>();
        _taskSchedulerMock = new Mock<ITaskScheduler>();
        _taskExecutorMock = new Mock<ITaskExecutor>();
        _taskRepositoryMock = new Mock<IRepository<TaskItem>>();
        _configRepositoryMock = new Mock<IRepository<TaskConfiguration>>();

        _serviceScopeMock.Setup(x => x.ServiceProvider).Returns(_serviceProviderMock.Object);
        _serviceScopeFactoryMock.Setup(x => x.CreateScope()).Returns(_serviceScopeMock.Object);

        _serviceProviderMock.Setup(x => x.GetService(typeof(ITaskScheduler))).Returns(_taskSchedulerMock.Object);
        _serviceProviderMock.Setup(x => x.GetService(typeof(ITaskExecutor))).Returns(_taskExecutorMock.Object);
        _serviceProviderMock.Setup(x => x.GetService(typeof(IRepository<TaskItem>))).Returns(_taskRepositoryMock.Object);
        _serviceProviderMock.Setup(x => x.GetService(typeof(IRepository<TaskConfiguration>))).Returns(_configRepositoryMock.Object);
        _serviceProviderMock.Setup(x => x.GetService(typeof(IServiceScopeFactory))).Returns(_serviceScopeFactoryMock.Object);

        _configRepositoryMock.Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<TaskConfiguration>());
    }

    [Fact]
    public async Task IsRunningAsync_ShouldReturnFalse_WhenServiceNotStarted()
    {
        var service = CreateService();

        var result = await service.IsRunningAsync();

        Assert.False(result);
    }

    [Fact]
    public async Task IsPausedAsync_ShouldReturnFalse_WhenServiceNotStarted()
    {
        var service = CreateService();

        var result = await service.IsPausedAsync();

        Assert.False(result);
    }

    [Fact]
    public async Task GetStatusAsync_ShouldReturnCorrectStatus_WhenServiceNotRunning()
    {
        _taskExecutorMock.Setup(x => x.GetRunningCountAsync(It.IsAny<CancellationToken>())).ReturnsAsync(0);
        _taskSchedulerMock.Setup(x => x.GetQueueLengthAsync(It.IsAny<CancellationToken>())).ReturnsAsync(0);

        var service = CreateService();

        var status = await service.GetStatusAsync();

        Assert.False(status.IsRunning);
        Assert.False(status.IsPaused);
        Assert.Equal(0, status.RunningTaskCount);
        Assert.Equal(0, status.QueueLength);
        Assert.Null(status.StartedAt);
    }

    [Fact]
    public async Task PauseAsync_ShouldNotPause_WhenServiceNotRunning()
    {
        var service = CreateService();

        await service.PauseAsync();

        var isPaused = await service.IsPausedAsync();
        Assert.False(isPaused);
    }

    [Fact]
    public async Task ResumeAsync_ShouldNotResume_WhenServiceNotRunning()
    {
        var service = CreateService();

        await service.ResumeAsync();

        var isPaused = await service.IsPausedAsync();
        Assert.False(isPaused);
    }

    [Fact]
    public async Task StartServiceAsync_ShouldSetRunningState()
    {
        var service = CreateService();

        await service.StartServiceAsync();

        await Task.Delay(100);

        var isRunning = await service.IsRunningAsync();
        Assert.True(isRunning);

        await service.StopServiceAsync();
    }

    [Fact]
    public async Task PauseAsync_ShouldSetPausedState_WhenServiceIsRunning()
    {
        var service = CreateService();

        await service.StartServiceAsync();
        await Task.Delay(100);

        await service.PauseAsync();

        var isPaused = await service.IsPausedAsync();
        Assert.True(isPaused);

        await service.StopServiceAsync();
    }

    [Fact]
    public async Task ResumeAsync_ShouldClearPausedState_WhenServiceIsPaused()
    {
        var service = CreateService();

        await service.StartServiceAsync();
        await Task.Delay(100);
        await service.PauseAsync();

        await service.ResumeAsync();

        var isPaused = await service.IsPausedAsync();
        Assert.False(isPaused);

        await service.StopServiceAsync();
    }

    [Fact]
    public async Task StopServiceAsync_ShouldStopRunningService()
    {
        var service = CreateService();

        await service.StartServiceAsync();
        await Task.Delay(100);

        await service.StopServiceAsync();

        var isRunning = await service.IsRunningAsync();
        Assert.False(isRunning);
    }

    [Fact]
    public async Task GetStatusAsync_ShouldReturnRunningCountAndQueueLength()
    {
        _taskExecutorMock.Setup(x => x.GetRunningCountAsync(It.IsAny<CancellationToken>())).ReturnsAsync(3);
        _taskSchedulerMock.Setup(x => x.GetQueueLengthAsync(It.IsAny<CancellationToken>())).ReturnsAsync(5);

        var service = CreateService();

        var status = await service.GetStatusAsync();

        Assert.Equal(3, status.RunningTaskCount);
        Assert.Equal(5, status.QueueLength);
    }

    [Fact]
    public async Task StartServiceAsync_ShouldBeIdempotent()
    {
        var service = CreateService();

        await service.StartServiceAsync();
        await Task.Delay(100);

        await service.StartServiceAsync();

        var isRunning = await service.IsRunningAsync();
        Assert.True(isRunning);

        await service.StopServiceAsync();
    }

    [Fact]
    public async Task GetStatusAsync_ShouldCreateScopeAndResolveServices()
    {
        _taskExecutorMock.Setup(x => x.GetRunningCountAsync(It.IsAny<CancellationToken>())).ReturnsAsync(2);
        _taskSchedulerMock.Setup(x => x.GetQueueLengthAsync(It.IsAny<CancellationToken>())).ReturnsAsync(4);

        var service = CreateService();

        var status = await service.GetStatusAsync();

        _serviceScopeFactoryMock.Verify(x => x.CreateScope(), Times.AtLeastOnce);
        _serviceProviderMock.Verify(x => x.GetService(typeof(ITaskExecutor)), Times.AtLeastOnce);
        _serviceProviderMock.Verify(x => x.GetService(typeof(ITaskScheduler)), Times.AtLeastOnce);

        Assert.Equal(2, status.RunningTaskCount);
        Assert.Equal(4, status.QueueLength);
    }

    private TaskBackgroundService CreateService()
    {
        return new TaskBackgroundService(_serviceProviderMock.Object, _loggerMock.Object);
    }
}
