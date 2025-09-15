using Mimarlik.Application.DTOs;

namespace Mimarlik.Application.Interfaces;

public interface IPhotoService
{
    Task<IEnumerable<PhotoDto>> GetAllAsync();
    Task<IEnumerable<PhotoDto>> GetByProjectIdAsync(int projectId);
    Task<IEnumerable<SliderPhotoDto>> GetHomepageSliderPhotosAsync();
    Task<PhotoDto?> GetByIdAsync(int id);
    Task<PhotoDto> UploadAsync(Stream fileStream, string fileName, CreatePhotoDto createPhotoDto);
    Task<PhotoDto> UpdateAsync(int id, UpdatePhotoDto updatePhotoDto);
    Task<bool> DeleteAsync(int id);
    Task<bool> AddToSliderAsync(int photoId, string sliderText = "");
    Task<bool> RemoveFromSliderAsync(int photoId);
}