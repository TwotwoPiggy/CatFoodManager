using System.Reflection;
using Microsoft.Extensions.Logging;

namespace CatFoodManager.Infrastructure.Logging;

/// <summary>
/// 日志装饰器，为服务添加日志记录功能。
/// Logging decorator, adding logging functionality to services.
/// </summary>
/// <typeparam name="TService">服务接口类型 / Service interface type</typeparam>
public class LoggingDecorator<TService> : DispatchProxy where TService : class
{
    private TService? _decorated;
    private ILogger<TService>? _logger;

    /// <summary>
    /// 创建装饰器实例。
    /// Creates a decorator instance.
    /// </summary>
    /// <param name="decorated">被装饰的服务实例 / Decorated service instance</param>
    /// <param name="logger">日志记录器 / Logger</param>
    /// <returns>装饰后的服务实例 / Decorated service instance</returns>
    public static TService Create(TService decorated, ILogger<TService> logger)
    {
        var proxy = Create<TService, LoggingDecorator<TService>>() as LoggingDecorator<TService>;
        if (proxy != null)
        {
            proxy._decorated = decorated;
            proxy._logger = logger;
        }
        return (proxy as TService)!;
    }

    /// <summary>
    /// 调用方法。
    /// Invokes a method.
    /// </summary>
    /// <param name="method">方法信息 / Method info</param>
    /// <param name="args">方法参数 / Method arguments</param>
    /// <returns>方法返回值 / Method return value</returns>
    protected override object? Invoke(MethodInfo? method, object?[]? args)
    {
        if (method == null || _decorated == null || _logger == null)
        {
            return null;
        }

        var methodName = method.Name;

        try
        {
            _logger.LogDebug("Entering {MethodName} with args: {Args}", methodName, args);

            var result = method.Invoke(_decorated, args);

            if (result is Task task)
            {
                return HandleAsyncTask(task, methodName);
            }

            _logger.LogDebug("Exiting {MethodName}", methodName);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in {MethodName}: {Message}", methodName, ex.InnerException?.Message ?? ex.Message);
            throw;
        }
    }

    /// <summary>
    /// 处理异步任务。
    /// Handles an async task.
    /// </summary>
    /// <param name="task">异步任务 / Async task</param>
    /// <param name="methodName">方法名称 / Method name</param>
    /// <returns>异步任务 / Async task</returns>
    private async Task HandleAsyncTask(Task task, string methodName)
    {
        try
        {
            await task.ConfigureAwait(false);
            _logger.LogDebug("Exiting {MethodName}", methodName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in {MethodName}: {Message}", methodName, ex.Message);
            throw;
        }
    }
}
