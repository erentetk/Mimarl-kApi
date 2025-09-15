using Mimarlik.Application.DTOs;

namespace Mimarlik.Application.Interfaces;

public interface ICategoryService
{
    Task<IEnumerable<CategoryDto>> GetAllAsync();
    Task<IEnumerable<CategoryDto>> GetAllWithChildrenAsync();
    Task<IEnumerable<CategoryDto>> GetByParentIdAsync(int? parentId);
    Task<CategoryDto?> GetByIdAsync(int id);
    Task<CategoryDto?> GetBySlugAsync(string slug);
    Task<CategoryDto> CreateAsync(CreateCategoryDto createCategoryDto);
    Task<CategoryDto> UpdateAsync(int id, UpdateCategoryDto updateCategoryDto);
    Task<bool> DeleteAsync(int id);
    Task<bool> CanDeleteAsync(int id);
}