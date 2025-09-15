using Microsoft.EntityFrameworkCore;
using Mimarlik.Core.Entities;
using Mimarlik.Core.Enums;
using Mimarlik.Core.Interfaces;
using Mimarlik.Infrastructure.Data;

namespace Mimarlik.Infrastructure.Repositories;

public class PhotoRepository : GenericRepository<Photo>, IPhotoRepository
{
    public PhotoRepository(MimarlikDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Photo>> GetByProjectIdAsync(int projectId)
    {
        return await _dbSet
            .Where(p => p.ProjectId == projectId)
            .OrderBy(p => p.SortOrder)
            .ThenBy(p => p.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Photo>> GetHomepageSliderPhotosAsync()
    {
        return await _dbSet
            .Where(p => p.IsHomepageSlider && p.Status == ContentStatus.Published)
            .Include(p => p.Project)
            .OrderBy(p => p.SortOrder)
            .ThenBy(p => p.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Photo>> GetByStatusAsync(ContentStatus status)
    {
        return await _dbSet
            .Where(p => p.Status == status)
            .Include(p => p.Project)
            .OrderBy(p => p.SortOrder)
            .ThenByDescending(p => p.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Photo>> GetOrphanPhotosAsync()
    {
        return await _dbSet
            .Where(p => p.ProjectId == null)
            .OrderBy(p => p.SortOrder)
            .ThenByDescending(p => p.CreatedAt)
            .ToListAsync();
    }

    public async Task DeletePhotoFileAsync(string filePath)
    {
        // This will be implemented in the file service
        await Task.CompletedTask;
    }

    public async Task<bool> IsUsedInSliderAsync(int photoId)
    {
        return await _dbSet.AnyAsync(p => p.Id == photoId && p.IsHomepageSlider);
    }
}