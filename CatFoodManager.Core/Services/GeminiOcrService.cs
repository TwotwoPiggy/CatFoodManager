using CatFoodManager.Core.Interfaces;
using CatFoodManager.Core.Models;
using System.Collections.Generic;
using Newtonsoft.Json;
using Twotwo.Agent.Interfaces;
using Twotwo.Agent.Types;
using Twotwo.Agent.Services;

namespace CatFoodManager.Core.Services
{
    public class GeminiOcrService : GenericServiceBase<GeminiResponseEntity>, IGeminiOcrService
    {
        private static readonly HashSet<string> AllowedExtensions = new(StringComparer.OrdinalIgnoreCase)
        {
            ".jpg",
            ".jpeg",
            ".png",
            ".bmp",
            ".webp"
        };

        private readonly IGeminiAgentService _agentService;

        public GeminiOcrService(IRepository repo, IGeminiAgentService agentService, bool needMigrate = true)
            : base(repo, needMigrate)
        {
            _agentService = agentService ?? throw new ArgumentNullException(nameof(agentService));
        }

        public async Task<bool> ValidateModelAsync(CancellationToken cancellationToken = default)
        {
            return await _agentService.ValidateModelAsync(cancellationToken);
        }

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
