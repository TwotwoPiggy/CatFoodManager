using Microsoft.Extensions.Logging;

namespace CatFoodManager.Infrastructure.Exceptions;

/// <summary>
/// 异常处理器，提供统一的异常处理逻辑。
/// Exception handler, providing unified exception handling logic.
/// </summary>
public class ExceptionHandler
{
    private readonly ILogger<ExceptionHandler> _logger;

    /// <summary>
    /// 构造函数。
    /// Constructor.
    /// </summary>
    /// <param name="logger">日志记录器 / Logger</param>
    public ExceptionHandler(ILogger<ExceptionHandler> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// 处理异常。
    /// Handles an exception.
    /// </summary>
    /// <param name="exception">异常对象 / Exception object</param>
    /// <returns>错误消息 / Error message</returns>
    public string Handle(Exception exception)
    {
        _logger.LogError(exception, "An error occurred: {Message}", exception.Message);

        return exception switch
        {
            DomainException domainEx => $"Domain error [{domainEx.Code}]: {domainEx.Message}",
            EntityNotFoundException notFoundEx => notFoundEx.Message,
            ValidationException validationEx => validationEx.Message,
            ArgumentException argEx => $"Invalid argument: {argEx.Message}",
            _ => "An unexpected error occurred. Please try again later."
        };
    }

    /// <summary>
    /// 异步处理异常。
    /// Handles an exception asynchronously.
    /// </summary>
    /// <param name="exception">异常对象 / Exception object</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    /// <returns>错误消息 / Error message</returns>
    public Task<string> HandleAsync(Exception exception, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(Handle(exception));
    }
}
