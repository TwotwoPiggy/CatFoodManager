using Microsoft.Extensions.Options;

namespace CatFoodManager.Infrastructure.Configuration;

public class DatabaseSettingsValidator : IValidateOptions<DatabaseSettings>
{
    public ValidateOptionsResult Validate(string? name, DatabaseSettings options)
    {
        if (string.IsNullOrWhiteSpace(options.ConnectionString))
        {
            return ValidateOptionsResult.Fail("ConnectionString is required.");
        }
        return ValidateOptionsResult.Success;
    }
}
