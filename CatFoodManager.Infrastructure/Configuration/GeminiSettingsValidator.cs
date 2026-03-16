using Microsoft.Extensions.Options;

namespace CatFoodManager.Infrastructure.Configuration;

public class GeminiSettingsValidator : IValidateOptions<GeminiSettings>
{
    public ValidateOptionsResult Validate(string? name, GeminiSettings options)
    {
        if (string.IsNullOrWhiteSpace(options.ApiKey))
        {
            return ValidateOptionsResult.Fail("ApiKey is required for Gemini.");
        }
        return ValidateOptionsResult.Success;
    }
}
