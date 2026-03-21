namespace CatFoodManager.Application.Interfaces;

public interface IGeminiOcrService
{
    Task<bool> ValidateModelAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<T>> ProcessPicturesAsync<T>(string folderPath, string promptText, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ModelInfo>> GetModelsAsync(string? apiKey = null, CancellationToken cancellationToken = default);
    void ClearModelsCache(string? apiKey = null);
}

public record ModelInfo(string Name, string DisplayName);
