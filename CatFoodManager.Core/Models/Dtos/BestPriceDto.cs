namespace CatFoodManager.Core.Models.Dtos
{
    public class BestPriceDto
    {
        public string Name { get; set; } = string.Empty;

        public DateTime? PurchasedAt { get; set; }

        public decimal FinalPrice { get; set; }
    }
}
