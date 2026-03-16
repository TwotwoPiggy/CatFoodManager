using CatFoodManager.Infrastructure.Exceptions;
using Xunit;

namespace CatFoodManager.Infrastructure.Tests.Exceptions;

public class DomainExceptionTests
{
    [Fact]
    public void Constructor_WithCodeAndMessage_ShouldSetProperties()
    {
        var code = "TEST_ERROR";
        var message = "Test error message";

        var exception = new DomainException(code, message);

        Assert.Equal(code, exception.Code);
        Assert.Equal(message, exception.Message);
    }

    [Fact]
    public void Constructor_WithCodeMessageAndInnerException_ShouldSetProperties()
    {
        var code = "TEST_ERROR";
        var message = "Test error message";
        var innerException = new InvalidOperationException("Inner error");

        var exception = new DomainException(code, message, innerException);

        Assert.Equal(code, exception.Code);
        Assert.Equal(message, exception.Message);
        Assert.Equal(innerException, exception.InnerException);
    }

    [Fact]
    public void DomainException_ShouldInheritFromException()
    {
        var exception = new DomainException("CODE", "Message");

        Assert.IsAssignableFrom<Exception>(exception);
    }
}
