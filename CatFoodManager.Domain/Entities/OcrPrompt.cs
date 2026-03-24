using CatFoodManager.Domain.Interfaces;

namespace CatFoodManager.Domain.Entities
{
    /// <summary>
    /// OCR提示词实体，用于存储OCR识别的提示词模板。
    /// OCR prompt entity, used to store OCR recognition prompt templates.
    /// </summary>
    public class OcrPrompt : BaseEntity
    {
        /// <summary>
        /// 提示词名称。
        /// Prompt name.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// 提示词内容。
        /// Prompt content.
        /// </summary>
        public string Content { get; set; } = string.Empty;

        /// <summary>
        /// 是否为默认提示词。
        /// Whether this is the default prompt.
        /// </summary>
        public bool IsDefault { get; set; }

        /// <summary>
        /// 提示词描述。
        /// Prompt description.
        /// </summary>
        public string? Description { get; set; }
    }
}
