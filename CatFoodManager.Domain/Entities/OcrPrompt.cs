using CatFoodManager.Domain.Interfaces;

namespace CatFoodManager.Domain.Entities
{
    public class OcrPrompt : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public bool IsDefault { get; set; }
        public string? Description { get; set; }
    }
}
