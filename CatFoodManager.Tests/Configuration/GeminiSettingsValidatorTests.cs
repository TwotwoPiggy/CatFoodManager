using CatFoodManager.Infrastructure.Configuration;
using Xunit;

namespace CatFoodManager.Tests.Configuration;

public class GeminiSettingsValidatorTests
{
    private readonly GeminiSettingsValidator _validator;

    public GeminiSettingsValidatorTests()
    {
        _validator = new GeminiSettingsValidator();
    }

    [Fact]
    public void Validate_ShouldReturnSuccess_WhenApiKeyIsValid()
    {
        var settings = new GeminiSettings
        {
            ApiKey = "valid-api-key"
        };

        var result = _validator.Validate(null, settings);

        Assert.True(result.Succeeded);
    }

    [Fact]
    public void Validate_ShouldReturnFail_WhenApiKeyIsEmpty()
    {
        var settings = new GeminiSettings
        {
            ApiKey = string.Empty
        };

        var result = _validator.Validate(null, settings);

        Assert.True(result.Failed);
        Assert.Equal("ApiKey is required for Gemini.", result.FailureMessage);
    }

    [Fact]
    public void Validate_ShouldReturnFail_WhenApiKeyIsNull()
    {
        var settings = new GeminiSettings
        {
            ApiKey = null!
        };

        var result = _validator.Validate(null, settings);

        Assert.True(result.Failed);
        Assert.Equal("ApiKey is required for Gemini.", result.FailureMessage);
    }

    [Fact]
    public void Validate_ShouldReturnFail_WhenApiKeyIsWhitespace()
    {
        var settings = new GeminiSettings
        {
            ApiKey = "   "
        };

        var result = _validator.Validate(null, settings);

        Assert.True(result.Failed);
        Assert.Equal("ApiKey is required for Gemini.", result.FailureMessage);
    }
}
