using CatFoodManager.Domain.Interfaces;
using SQLite;

namespace CatFoodManager.Domain.Entities;

/// <summary>
/// 品牌实体，表示猫粮品牌信息。
/// Brand entity, representing cat food brand information.
/// </summary>
public class Brand : IEntity
{
    /// <summary>
    /// 品牌唯一标识符。
    /// Unique identifier for the brand.
    /// </summary>
    [PrimaryKey, AutoIncrement]
    public long Id { get; set; }

    /// <summary>
    /// 品牌名称。
    /// Name of the brand.
    /// </summary>
    public string? Name { get; set; }
}
