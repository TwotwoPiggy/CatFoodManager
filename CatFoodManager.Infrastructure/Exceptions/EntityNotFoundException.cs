namespace CatFoodManager.Infrastructure.Exceptions;

public class EntityNotFoundException : DomainException
{
    public Type EntityType { get; }
    public object Id { get; }

    public EntityNotFoundException(Type entityType, object id)
        : base("ENTITY_NOT_FOUND", $"Entity '{entityType.Name}' with id '{id}' was not found.")
    {
        EntityType = entityType;
        Id = id;
    }
}
