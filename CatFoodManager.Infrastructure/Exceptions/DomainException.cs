namespace CatFoodManager.Infrastructure.Exceptions;

/// <summary>
/// 领域异常类，用于表示领域层发生的业务规则违规。
/// Domain exception class, used to represent business rule violations in the domain layer.
/// </summary>
public class DomainException : Exception
{
    /// <summary>
    /// 异常代码。
    /// Exception code.
    /// </summary>
    public string Code { get; }

    /// <summary>
    /// 构造函数。
    /// Constructor.
    /// </summary>
    /// <param name="code">异常代码 / Exception code</param>
    /// <param name="message">异常消息 / Exception message</param>
    public DomainException(string code, string message) : base(message)
    {
        Code = code;
    }

    /// <summary>
    /// 构造函数，包含内部异常。
    /// Constructor with inner exception.
    /// </summary>
    /// <param name="code">异常代码 / Exception code</param>
    /// <param name="message">异常消息 / Exception message</param>
    /// <param name="innerException">内部异常 / Inner exception</param>
    public DomainException(string code, string message, Exception innerException) : base(message, innerException)
    {
        Code = code;
    }
}
