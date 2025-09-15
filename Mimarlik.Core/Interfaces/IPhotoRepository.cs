using Mimarlik.Core.Entities;
using Mimarlik.Core.Enums;

namespace Mimarlik.Core.Interfaces;

public interface IPhotoRepository : IGenericRepository<Photo>
{
    Task<IEnumerable<Photo>> GetByProjectIdAsync(int projectId);
    Task<IEnumerable<Photo>> GetHomepageSliderPhotosAsync();
    Task<IEnumerable<Photo>> GetByStatusAsync(ContentStatus status);
    Task<IEnumerable<Photo>> GetOrphanPhotosAsync(); // Photos without project
    Task DeletePhotoFileAsync(string filePath);
    Task<bool> IsUsedInSliderAsync(int photoId);
}