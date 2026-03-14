using System;

namespace CatFoodManager.Core.Models.Dtos
{
    public class BestPriceDto
    {
        public string Name { get; set; }

        public DateTime? PurchasedAt { get; set; }

        public decimal FinalPrice { get; set; }
    }
}
