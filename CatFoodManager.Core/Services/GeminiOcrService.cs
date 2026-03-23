using CatFoodManager.Core.Interfaces;
using CatFoodManager.Core.Models;
using Newtonsoft.Json;
using Twotwo.Agent.Interfaces;
using Twotwo.Agent.Types;

namespace CatFoodManager.Core.Services
{
    /// <summary>
    /// Gemini OCR服务类，使用Google Gemini AI进行图片文字识别。
    /// Gemini OCR service class, using Google Gemini AI for image text recognition.
    /// </summary>
    public class GeminiOcrService : GenericServiceBase<GeminiResponseEntity>, IGeminiOcrService
    {
        /// <summary>
        /// 允许的图片扩展名集合。
        /// Set of allowed image extensions.
        /// </summary>
        private static readonly HashSet<string> AllowedExtensions = new(StringComparer.OrdinalIgnoreCase)
        {
            ".jpg",
            ".jpeg",
            ".png",
            ".bmp",
            ".webp"
        };

        private readonly IGeminiAgentService _agentService;

        /// <summary>
        /// 构造函数。
        /// Constructor.
        /// </summary>
        /// <param name="repo">仓储实例 / Repository instance</param>
        /// <param name="agentService">Gemini代理服务实例 / Gemini agent service instance</param>
        /// <param name="needMigrate">是否需要执行数据库迁移 / Whether database migration is needed</param>
        public GeminiOcrService(IRepository repo, IGeminiAgentService agentService, bool needMigrate = true)
            : base(repo, needMigrate)
        {
            _agentService = agentService ?? throw new ArgumentNullException(nameof(agentService));
        }

        /// <summary>
        /// 验证模型是否可用。
        /// Validates whether the model is available.
        /// </summary>
        /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
        /// <returns>模型是否可用 / Whether the model is available</returns>
        public async Task<bool> ValidateModelAsync(CancellationToken cancellationToken = default)
        {
            return await _agentService.ValidateModelAsync(cancellationToken);
        }

        /// <summary>
        /// 处理图片文件夹中的图片并返回识别结果。
        /// Processes images in the folder and returns recognition results.
        /// </summary>
        /// <typeparam name="T">返回的DTO类型 / The DTO type to return</typeparam>
        /// <param name="folderPath">图片文件夹路径 / Image folder path</param>
        /// <param name="promptText">提示文本 / Prompt text</param>
        /// <returns>识别结果列表 / List of recognition results</returns>
        public async Task<List<T>> ProcessPicAsync<T>(string folderPath, string promptText)
        {
            if (string.IsNullOrEmpty(folderPath)) throw new ArgumentNullException(nameof(folderPath));
            if (!Directory.Exists(folderPath)) throw new DirectoryNotFoundException(folderPath);

            var files = Directory.GetFiles(folderPath)
                                .Where(f => AllowedExtensions.Contains(Path.GetExtension(f)))
                                .ToList();
            var fileBytes = new List<byte[]>();
            foreach (var f in files)
            {
                fileBytes.Add(File.ReadAllBytes(f));
            }

            var request = new AIRequest(Text: promptText, Files: fileBytes, MimeType: "image/jpeg");

            try
            {
                var response = await _agentService.GenerateContentAsync(request);
                if (response == null)
                {
                    return [];
                }
                var entity = new GeminiResponseEntity
                {
                    Name = response.ResponseId,
                    ResponseJson = JsonConvert.SerializeObject(response),
                    ResponseText = response.Text ?? string.Empty,
                    ModelVersion = response.ModelVersion ?? string.Empty,
                    TotalToken = response.UsageMetadata?.TotalTokenCount ?? 0,
                    PromptToken = response.UsageMetadata?.PromptTokenCount ?? 0,
                    CreatedAt = DateTime.Now
                };

                _repo.Add(entity);
                if (string.IsNullOrEmpty(response.Text))
                {
                    return [];
                }
                var jsonText = CleanJsonMarkdown(response.Text);
                var dtos = JsonConvert.DeserializeObject<List<T>>(jsonText) ?? [];
                return dtos;
            }
            catch (Exception)
            {
                return [];
            }
        }

        /// <summary>
        /// 清理JSON字符串中的Markdown标记。
        /// Cleans Markdown tags from JSON string.
        /// </summary>
        /// <param name="text">原始文本 / Original text</param>
        /// <returns>清理后的JSON字符串 / Cleaned JSON string</returns>
        private static string CleanJsonMarkdown(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return text;

            var trimmed = text.Trim();
            if (trimmed.StartsWith("```json", StringComparison.OrdinalIgnoreCase))
            {
                trimmed = trimmed.Substring(7);
            }
            else if (trimmed.StartsWith("```", StringComparison.OrdinalIgnoreCase))
            {
                trimmed = trimmed.Substring(3);
            }

            if (trimmed.EndsWith("```", StringComparison.OrdinalIgnoreCase))
            {
                trimmed = trimmed.Substring(0, trimmed.Length - 3);
            }

            return trimmed.Trim();
        }
    }
}
