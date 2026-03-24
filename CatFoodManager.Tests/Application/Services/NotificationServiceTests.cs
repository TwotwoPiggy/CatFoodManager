using CatFoodManager.Application.Interfaces;
using CatfoodManagement;
using CatfoodManagement.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using AppTaskStatus = CatFoodManager.Domain.Enums.TaskStatus;
using AppTaskType = CatFoodManager.Domain.Enums.TaskType;

namespace CatFoodManager.Tests.Application.Services;

public class NotificationServiceTests
{
    private readonly Mock<MainForm> _mainFormMock;
    private readonly Mock<ILogger<NotificationService>> _loggerMock;
    private readonly NotificationService _service;

    public NotificationServiceTests()
    {
        _mainFormMock = new Mock<MainForm>();
        _loggerMock = new Mock<ILogger<NotificationService>>();

        _service = new NotificationService(_mainFormMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task PublishTaskStatusChangedAsync_ShouldCallExecuteScriptAsync()
    {
        var taskId = 1L;
        var taskName = "Test Task";
        var taskType = (int)AppTaskType.ImageSync;
        var oldStatus = (int)AppTaskStatus.Pending;
        var newStatus = (int)AppTaskStatus.Running;

        await _service.PublishTaskStatusChangedAsync(taskId, taskName, taskType, oldStatus, newStatus);

        _mainFormMock.Verify(
            m => m.ExecuteScriptAsync(It.Is<string>(s => 
                s.Contains("window.onTaskStatusChanged") && 
                s.Contains(taskId.ToString()))),
            Times.Once);
    }

    [Fact]
    public async Task PublishTaskStatusChangedAsync_ShouldIncludeAllEventData()
    {
        var taskId = 1L;
        var taskName = "Test Task";
        var taskType = (int)AppTaskType.ImageSync;
        var oldStatus = (int)AppTaskStatus.Pending;
        var newStatus = (int)AppTaskStatus.Completed;
        var result = "{\"count\":10}";
        var errorMessage = "Test error";

        string? capturedScript = null;
        _mainFormMock.Setup(m => m.ExecuteScriptAsync(It.IsAny<string>()))
            .Callback<string>(s => capturedScript = s)
            .ReturnsAsync(string.Empty);

        await _service.PublishTaskStatusChangedAsync(taskId, taskName, taskType, oldStatus, newStatus, result, errorMessage);

        Assert.NotNull(capturedScript);
        Assert.Contains($"\"taskId\":{taskId}", capturedScript);
        Assert.Contains($"\"taskName\":\"{taskName}\"", capturedScript);
        Assert.Contains($"\"taskType\":{taskType}", capturedScript);
        Assert.Contains($"\"oldStatus\":{oldStatus}", capturedScript);
        Assert.Contains($"\"newStatus\":{newStatus}", capturedScript);
        Assert.Contains($"\"result\":\"{result}\"", capturedScript);
        Assert.Contains($"\"errorMessage\":\"{errorMessage}\"", capturedScript);
    }

    [Fact]
    public async Task PublishTaskStatusChangedAsync_ShouldHandleException()
    {
        _mainFormMock.Setup(m => m.ExecuteScriptAsync(It.IsAny<string>()))
            .ThrowsAsync(new Exception("Test exception"));

        await _service.PublishTaskStatusChangedAsync(1, "Test", 0, 0, 1);

        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Failed to send task status notification")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task PublishTaskStatusChangedAsync_ShouldCheckFunctionExists()
    {
        _mainFormMock.Setup(m => m.ExecuteScriptAsync(It.IsAny<string>()))
            .ReturnsAsync(string.Empty);

        await _service.PublishTaskStatusChangedAsync(1, "Test", 0, 0, 1);

        _mainFormMock.Verify(
            m => m.ExecuteScriptAsync(It.Is<string>(s => s.Contains("typeof window.onTaskStatusChanged === 'function'"))),
            Times.Once);
    }
}
