namespace CatFoodManager.Core.Models.Dtos
{
    /// <summary>
    /// 最低价格数据传输对象，用于展示最低价格信息。
    /// Best price data transfer object, used for displaying best price information.
    /// </summary>
    public class BestPriceDto
    {
        /// <summary>
        /// 产品名称。
        /// Product name.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// 购买时间。
        /// Purchase time.
        /// </summary>
        public DateTime? PurchasedAt { get; set; }

        /// <summary>
        /// 实际购买价格。
        /// Actual purchase price.
        /// </summary>
        public decimal FinalPrice { get; set; }
    }
}
