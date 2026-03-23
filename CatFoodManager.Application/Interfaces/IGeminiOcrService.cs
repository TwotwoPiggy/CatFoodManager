namespace CatFoodManager.Application.Interfaces;

public interface IGeminiOcrService
{
    Task<bool> ValidateModelAsync(CancellationToken cancellationToken = default);
    Task<ProcessPicturesResult<T>> ProcessPicturesAsync<T>(string folderPath, string promptText, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ModelInfo>> GetModelsAsync(string? apiKey = null, CancellationToken cancellationToken = default);
    void ClearModelsCache(string? apiKey = null);

    IReadOnlyList<FailedResponseCacheItem> GetFailedResponses();
    Task<bool> RetrySaveResponseAsync(string responseId);
    void RemoveFailedResponse(string responseId);
}

public record ModelInfo(string Name, string DisplayName);

public record ProcessPicturesResult<T>(
    List<T> Items,
    string? ResponseId
);

public record FailedResponseCacheItem(
    string ResponseId,
    long TaskId,
    string FolderPath,
    string PromptText,
    DateTime FailedAt,
    string? ErrorMessage
);
