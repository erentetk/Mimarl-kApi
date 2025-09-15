using Mimarlik.Core.Entities;
using Mimarlik.Core.Enums;

namespace Mimarlik.Core.Interfaces;

public interface ICategoryRepository : IGenericRepository<Category>
{
    Task<IEnumerable<Category>> GetAllWithChildrenAsync();
    Task<IEnumerable<Category>> GetByParentIdAsync(int? parentId);
    Task<IEnumerable<Category>> GetByStatusAsync(ContentStatus status);
    Task<Category?> GetBySlugAsync(string slug);
    Task<bool> HasChildrenAsync(int categoryId);
    Task<bool> HasProjectsAsync(int categoryId);
}