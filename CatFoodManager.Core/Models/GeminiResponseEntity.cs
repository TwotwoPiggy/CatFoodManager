using SQLite;

namespace CatFoodManager.Core.Models
{
    public class GeminiResponseEntity : BaseEntity
    {
        public string ResponseJson { get; set; } = string.Empty;

        public string ResponseText { get; set; } = string.Empty;

        public string ModelVersion { get; set; } = string.Empty;

        public int PromptToken { get; set; }

        public int TotalToken { get; set; }
    }
}
