using CatFoodManager.Domain.Enums;

namespace CatFoodManager.Domain.Entities;

/// <summary>
/// 最低价格实体，记录产品的历史最低价格信息�?/// Best price entity, recording historical lowest price information for products.
/// </summary>
public class BestPrice : BaseEntity
{
    /// <summary>
    /// 产品类型（猫粮、零食等）�?    /// Product type (cat food, snack, etc.).
    /// </summary>
    public ProductType Type { get; set; }

    /// <summary>
    /// 购买平台（京东、淘宝等）�?    /// Purchase platform (JD, Taobao, etc.).
    /// </summary>
    public PlatformType Platform { get; set; }

    /// <summary>
    /// 历史最低价格�?    /// Historical lowest price.
    /// </summary>
    public decimal LowestPrice { get; set; }

    /// <summary>
    /// 是否已购买�?    /// Whether the product has been purchased.
    /// </summary>
    public bool HasPurchased { get; set; }

    /// <summary>
    /// 实际购买价格�?    /// Actual purchase price.
    /// </summary>
    public decimal? FinalPrice { get; set; }

    /// <summary>
    /// 产品图片路径�?    /// Product image path.
    /// </summary>
    public string? PicturePath { get; set; }

    /// <summary>
    /// 代工厂名称�?    /// Factory name.
    /// </summary>
    public string? FactoryName { get; set; }

    /// <summary>
    /// 是否有检测报告�?    /// Whether there is a test report.
    /// </summary>
    public bool HasTestReport { get; set; }

    /// <summary>
    /// 是否值得回购。
    /// Whether it is worth repurchasing.
    /// </summary>
    public bool IsWorthRepurchasing { get; set; }

    /// <summary>
    /// 检测机构名称。
    /// Testing agency name.
    /// </summary>
    public string? TestingAgency { get; set; }

    /// <summary>
    /// 构造函数，初始化创建时间为当前时间。
    /// Constructor, initializing creation time to current time.
    /// </summary>
    public BestPrice()
    {
        CreatedAt = DateTimeOffset.Now;
    }
}
