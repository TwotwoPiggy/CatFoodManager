using CatFoodManager.Domain.Enums;

namespace CatFoodManager.Domain.Entities;

public class CatFood : BaseEntity
{
    public string? OrderId { get; set; }
    public ProductType FoodType { get; set; }
    public int Count { get; set; }
    public double Price { get; set; }
    public int Weights { get; set; }
    public string? PicturePath { get; set; }
    public DateTimeOffset ProductionDate { get; set; }
    public int FeededCount { get; set; }
    public long BrandId { get; set; }
    public int FactoryId { get; set; }

    public bool Feeded
    {
        get => Count == FeededCount;
        set => FeededCount = value ? Count : FeededCount;
    }
}
