using System;
using SQLite;

namespace CatFoodManager.Core.Models
{
    public class GeminiResponseEntity : BaseEntity
    {
        public string ResponseJson { get; set; }

        public string ResponseText { get; set; }

        public string ModelVersion { get; set; }

        public int PromptToken { get; set; }

        public int TotalToken { get; set; }
    }
}
