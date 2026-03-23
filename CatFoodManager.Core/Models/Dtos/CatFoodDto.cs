using CatFoodManager.Core.Statics;

namespace CatFoodManager.Core.Models.Dtos
{
    public class CatFoodDto
    {
        public string Name { get; set; } = string.Empty;
        public string OrderId { get; set; } = string.Empty;
        public int FoodType { get; set; }
        public int Count { get; set; }
        public double Price { get; set; }
        public int Weights { get; set; }
        public string PicturePath { get; set; } = string.Empty;
        public DateTime? ProductionDate { get; set; }
        public string BrandName { get; set; } = string.Empty;
        public PlatformType Platform { get; set; }
    }
}
