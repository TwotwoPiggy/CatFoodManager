using CatFoodManager.Infrastructure.Configuration;
using Xunit;

namespace CatFoodManager.Tests.Configuration;

public class DatabaseSettingsValidatorTests
{
    private readonly DatabaseSettingsValidator _validator;

    public DatabaseSettingsValidatorTests()
    {
        _validator = new DatabaseSettingsValidator();
    }

    [Fact]
    public void Validate_ShouldReturnSuccess_WhenConnectionStringIsValid()
    {
        var settings = new DatabaseSettings
        {
            ConnectionString = "Data Source=test.db"
        };

        var result = _validator.Validate(null, settings);

        Assert.True(result.Succeeded);
    }

    [Fact]
    public void Validate_ShouldReturnFail_WhenConnectionStringIsEmpty()
    {
        var settings = new DatabaseSettings
        {
            ConnectionString = string.Empty
        };

        var result = _validator.Validate(null, settings);

        Assert.True(result.Failed);
        Assert.Equal("ConnectionString is required.", result.FailureMessage);
    }

    [Fact]
    public void Validate_ShouldReturnFail_WhenConnectionStringIsNull()
    {
        var settings = new DatabaseSettings
        {
            ConnectionString = null!
        };

        var result = _validator.Validate(null, settings);

        Assert.True(result.Failed);
        Assert.Equal("ConnectionString is required.", result.FailureMessage);
    }

    [Fact]
    public void Validate_ShouldReturnFail_WhenConnectionStringIsWhitespace()
    {
        var settings = new DatabaseSettings
        {
            ConnectionString = "   "
        };

        var result = _validator.Validate(null, settings);

        Assert.True(result.Failed);
        Assert.Equal("ConnectionString is required.", result.FailureMessage);
    }
}
