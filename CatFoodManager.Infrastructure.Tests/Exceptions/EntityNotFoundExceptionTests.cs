using CatFoodManager.Infrastructure.Exceptions;
using Xunit;

namespace CatFoodManager.Infrastructure.Tests.Exceptions;

public class EntityNotFoundExceptionTests
{
    [Fact]
    public void Constructor_WithEntityTypeAndId_ShouldSetProperties()
    {
        var entityType = typeof(TestEntity);
        var id = 123;

        var exception = new EntityNotFoundException(entityType, id);

        Assert.Equal("ENTITY_NOT_FOUND", exception.Code);
        Assert.Equal(entityType, exception.EntityType);
        Assert.Equal(id, exception.Id);
        Assert.Contains("TestEntity", exception.Message);
        Assert.Contains("123", exception.Message);
    }

    [Fact]
    public void Constructor_WithStringId_ShouldSetProperties()
    {
        var entityType = typeof(TestEntity);
        var id = "test-id-123";

        var exception = new EntityNotFoundException(entityType, id);

        Assert.Equal("ENTITY_NOT_FOUND", exception.Code);
        Assert.Equal(entityType, exception.EntityType);
        Assert.Equal(id, exception.Id);
        Assert.Contains("test-id-123", exception.Message);
    }

    [Fact]
    public void EntityNotFoundException_ShouldInheritFromDomainException()
    {
        var exception = new EntityNotFoundException(typeof(TestEntity), 1);

        Assert.IsAssignableFrom<DomainException>(exception);
    }

    private class TestEntity { }
}
