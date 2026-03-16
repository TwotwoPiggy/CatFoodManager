using CatFoodManager.Domain.Entities;
using Xunit;

namespace CatFoodManager.Tests.Domain.Entities;

public class BaseEntityTests
{
    [Fact]
    public void Constructor_ShouldInitializeWithDefaultValues()
    {
        var entity = new TestEntity();

        Assert.Equal(0, entity.Id);
        Assert.Equal(default, entity.CreatedAt);
        Assert.Null(entity.UpdatedAt);
        Assert.Null(entity.PurchasedAt);
        Assert.Null(entity.Name);
        Assert.Empty(entity.DomainEvents);
    }

    [Fact]
    public void Properties_ShouldBeSettable()
    {
        var now = DateTimeOffset.Now;
        var entity = new TestEntity
        {
            Id = 1,
            Name = "Test Name",
            CreatedAt = now,
            UpdatedAt = now.AddDays(1),
            PurchasedAt = now.AddDays(2)
        };

        Assert.Equal(1, entity.Id);
        Assert.Equal("Test Name", entity.Name);
        Assert.Equal(now, entity.CreatedAt);
        Assert.Equal(now.AddDays(1), entity.UpdatedAt);
        Assert.Equal(now.AddDays(2), entity.PurchasedAt);
    }

    [Fact]
    public void AddDomainEvent_ShouldAddEventToCollection()
    {
        var entity = new TestEntity();
        Func<Task> eventHandler = () => Task.CompletedTask;

        entity.AddDomainEvent(eventHandler);

        Assert.Single(entity.DomainEvents);
        Assert.Contains(eventHandler, entity.DomainEvents);
    }

    [Fact]
    public void RemoveDomainEvent_ShouldRemoveEventFromCollection()
    {
        var entity = new TestEntity();
        Func<Task> eventHandler = () => Task.CompletedTask;
        entity.AddDomainEvent(eventHandler);

        entity.RemoveDomainEvent(eventHandler);

        Assert.Empty(entity.DomainEvents);
    }

    [Fact]
    public void ClearDomainEvents_ShouldRemoveAllEvents()
    {
        var entity = new TestEntity();
        entity.AddDomainEvent(() => Task.CompletedTask);
        entity.AddDomainEvent(() => Task.CompletedTask);
        entity.AddDomainEvent(() => Task.CompletedTask);

        entity.ClearDomainEvents();

        Assert.Empty(entity.DomainEvents);
    }

    private class TestEntity : BaseEntity { }
}
