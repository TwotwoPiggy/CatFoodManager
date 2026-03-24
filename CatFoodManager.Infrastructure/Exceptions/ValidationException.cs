namespace CatFoodManager.Infrastructure.Exceptions;

/// <summary>
/// 验证异常，当数据验证失败时抛出。
/// Validation exception, thrown when data validation fails.
/// </summary>
public class ValidationException : Exception
{
    /// <summary>
    /// 验证错误字典，键为字段名，值为错误消息列表。
    /// Validation errors dictionary, key is field name, value is list of error messages.
    /// </summary>
    public IReadOnlyDictionary<string, string[]> Errors { get; }

    /// <summary>
    /// 构造函数。
    /// Constructor.
    /// </summary>
    /// <param name="errors">验证错误字典 / Validation errors dictionary</param>
    public ValidationException(IDictionary<string, string[]> errors)
        : base("One or more validation errors occurred.")
    {
        Errors = new Dictionary<string, string[]>(errors);
    }

    /// <summary>
    /// 构造函数，包含单个错误。
    /// Constructor with a single error.
    /// </summary>
    /// <param name="field">字段名 / Field name</param>
    /// <param name="message">错误消息 / Error message</param>
    public ValidationException(string field, string message)
        : base(message)
    {
        Errors = new Dictionary<string, string[]>
        {
            { field, new[] { message } }
        };
    }

    /// <summary>
    /// 构造函数，包含错误消息。
    /// Constructor with error message.
    /// </summary>
    /// <param name="message">错误消息 / Error message</param>
    public ValidationException(string message)
        : base(message)
    {
        Errors = new Dictionary<string, string[]>();
    }
}
