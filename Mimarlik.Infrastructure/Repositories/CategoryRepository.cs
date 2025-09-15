using Microsoft.EntityFrameworkCore;
using Mimarlik.Core.Entities;
using Mimarlik.Core.Enums;
using Mimarlik.Core.Interfaces;
using Mimarlik.Infrastructure.Data;

namespace Mimarlik.Infrastructure.Repositories;

public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
{
    public CategoryRepository(MimarlikDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Category>> GetAllWithChildrenAsync()
    {
        return await _dbSet
            .Include(c => c.Children)
            .OrderBy(c => c.SortOrder)
            .ThenBy(c => c.Title)
            .ToListAsync();
    }

    public async Task<IEnumerable<Category>> GetByParentIdAsync(int? parentId)
    {
        return await _dbSet
            .Where(c => c.ParentId == parentId)
            .OrderBy(c => c.SortOrder)
            .ThenBy(c => c.Title)
            .ToListAsync();
    }

    public async Task<IEnumerable<Category>> GetByStatusAsync(ContentStatus status)
    {
        return await _dbSet
            .Where(c => c.Status == status)
            .OrderBy(c => c.SortOrder)
            .ThenBy(c => c.Title)
            .ToListAsync();
    }

    public async Task<Category?> GetBySlugAsync(string slug)
    {
        return await _dbSet
            .Include(c => c.Children)
            .Include(c => c.Projects.Where(p => p.Status == ContentStatus.Published))
            .FirstOrDefaultAsync(c => c.Slug == slug);
    }

    public async Task<bool> HasChildrenAsync(int categoryId)
    {
        return await _dbSet.AnyAsync(c => c.ParentId == categoryId);
    }

    public async Task<bool> HasProjectsAsync(int categoryId)
    {
        return await _context.Projects.AnyAsync(p => p.CategoryId == categoryId);
    }
}