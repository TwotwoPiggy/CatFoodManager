namespace CatFoodManager.Core.Interfaces
{
    public interface IGeminiOcrService
    {
        Task<bool> ValidateModelAsync(CancellationToken cancellationToken = default);
        Task<List<T>> ProcessPicAsync<T>(string folderPath, string promptText);
    }
}
