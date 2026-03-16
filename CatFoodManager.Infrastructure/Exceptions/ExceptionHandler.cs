using Microsoft.Extensions.Logging;

namespace CatFoodManager.Infrastructure.Exceptions;

public class ExceptionHandler
{
    private readonly ILogger<ExceptionHandler> _logger;

    public ExceptionHandler(ILogger<ExceptionHandler> logger)
    {
        _logger = logger;
    }

    public void Handle(Exception exception)
    {
        switch (exception)
        {
            case DomainException domainException:
                _logger.LogWarning(domainException,
                    "Domain exception occurred: {Code} - {Message}",
                    domainException.Code, domainException.Message);
                break;
            default:
                _logger.LogError(exception, "An unhandled exception occurred");
                break;
        }
    }

    public string GetUserFriendlyMessage(Exception exception)
    {
        return exception switch
        {
            DomainException domainException => domainException.Message,
            _ => "An unexpected error occurred. Please try again later."
        };
    }
}
