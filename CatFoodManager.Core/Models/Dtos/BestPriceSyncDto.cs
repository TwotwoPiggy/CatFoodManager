namespace CatFoodManager.Core.Models.Dtos
{
    public class BestPriceSyncDto
    {
        public string Name { get; set; } = string.Empty;
        public int Type { get; set; }
        public int Platform { get; set; }
        public decimal LowestPrice { get; set; }
        public decimal? FinalPrice { get; set; }
        public string? PicturePath { get; set; }
        public string? FactoryName { get; set; }
        public bool HasTestReport { get; set; }
        public bool IsWorthRepurchasing { get; set; }
        public DateTime? PurchasedAt { get; set; }
    }
}
