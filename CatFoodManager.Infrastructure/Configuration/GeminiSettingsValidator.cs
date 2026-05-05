using Microsoft.Extensions.Options;

namespace CatFoodManager.Infrastructure.Configuration;

/// <summary>
/// Gemini设置验证器，用于验证Gemini配置的有效性�?/// Gemini settings validator, used to validate the validity of Gemini configuration.
/// </summary>
public class GeminiSettingsValidator : IValidateOptions<GeminiSettings>
{
    /// <summary>
    /// 验证Gemini设置�?    /// Validates Gemini settings.
    /// </summary>
    /// <param name="name">选项名称 / Options name</param>
    /// <param name="options">Gemini设置选项 / Gemini settings options</param>
    /// <returns>验证结果 / Validation result</returns>
    public ValidateOptionsResult Validate(string? name, GeminiSettings options)
    {
        if (string.IsNullOrWhiteSpace(options.ApiKey))
        {
            return ValidateOptionsResult.Fail("ApiKey is required for Gemini settings.");
        }

        if (string.IsNullOrWhiteSpace(options.Model))
        {
            return ValidateOptionsResult.Fail("Model is required for Gemini settings.");
        }

        if (options.TimeoutSeconds <= 0)
        {
            return ValidateOptionsResult.Fail("TimeoutSeconds must be greater than 0.");
        }

        if (options.MaxRetries < 0)
        {
            return ValidateOptionsResult.Fail("MaxRetries cannot be negative.");
        }

        if (options.EnableCache && options.CacheExpirationMinutes <= 0)
        {
            return ValidateOptionsResult.Fail("CacheExpirationMinutes must be greater than 0 when caching is enabled.");
        }

        return ValidateOptionsResult.Success;
    }
}
