namespace CatFoodManager.Infrastructure.Exceptions;

public class ValidationException : DomainException
{
    public IDictionary<string, string[]> Failures { get; }

    public ValidationException(IDictionary<string, string[]> failures)
        : base("VALIDATION_ERROR", "One or more validation failures have occurred.")
    {
        Failures = failures;
    }
}
