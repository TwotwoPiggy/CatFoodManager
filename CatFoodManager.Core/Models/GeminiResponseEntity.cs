using SQLite;

namespace CatFoodManager.Core.Models
{
    /// <summary>
    /// Gemini响应实体，存储AI模型的响应数据。
    /// Gemini response entity, storing AI model response data.
    /// </summary>
    public class GeminiResponseEntity : BaseEntity
    {
        /// <summary>
        /// 响应的JSON格式数据。
        /// Response data in JSON format.
        /// </summary>
        public string ResponseJson { get; set; } = string.Empty;

        /// <summary>
        /// 响应的文本内容。
        /// Response text content.
        /// </summary>
        public string ResponseText { get; set; } = string.Empty;

        /// <summary>
        /// 使用的模型版本。
        /// Model version used.
        /// </summary>
        public string ModelVersion { get; set; } = string.Empty;

        /// <summary>
        /// 提示词消耗的Token数量。
        /// Number of tokens consumed by the prompt.
        /// </summary>
        public int PromptToken { get; set; }

        /// <summary>
        /// 总Token消耗数量。
        /// Total number of tokens consumed.
        /// </summary>
        public int TotalToken { get; set; }
    }
}
