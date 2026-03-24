namespace CatFoodManager.Core.Interfaces
{
    /// <summary>
    /// Gemini OCR服务接口，提供图片处理和文字识别功能。
    /// Gemini OCR service interface, providing image processing and text recognition functionality.
    /// </summary>
    public interface IGeminiOcrService
    {
        /// <summary>
        /// 验证模型是否可用。
        /// Validates whether the model is available.
        /// </summary>
        /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
        /// <returns>模型是否可用 / Whether the model is available</returns>
        Task<bool> ValidateModelAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// 处理图片文件夹中的图片并返回识别结果。
        /// Processes images in the folder and returns recognition results.
        /// </summary>
        /// <typeparam name="T">返回的DTO类型 / The DTO type to return</typeparam>
        /// <param name="folderPath">图片文件夹路径 / Image folder path</param>
        /// <param name="promptText">提示文本 / Prompt text</param>
        /// <returns>识别结果列表 / List of recognition results</returns>
        Task<List<T>> ProcessPicAsync<T>(string folderPath, string promptText);
    }
}
