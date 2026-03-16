namespace CatFoodManager.Application.Interfaces;

public interface IGeminiOcrService
{
    Task<bool> ValidateModelAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<T>> ProcessPicturesAsync<T>(string folderPath, string promptText, CancellationToken cancellationToken = default);
}
