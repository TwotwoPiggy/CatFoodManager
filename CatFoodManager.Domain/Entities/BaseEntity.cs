using CatFoodManager.Domain.Interfaces;
using SQLite;

namespace CatFoodManager.Domain.Entities;

public abstract class BaseEntity : IEntity
{
    [PrimaryKey, AutoIncrement]
    public long Id { get; set; }
    public string? Name { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
    public DateTimeOffset? PurchasedAt { get; set; }

    private readonly List<Func<Task>> _domainEvents = new();
    public IReadOnlyCollection<Func<Task>> DomainEvents => _domainEvents.AsReadOnly();

    public void AddDomainEvent(Func<Task> eventHandler)
    {
        _domainEvents.Add(eventHandler);
    }

    public void RemoveDomainEvent(Func<Task> eventHandler)
    {
        _domainEvents.Remove(eventHandler);
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}
