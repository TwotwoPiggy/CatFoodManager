using CatFoodManager.Infrastructure.Logging;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace CatFoodManager.Infrastructure.Tests.Logging;

public class LoggingDecoratorTests
{
    private readonly Mock<ITestService> _serviceMock;
    private readonly Mock<ILogger<LoggingDecorator<ITestService>>> _loggerMock;
    private readonly LoggingDecorator<ITestService> _decorator;

    public LoggingDecoratorTests()
    {
        _serviceMock = new Mock<ITestService>();
        _loggerMock = new Mock<ILogger<LoggingDecorator<ITestService>>>();
        _decorator = new LoggingDecorator<ITestService>(_serviceMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_WithResult_ShouldLogStartAndCompletion()
    {
        var expectedResult = "test result";
        _serviceMock.Setup(x => x.GetDataAsync()).ReturnsAsync(expectedResult);

        var result = await _decorator.ExecuteAsync(x => x.GetDataAsync(), "GetData");

        Assert.Equal(expectedResult, result);
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Starting operation")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Operation completed")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WithException_ShouldLogErrorAndRethrow()
    {
        var expectedException = new InvalidOperationException("Test error");
        _serviceMock.Setup(x => x.GetDataAsync()).ThrowsAsync(expectedException);

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _decorator.ExecuteAsync(x => x.GetDataAsync(), "GetData"));

        Assert.Equal(expectedException, exception);
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Operation failed")),
                expectedException,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WithoutResult_ShouldLogStartAndCompletion()
    {
        var wasCalled = false;
        _serviceMock.Setup(x => x.DoWorkAsync()).Callback(() => wasCalled = true).Returns(Task.CompletedTask);

        await _decorator.ExecuteAsync(x => x.DoWorkAsync(), "DoWork");

        Assert.True(wasCalled);
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Starting operation")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Operation completed")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WithoutResult_WithException_ShouldLogErrorAndRethrow()
    {
        var expectedException = new InvalidOperationException("Test error");
        _serviceMock.Setup(x => x.DoWorkAsync()).ThrowsAsync(expectedException);

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _decorator.ExecuteAsync(x => x.DoWorkAsync(), "DoWork"));

        Assert.Equal(expectedException, exception);
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Operation failed")),
                expectedException,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public void Constructor_WithNullDecorated_ShouldThrowArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => 
            new LoggingDecorator<ITestService>(null!, _loggerMock.Object));
    }

    [Fact]
    public void Constructor_WithNullLogger_ShouldThrowArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => 
            new LoggingDecorator<ITestService>(_serviceMock.Object, null!));
    }

    public interface ITestService
    {
        Task<string> GetDataAsync();
        Task DoWorkAsync();
    }
}
