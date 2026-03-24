namespace CatFoodManager.Domain.Interfaces;

/// <summary>
/// 实体接口，定义实体的基本标识属性。
/// Entity interface, defining the basic identity property of an entity.
/// </summary>
public interface IEntity
{
    /// <summary>
    /// 实体唯一标识符。
    /// Unique identifier for the entity.
    /// </summary>
    long Id { get; set; }
}
