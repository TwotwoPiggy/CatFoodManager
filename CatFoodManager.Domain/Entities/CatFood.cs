using CatFoodManager.Domain.Enums;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace CatFoodManager.Domain.Entities;

/// <summary>
/// 猫粮实体，表示猫粮产品的详细信息�?/// Cat food entity, representing detailed information of cat food products.
/// </summary>
public class CatFood : BaseEntity
{
    /// <summary>
    /// 订单编号�?    /// Order ID.
    /// </summary>
    public string? OrderId { get; set; }

    /// <summary>
    /// 食品类型（猫粮、零食等）�?    /// Food type (cat food, snack, etc.).
    /// </summary>
    public ProductType FoodType { get; set; }

    /// <summary>
    /// 购买数量�?    /// Purchase quantity.
    /// </summary>
    public int Count { get; set; }

    /// <summary>
    /// 购买价格�?    /// Purchase price.
    /// </summary>
    public double Price { get; set; }

    /// <summary>
    /// 重量（克）�?    /// Weight in grams.
    /// </summary>
    public int Weights { get; set; }

    /// <summary>
    /// 产品图片路径�?    /// Product image path.
    /// </summary>
    public string? PicturePath { get; set; }

    /// <summary>
    /// 生产日期�?    /// Production date.
    /// </summary>
    public DateTimeOffset ProductionDate { get; set; }

    /// <summary>
    /// 已投喂数量�?    /// Number of times fed.
    /// </summary>
    public int FeededCount { get; set; }

    /// <summary>
    /// 品牌ID外键�?    /// Brand ID foreign key.
    /// </summary>
    [ForeignKey(typeof(Brand))]
    public long BrandId { get; set; }

    /// <summary>
    /// 关联的品牌实体�?    /// Associated brand entity.
    /// </summary>
    [ManyToOne]
    public Brand? Brand { get; set; }

    /// <summary>
    /// 品牌名称（从关联实体获取）�?    /// Brand name (obtained from associated entity).
    /// </summary>
    [Ignore]
    public string? BrandName => Brand?.Name;

    /// <summary>
    /// 代工厂ID外键。
    /// Factory ID foreign key.
    /// </summary>
    [ForeignKey(typeof(Factory))]
    public long FactoryId { get; set; }

    /// <summary>
    /// 关联的代工厂实体�?    /// Associated factory entity.
    /// </summary>
    [ManyToOne]
    public Factory? Factory { get; set; }

    /// <summary>
    /// 是否已投喂完毕�?    /// Whether feeding is complete.
    /// </summary>
    [Ignore]
    public bool Feeded
    {
        get => Count == FeededCount;
        set => FeededCount = value ? Count : FeededCount;
    }
}
