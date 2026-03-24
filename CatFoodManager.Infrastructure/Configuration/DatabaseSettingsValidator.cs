using Microsoft.Extensions.Options;

namespace CatFoodManager.Infrastructure.Configuration;

/// <summary>
/// 数据库设置验证器，用于验证数据库配置的有效性。
/// Database settings validator, used to validate the validity of database configuration.
/// </summary>
public class DatabaseSettingsValidator : IValidateOptions<DatabaseSettings>
{
    /// <summary>
    /// 验证数据库设置。
    /// Validates database settings.
    /// </summary>
    /// <param name="name">选项名称 / Options name</param>
    /// <param name="options">数据库设置选项 / Database settings options</param>
    /// <returns>验证结果 / Validation result</returns>
    public ValidateOptionsResult Validate(string? name, DatabaseSettings options)
    {
        if (string.IsNullOrWhiteSpace(options.ConnectionString) && string.IsNullOrWhiteSpace(options.DatabasePath))
        {
            return ValidateOptionsResult.Fail("Either ConnectionString or DatabasePath must be specified.");
        }

        if (options.CommandTimeout <= 0)
        {
            return ValidateOptionsResult.Fail("CommandTimeout must be greater than 0.");
        }

        return ValidateOptionsResult.Success;
    }
}
