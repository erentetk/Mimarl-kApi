using Microsoft.EntityFrameworkCore;
using Mimarlik.Core.Entities;
using Mimarlik.Core.Enums;
using Mimarlik.Core.Interfaces;
using Mimarlik.Infrastructure.Data;

namespace Mimarlik.Infrastructure.Repositories;

public class ProjectRepository : GenericRepository<Project>, IProjectRepository
{
    public ProjectRepository(MimarlikDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Project>> GetByCategoryIdAsync(int categoryId)
    {
        return await _dbSet
            .Where(p => p.CategoryId == categoryId)
            .Include(p => p.Photos.Where(ph => ph.Status == ContentStatus.Published))
            .OrderBy(p => p.SortOrder)
            .ThenByDescending(p => p.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Project>> GetByStatusAsync(ContentStatus status)
    {
        return await _dbSet
            .Where(p => p.Status == status)
            .Include(p => p.Category)
            .Include(p => p.Photos.Where(ph => ph.Status == ContentStatus.Published))
            .OrderBy(p => p.SortOrder)
            .ThenByDescending(p => p.CreatedAt)
            .ToListAsync();
    }

    public async Task<Project?> GetBySlugAsync(string slug)
    {
        return await _dbSet
            .Include(p => p.Category)
            .Include(p => p.Photos.Where(ph => ph.Status == ContentStatus.Published).OrderBy(ph => ph.SortOrder))
            .Include(p => p.ContentBlocks.Where(cb => cb.Status == ContentStatus.Published).OrderBy(cb => cb.SortOrder))
            .FirstOrDefaultAsync(p => p.Slug == slug);
    }

    public async Task<Project?> GetWithDetailsAsync(int id)
    {
        return await _dbSet
            .Include(p => p.Category)
            .Include(p => p.Photos.OrderBy(ph => ph.SortOrder))
            .Include(p => p.ContentBlocks.OrderBy(cb => cb.SortOrder))
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<IEnumerable<Project>> GetFeaturedAsync()
    {
        return await _dbSet
            .Where(p => p.IsFeatured && p.Status == ContentStatus.Published)
            .Include(p => p.Photos.Where(ph => ph.Status == ContentStatus.Published).OrderBy(ph => ph.SortOrder))
            .OrderBy(p => p.SortOrder)
            .ThenByDescending(p => p.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Project>> GetPublishedAsync()
    {
        return await _dbSet
            .Where(p => p.Status == ContentStatus.Published)
            .Include(p => p.Category)
            .Include(p => p.Photos.Where(ph => ph.Status == ContentStatus.Published).OrderBy(ph => ph.SortOrder))
            .OrderBy(p => p.SortOrder)
            .ThenByDescending(p => p.CreatedAt)
            .ToListAsync();
    }

    public async Task<bool> SlugExistsAsync(string slug, int? excludeId = null)
    {
        var query = _dbSet.Where(p => p.Slug == slug);
        
        if (excludeId.HasValue)
            query = query.Where(p => p.Id != excludeId.Value);
            
        return await query.AnyAsync();
    }
}