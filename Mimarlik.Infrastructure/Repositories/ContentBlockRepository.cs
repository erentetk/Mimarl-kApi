using Microsoft.EntityFrameworkCore;
using Mimarlik.Core.Entities;
using Mimarlik.Core.Interfaces;
using Mimarlik.Infrastructure.Data;

namespace Mimarlik.Infrastructure.Repositories;

public class ContentBlockRepository : GenericRepository<ContentBlock>, IContentBlockRepository
{
    public ContentBlockRepository(MimarlikDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<ContentBlock>> GetByProjectIdAsync(int projectId)
    {
        return await _dbSet
            .Where(cb => cb.ProjectId == projectId)
            .OrderBy(cb => cb.SortOrder)
            .ToListAsync();
    }

    public async Task DeleteByProjectIdAsync(int projectId)
    {
        var contentBlocks = await _dbSet
            .Where(cb => cb.ProjectId == projectId)
            .ToListAsync();
            
        _dbSet.RemoveRange(contentBlocks);
    }
}