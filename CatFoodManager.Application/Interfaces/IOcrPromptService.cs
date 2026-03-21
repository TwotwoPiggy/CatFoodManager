using CatFoodManager.Domain.Entities;

namespace CatFoodManager.Application.Interfaces
{
    public interface IOcrPromptService
    {
        Task<OcrPrompt?> GetDefaultAsync();
        Task<OcrPrompt?> GetByIdAsync(long id);
        Task<IEnumerable<OcrPrompt>> GetAllAsync();
        Task<OcrPrompt> CreateAsync(string name, string content, bool isDefault = false, string? description = null);
        Task UpdateAsync(long id, string name, string content, bool isDefault, string? description = null);
        Task DeleteAsync(long id);
        Task SetDefaultAsync(long id);
        Task InitializeDefaultPromptAsync();
    }
}
