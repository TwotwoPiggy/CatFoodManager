namespace CatFoodManager.Infrastructure.Exceptions;

/// <summary>
/// 实体未找到异常，当查询的实体不存在时抛出。
/// Entity not found exception, thrown when the queried entity does not exist.
/// </summary>
public class EntityNotFoundException : Exception
{
    /// <summary>
    /// 实体类型。
    /// Entity type.
    /// </summary>
    public Type EntityType { get; }

    /// <summary>
    /// 实体ID。
    /// Entity ID.
    /// </summary>
    public object Id { get; }

    /// <summary>
    /// 构造函数。
    /// Constructor.
    /// </summary>
    /// <param name="entityType">实体类型 / Entity type</param>
    /// <param name="id">实体ID / Entity ID</param>
    public EntityNotFoundException(Type entityType, object id)
        : base($"Entity of type '{entityType.Name}' with id '{id}' was not found.")
    {
        EntityType = entityType;
        Id = id;
    }

    /// <summary>
    /// 构造函数，包含自定义消息。
    /// Constructor with custom message.
    /// </summary>
    /// <param name="entityType">实体类型 / Entity type</param>
    /// <param name="id">实体ID / Entity ID</param>
    /// <param name="message">自定义消息 / Custom message</param>
    public EntityNotFoundException(Type entityType, object id, string message)
        : base(message)
    {
        EntityType = entityType;
        Id = id;
    }

    /// <summary>
    /// 创建泛型实体未找到异常。
    /// Creates a generic entity not found exception.
    /// </summary>
    /// <typeparam name="T">实体类型 / Entity type</typeparam>
    /// <param name="id">实体ID / Entity ID</param>
    /// <returns>实体未找到异常 / Entity not found exception</returns>
    public static EntityNotFoundException For<T>(object id)
    {
        return new EntityNotFoundException(typeof(T), id);
    }
}
