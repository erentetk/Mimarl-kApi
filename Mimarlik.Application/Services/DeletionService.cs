using Mimarlik.Application.Interfaces;
using Mimarlik.Core.Interfaces;

namespace Mimarlik.Application.Services;

public class DeletionService : IDeletionService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFileService _fileService;

    public DeletionService(IUnitOfWork unitOfWork, IFileService fileService)
    {
        _unitOfWork = unitOfWork;
        _fileService = fileService;
    }

    public async Task<DeletionResult> CanDeleteProjectAsync(int projectId)
    {
        var result = new DeletionResult();
        
        var project = await _unitOfWork.Projects.GetWithDetailsAsync(projectId);
        if (project == null)
        {
            result.CanDelete = false;
            result.Message = "Project not found";
            return result;
        }

        result.CanDelete = true;
        result.Message = "Project can be safely deleted";

        // List what will be deleted
        result.Dependencies.Add($"Project: {project.Title}");
        
        if (project.Photos.Any())
        {
            result.Dependencies.Add($"{project.Photos.Count} photos will be deleted");
            foreach (var photo in project.Photos.Take(5)) // Show first 5
            {
                result.Dependencies.Add($"  - {photo.FileName}");
            }
            if (project.Photos.Count > 5)
            {
                result.Dependencies.Add($"  - ... and {project.Photos.Count - 5} more photos");
            }
        }

        if (project.ContentBlocks.Any())
        {
            result.Dependencies.Add($"{project.ContentBlocks.Count} content blocks will be deleted");
        }

        // Check for translations
        var translations = await _unitOfWork.Translations.GetByEntityAsync("Project", projectId);
        if (translations.Any())
        {
            result.Dependencies.Add($"{translations.Count()} translations will be deleted");
        }

        return result;
    }

    public async Task<DeletionResult> DeleteProjectSafelyAsync(int projectId)
    {
        var result = new DeletionResult();
        
        var project = await _unitOfWork.Projects.GetWithDetailsAsync(projectId);
        if (project == null)
        {
            result.Success = false;
            result.Message = "Project not found";
            return result;
        }

        await _unitOfWork.BeginTransactionAsync();
        try
        {
            // Delete photos and their files
            var photos = project.Photos.ToList();
            foreach (var photo in photos)
            {
                await _fileService.DeleteFileAsync(photo.FilePath);
                result.DeletedItems.Add($"Photo file: {photo.FileName}");
            }
            await _unitOfWork.Photos.DeleteRangeAsync(photos);
            result.DeletedItems.Add($"{photos.Count} photo records");

            // Delete content blocks
            var contentBlocks = project.ContentBlocks.ToList();
            await _unitOfWork.ContentBlocks.DeleteRangeAsync(contentBlocks);
            result.DeletedItems.Add($"{contentBlocks.Count} content blocks");

            // Delete translations
            await _unitOfWork.Translations.DeleteByEntityAsync("Project", projectId);
            result.DeletedItems.Add("Project translations");

            // Delete the project itself
            await _unitOfWork.Projects.DeleteAsync(project);
            result.DeletedItems.Add($"Project: {project.Title}");

            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitTransactionAsync();

            result.Success = true;
            result.Message = "Project deleted successfully";
            return result;
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync();
            result.Success = false;
            result.Message = $"Failed to delete project: {ex.Message}";
            return result;
        }
    }

    public async Task<DeletionResult> CanDeleteCategoryAsync(int categoryId)
    {
        var result = new DeletionResult();
        
        var category = await _unitOfWork.Categories.GetByIdAsync(categoryId);
        if (category == null)
        {
            result.CanDelete = false;
            result.Message = "Category not found";
            return result;
        }

        // Check for child categories
        var hasChildren = await _unitOfWork.Categories.HasChildrenAsync(categoryId);
        if (hasChildren)
        {
            result.CanDelete = false;
            result.Message = "Category has child categories and cannot be deleted";
            var children = await _unitOfWork.Categories.GetByParentIdAsync(categoryId);
            foreach (var child in children.Take(5))
            {
                result.Dependencies.Add($"Child category: {child.Title}");
            }
            return result;
        }

        // Check for projects
        var hasProjects = await _unitOfWork.Categories.HasProjectsAsync(categoryId);
        if (hasProjects)
        {
            result.CanDelete = false;
            result.Message = "Category has associated projects and cannot be deleted";
            var projects = await _unitOfWork.Projects.GetByCategoryIdAsync(categoryId);
            foreach (var project in projects.Take(5))
            {
                result.Dependencies.Add($"Project: {project.Title}");
            }
            return result;
        }

        result.CanDelete = true;
        result.Message = "Category can be safely deleted";
        return result;
    }

    public async Task<DeletionResult> DeleteCategorySafelyAsync(int categoryId)
    {
        var canDeleteResult = await CanDeleteCategoryAsync(categoryId);
        if (!canDeleteResult.CanDelete)
        {
            return canDeleteResult;
        }

        var result = new DeletionResult();
        var category = await _unitOfWork.Categories.GetByIdAsync(categoryId);

        await _unitOfWork.BeginTransactionAsync();
        try
        {
            // Delete translations
            await _unitOfWork.Translations.DeleteByEntityAsync("Category", categoryId);
            result.DeletedItems.Add("Category translations");

            // Delete the category
            await _unitOfWork.Categories.DeleteAsync(category!);
            result.DeletedItems.Add($"Category: {category!.Title}");

            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitTransactionAsync();

            result.Success = true;
            result.Message = "Category deleted successfully";
            return result;
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync();
            result.Success = false;
            result.Message = $"Failed to delete category: {ex.Message}";
            return result;
        }
    }

    public async Task<DeletionResult> CanDeleteLanguageAsync(int languageId)
    {
        var result = new DeletionResult();
        
        var language = await _unitOfWork.Languages.GetByIdAsync(languageId);
        if (language == null)
        {
            result.CanDelete = false;
            result.Message = "Language not found";
            return result;
        }

        // Check if this is the default language
        if (language.IsDefault)
        {
            var allLanguages = await _unitOfWork.Languages.GetAllAsync();
            if (allLanguages.Count() <= 1)
            {
                result.CanDelete = false;
                result.Message = "Cannot delete the last language";
                return result;
            }
            result.CanDelete = false;
            result.Message = "Cannot delete the default language. Please set another language as default first.";
            return result;
        }

        // Check for translations
        var translations = await _unitOfWork.Translations.GetByLanguageAsync(languageId);
        if (translations.Any())
        {
            result.Dependencies.Add($"{translations.Count()} translations will be deleted");
        }

        result.CanDelete = true;
        result.Message = "Language can be safely deleted";
        return result;
    }

    public async Task<DeletionResult> DeleteLanguageSafelyAsync(int languageId)
    {
        var canDeleteResult = await CanDeleteLanguageAsync(languageId);
        if (!canDeleteResult.CanDelete)
        {
            return canDeleteResult;
        }

        var result = new DeletionResult();
        var language = await _unitOfWork.Languages.GetByIdAsync(languageId);

        await _unitOfWork.BeginTransactionAsync();
        try
        {
            // Delete all translations for this language
            var translations = await _unitOfWork.Translations.GetByLanguageAsync(languageId);
            await _unitOfWork.Translations.DeleteRangeAsync(translations);
            result.DeletedItems.Add($"{translations.Count()} translations");

            // Delete the language
            await _unitOfWork.Languages.DeleteAsync(language!);
            result.DeletedItems.Add($"Language: {language!.Name}");

            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitTransactionAsync();

            result.Success = true;
            result.Message = "Language deleted successfully";
            return result;
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync();
            result.Success = false;
            result.Message = $"Failed to delete language: {ex.Message}";
            return result;
        }
    }

    public async Task<DeletionResult> DeletePhotoSafelyAsync(int photoId)
    {
        var result = new DeletionResult();
        
        var photo = await _unitOfWork.Photos.GetByIdAsync(photoId);
        if (photo == null)
        {
            result.Success = false;
            result.Message = "Photo not found";
            return result;
        }

        await _unitOfWork.BeginTransactionAsync();
        try
        {
            // Delete the physical file
            await _fileService.DeleteFileAsync(photo.FilePath);
            result.DeletedItems.Add($"Photo file: {photo.FileName}");

            // Delete translations
            await _unitOfWork.Translations.DeleteByEntityAsync("Photo", photoId);
            result.DeletedItems.Add("Photo translations");

            // Delete the photo record
            await _unitOfWork.Photos.DeleteAsync(photo);
            result.DeletedItems.Add($"Photo record: {photo.FileName}");

            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitTransactionAsync();

            result.Success = true;
            result.Message = "Photo deleted successfully";
            return result;
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync();
            result.Success = false;
            result.Message = $"Failed to delete photo: {ex.Message}";
            return result;
        }
    }
}