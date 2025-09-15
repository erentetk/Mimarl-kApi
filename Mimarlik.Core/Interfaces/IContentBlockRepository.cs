using Mimarlik.Core.Entities;

namespace Mimarlik.Core.Interfaces;

public interface IContentBlockRepository : IGenericRepository<ContentBlock>
{
    Task<IEnumerable<ContentBlock>> GetByProjectIdAsync(int projectId);
    Task DeleteByProjectIdAsync(int projectId);
}