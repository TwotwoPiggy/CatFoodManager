using CatFoodManager.Infrastructure.Exceptions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace CatFoodManager.Infrastructure.Tests.Exceptions;

public class ExceptionHandlerTests
{
    private readonly Mock<ILogger<ExceptionHandler>> _loggerMock;
    private readonly ExceptionHandler _handler;

    public ExceptionHandlerTests()
    {
        _loggerMock = new Mock<ILogger<ExceptionHandler>>();
        _handler = new ExceptionHandler(_loggerMock.Object);
    }

    [Fact]
    public void Handle_WithDomainException_ShouldLogWarning()
    {
        var exception = new DomainException("TEST_CODE", "Test message");

        _handler.Handle(exception);

        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("TEST_CODE")),
                exception,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public void Handle_WithNonDomainException_ShouldLogError()
    {
        var exception = new InvalidOperationException("Test error");

        _handler.Handle(exception);

        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("unhandled exception")),
                exception,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public void GetUserFriendlyMessage_WithDomainException_ShouldReturnExceptionMessage()
    {
        var exception = new DomainException("CODE", "User friendly message");

        var message = _handler.GetUserFriendlyMessage(exception);

        Assert.Equal("User friendly message", message);
    }

    [Fact]
    public void GetUserFriendlyMessage_WithNonDomainException_ShouldReturnGenericMessage()
    {
        var exception = new InvalidOperationException("Internal error");

        var message = _handler.GetUserFriendlyMessage(exception);

        Assert.Equal("An unexpected error occurred. Please try again later.", message);
    }

    [Fact]
    public void GetUserFriendlyMessage_WithEntityNotFoundException_ShouldReturnExceptionMessage()
    {
        var exception = new EntityNotFoundException(typeof(TestEntity), 123);

        var message = _handler.GetUserFriendlyMessage(exception);

        Assert.Contains("TestEntity", message);
        Assert.Contains("123", message);
    }

    private class TestEntity { }
}
