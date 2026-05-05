namespace CatFoodManager.Core.Models.Dtos
{
    /// <summary>
    /// 最低价格同步数据传输对象，用于数据同步操作�?    /// Best price sync data transfer object, used for data synchronization operations.
    /// </summary>
    public class BestPriceSyncDto
    {
        /// <summary>
        /// 图片唯一标识，用于将 OCR 结果与输入图片关联�?        /// </summary>
        public string ImageId { get; set; } = string.Empty;

        /// <summary>
        /// 产品名称�?        /// Product name.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// 产品类型（整数表示）�?        /// Product type (represented as integer).
        /// </summary>
        public int Type { get; set; }

        /// <summary>
        /// 购买平台（整数表示）�?        /// Purchase platform (represented as integer).
        /// </summary>
        public int Platform { get; set; }

        /// <summary>
        /// 历史最低价格�?        /// Historical lowest price.
        /// </summary>
        public decimal LowestPrice { get; set; }

        /// <summary>
        /// 实际购买价格�?        /// Actual purchase price.
        /// </summary>
        public decimal? FinalPrice { get; set; }

        /// <summary>
        /// 产品图片路径�?        /// Product image path.
        /// </summary>
        public string? PicturePath { get; set; }

        /// <summary>
        /// 代工厂名称�?        /// Factory name.
        /// </summary>
        public string? FactoryName { get; set; }

        /// <summary>
        /// 是否有检测报告�?        /// Whether there is a test report.
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
        /// 购买时间。
        /// Purchase time.
        /// </summary>
        public DateTime? PurchasedAt { get; set; }
    }
}
