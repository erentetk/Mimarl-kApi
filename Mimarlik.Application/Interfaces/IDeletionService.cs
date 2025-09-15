namespace Mimarlik.Application.Interfaces;

public interface IDeletionService
{
    Task<DeletionResult> CanDeleteProjectAsync(int projectId);
    Task<DeletionResult> DeleteProjectSafelyAsync(int projectId);
    Task<DeletionResult> CanDeleteCategoryAsync(int categoryId);
    Task<DeletionResult> DeleteCategorySafelyAsync(int categoryId);
    Task<DeletionResult> CanDeleteLanguageAsync(int languageId);
    Task<DeletionResult> DeleteLanguageSafelyAsync(int languageId);
    Task<DeletionResult> DeletePhotoSafelyAsync(int photoId);
}

public class DeletionResult
{
    public bool CanDelete { get; set; }
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public List<string> Dependencies { get; set; } = new();
    public List<string> DeletedItems { get; set; } = new();
}