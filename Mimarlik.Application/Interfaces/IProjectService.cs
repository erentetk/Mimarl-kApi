using Mimarlik.Application.DTOs;

namespace Mimarlik.Application.Interfaces;

public interface IProjectService
{
    Task<IEnumerable<ProjectDto>> GetAllAsync();
    Task<IEnumerable<ProjectDto>> GetPublishedAsync();
    Task<IEnumerable<ProjectDto>> GetFeaturedAsync();
    Task<IEnumerable<ProjectDto>> GetByCategoryIdAsync(int categoryId);
    Task<ProjectDto?> GetByIdAsync(int id);
    Task<ProjectDto?> GetBySlugAsync(string slug);
    Task<ProjectDto?> GetWithDetailsAsync(int id);
    Task<ProjectDto> CreateAsync(CreateProjectDto createProjectDto);
    Task<ProjectDto> UpdateAsync(int id, UpdateProjectDto updateProjectDto);
    Task<bool> DeleteAsync(int id);
    Task<string> GenerateSlugAsync(string title, int? excludeId = null);
}