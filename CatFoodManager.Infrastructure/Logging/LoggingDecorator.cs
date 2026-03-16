using Microsoft.Extensions.Logging;

namespace CatFoodManager.Infrastructure.Logging;

public class LoggingDecorator<T> where T : class
{
    private readonly T _decorated;
    private readonly ILogger<LoggingDecorator<T>> _logger;

    public LoggingDecorator(T decorated, ILogger<LoggingDecorator<T>> logger)
    {
        _decorated = decorated ?? throw new ArgumentNullException(nameof(decorated));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<TResult> ExecuteAsync<TResult>(Func<T, Task<TResult>> func, string operationName)
    {
        _logger.LogInformation("Starting operation: {OperationName}", operationName);

        try
        {
            var result = await func(_decorated);
            _logger.LogInformation("Operation completed: {OperationName}", operationName);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Operation failed: {OperationName}", operationName);
            throw;
        }
    }

    public async Task ExecuteAsync(Func<T, Task> func, string operationName)
    {
        _logger.LogInformation("Starting operation: {OperationName}", operationName);

        try
        {
            await func(_decorated);
            _logger.LogInformation("Operation completed: {OperationName}", operationName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Operation failed: {OperationName}", operationName);
            throw;
        }
    }
}
