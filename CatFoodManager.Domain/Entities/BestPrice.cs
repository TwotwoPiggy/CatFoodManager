using CatFoodManager.Domain.Enums;

namespace CatFoodManager.Domain.Entities;

public class BestPrice : BaseEntity
{
    public ProductType Type { get; set; }
    public PlatformType Platform { get; set; }
    public decimal LowestPrice { get; set; }
    public bool HasPurchased { get; set; }
    public decimal? FinalPrice { get; set; }
    public string? PicturePath { get; set; }
    public string? FactoryName { get; set; }
    public bool HasTestReport { get; set; }
    public bool IsWorthRepurchasing { get; set; }
}
