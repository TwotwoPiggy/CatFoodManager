using CatFoodManager.Infrastructure.Exceptions;
using Xunit;

namespace CatFoodManager.Infrastructure.Tests.Exceptions;

public class ValidationExceptionTests
{
    [Fact]
    public void Constructor_WithFailures_ShouldSetProperties()
    {
        var failures = new Dictionary<string, string[]>
        {
            { "Name", new[] { "Name is required", "Name must be at least 3 characters" } },
            { "Email", new[] { "Invalid email format" } }
        };

        var exception = new ValidationException(failures);

        Assert.Equal("VALIDATION_ERROR", exception.Code);
        Assert.Equal(failures, exception.Failures);
        Assert.Equal("One or more validation failures have occurred.", exception.Message);
    }

    [Fact]
    public void ValidationException_ShouldInheritFromDomainException()
    {
        var failures = new Dictionary<string, string[]>();
        var exception = new ValidationException(failures);

        Assert.IsAssignableFrom<DomainException>(exception);
    }

    [Fact]
    public void Failures_ShouldContainMultipleErrors()
    {
        var failures = new Dictionary<string, string[]>
        {
            { "Field1", new[] { "Error1", "Error2" } },
            { "Field2", new[] { "Error3" } }
        };

        var exception = new ValidationException(failures);

        Assert.Equal(2, exception.Failures.Count);
        Assert.Equal(2, exception.Failures["Field1"].Length);
        Assert.Single(exception.Failures["Field2"]);
    }
}
