using Mimarlik.Core.Entities;
using Mimarlik.Core.Enums;

namespace Mimarlik.Core.Interfaces;

public interface IProjectRepository : IGenericRepository<Project>
{
    Task<IEnumerable<Project>> GetByCategoryIdAsync(int categoryId);
    Task<IEnumerable<Project>> GetByStatusAsync(ContentStatus status);
    Task<Project?> GetBySlugAsync(string slug);
    Task<Project?> GetWithDetailsAsync(int id);
    Task<IEnumerable<Project>> GetFeaturedAsync();
    Task<IEnumerable<Project>> GetPublishedAsync();
    Task<bool> SlugExistsAsync(string slug, int? excludeId = null);
}