using AutoMapper;
using Mimarlik.Application.DTOs;
using Mimarlik.Application.Interfaces;
using Mimarlik.Core.Entities;
using Mimarlik.Core.Interfaces;

namespace Mimarlik.Application.Services;

public class PhotoService : IPhotoService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IFileService _fileService;

    public PhotoService(IUnitOfWork unitOfWork, IMapper mapper, IFileService fileService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _fileService = fileService;
    }

    public async Task<IEnumerable<PhotoDto>> GetAllAsync()
    {
        var photos = await _unitOfWork.Photos.GetAllAsync();
        var photoDtos = _mapper.Map<IEnumerable<PhotoDto>>(photos);
        
        foreach (var photoDto in photoDtos)
        {
            photoDto.Url = _fileService.GetFileUrl(photoDto.FilePath);
        }
        
        return photoDtos;
    }

    public async Task<IEnumerable<PhotoDto>> GetByProjectIdAsync(int projectId)
    {
        var photos = await _unitOfWork.Photos.GetByProjectIdAsync(projectId);
        var photoDtos = _mapper.Map<IEnumerable<PhotoDto>>(photos);
        
        foreach (var photoDto in photoDtos)
        {
            photoDto.Url = _fileService.GetFileUrl(photoDto.FilePath);
        }
        
        return photoDtos;
    }

    public async Task<IEnumerable<SliderPhotoDto>> GetHomepageSliderPhotosAsync()
    {
        var photos = await _unitOfWork.Photos.GetHomepageSliderPhotosAsync();
        var sliderPhotos = _mapper.Map<IEnumerable<SliderPhotoDto>>(photos);
        
        foreach (var photoDto in sliderPhotos)
        {
            photoDto.Url = _fileService.GetFileUrl(photoDto.FilePath);
        }
        
        return sliderPhotos;
    }

    public async Task<PhotoDto?> GetByIdAsync(int id)
    {
        var photo = await _unitOfWork.Photos.GetByIdAsync(id);
        if (photo == null) return null;
        
        var photoDto = _mapper.Map<PhotoDto>(photo);
        photoDto.Url = _fileService.GetFileUrl(photoDto.FilePath);
        
        return photoDto;
    }

    public async Task<PhotoDto> UploadAsync(Stream fileStream, string fileName, CreatePhotoDto createPhotoDto)
    {
        string filePath = string.Empty;
        
        try
        {
            // Validate ProjectId if provided
            if (createPhotoDto.ProjectId.HasValue)
            {
                var projectExists = await _unitOfWork.Projects.GetByIdAsync(createPhotoDto.ProjectId.Value);
                if (projectExists == null)
                {
                    throw new ArgumentException($"Project with ID {createPhotoDto.ProjectId.Value} does not exist. Either leave ProjectId empty or provide a valid project ID.");
                }
            }

            // Get image dimensions
            var (width, height) = await _fileService.GetImageDimensionsAsync(fileStream);
            fileStream.Position = 0; // Reset stream position

            // Save file with or without watermark
            if (createPhotoDto.AddWatermark)
            {
                var watermarkText = "© Mimarlık Studio"; // Default watermark text
                filePath = await _fileService.SaveImageWithWatermarkAsync(fileStream, fileName, watermarkText, "photos");
            }
            else
            {
                filePath = await _fileService.SaveFileAsync(fileStream, fileName, "photos");
            }

            // Verify file was actually saved
            if (string.IsNullOrEmpty(filePath))
            {
                throw new InvalidOperationException("File was not saved successfully.");
            }

            // Create photo entity
            var photo = _mapper.Map<Photo>(createPhotoDto);
            photo.FileName = Path.GetFileName(filePath);
            photo.OriginalFileName = fileName;
            photo.FilePath = filePath;
            photo.Width = width;
            photo.Height = height;
            photo.HasWatermark = createPhotoDto.AddWatermark;
            photo.MimeType = GetMimeType(fileName);
            photo.FileSize = GetFileSizeString(fileStream.Length);

            // Begin transaction
            await _unitOfWork.BeginTransactionAsync();
            
            try
            {
                photo = await _unitOfWork.Photos.AddAsync(photo);
                var result = await _unitOfWork.SaveChangesAsync();
                
                // Verify that the save operation affected at least one row
                if (result == 0)
                {
                    throw new InvalidOperationException("Photo was not saved to database.");
                }
                
                await _unitOfWork.CommitTransactionAsync();

                var photoDto = _mapper.Map<PhotoDto>(photo);
                photoDto.Url = _fileService.GetFileUrl(photoDto.FilePath);

                return photoDto;
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                
                // If database save failed, delete the uploaded file
                if (!string.IsNullOrEmpty(filePath))
                {
                    try
                    {
                        await _fileService.DeleteFileAsync(filePath);
                    }
                    catch
                    {
                        // Log this error but don't throw - the main error is more important
                    }
                }
                
                throw;
            }
        }
        catch (Exception ex)
        {
            // If file save failed but we have a path, try to clean up
            if (!string.IsNullOrEmpty(filePath))
            {
                try
                {
                    await _fileService.DeleteFileAsync(filePath);
                }
                catch
                {
                    // Ignore cleanup errors
                }
            }
            
            throw new InvalidOperationException($"Failed to upload photo: {ex.Message}", ex);
        }
    }

    public async Task<PhotoDto> UpdateAsync(int id, UpdatePhotoDto updatePhotoDto)
    {
        var existingPhoto = await _unitOfWork.Photos.GetByIdAsync(id);
        if (existingPhoto == null)
        {
            throw new KeyNotFoundException($"Photo with ID {id} not found.");
        }

        // If photo status is being changed to Hidden or Draft, remove from slider
        if (updatePhotoDto.Status != Core.Enums.ContentStatus.Published && existingPhoto.IsHomepageSlider)
        {
            updatePhotoDto.IsHomepageSlider = false;
            updatePhotoDto.SliderText = string.Empty;
        }

        _mapper.Map(updatePhotoDto, existingPhoto);
        await _unitOfWork.Photos.UpdateAsync(existingPhoto);
        await _unitOfWork.SaveChangesAsync();

        var photoDto = _mapper.Map<PhotoDto>(existingPhoto);
        photoDto.Url = _fileService.GetFileUrl(photoDto.FilePath);

        return photoDto;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var photo = await _unitOfWork.Photos.GetByIdAsync(id);
        if (photo == null)
        {
            return false;
        }

        await _unitOfWork.BeginTransactionAsync();
        try
        {
            // Delete the physical file
            await _fileService.DeleteFileAsync(photo.FilePath);

            // Delete translations
            await _unitOfWork.Translations.DeleteByEntityAsync("Photo", id);

            // Delete the photo record
            await _unitOfWork.Photos.DeleteAsync(photo);
            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitTransactionAsync();

            return true;
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }

    public async Task<bool> AddToSliderAsync(int photoId, string sliderText = "")
    {
        var photo = await _unitOfWork.Photos.GetByIdAsync(photoId);
        if (photo == null)
        {
            return false;
        }

        photo.IsHomepageSlider = true;
        photo.SliderText = sliderText;

        await _unitOfWork.Photos.UpdateAsync(photo);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task<bool> RemoveFromSliderAsync(int photoId)
    {
        var photo = await _unitOfWork.Photos.GetByIdAsync(photoId);
        if (photo == null)
        {
            return false;
        }

        photo.IsHomepageSlider = false;
        photo.SliderText = string.Empty;

        await _unitOfWork.Photos.UpdateAsync(photo);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    private static string GetMimeType(string fileName)
    {
        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        return extension switch
        {
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".gif" => "image/gif",
            ".bmp" => "image/bmp",
            ".tiff" or ".tif" => "image/tiff",
            ".webp" => "image/webp",
            _ => "application/octet-stream"
        };
    }

    private static string GetFileSizeString(long bytes)
    {
        if (bytes < 1024)
            return $"{bytes} B";
        if (bytes < 1024 * 1024)
            return $"{bytes / 1024:F1} KB";
        if (bytes < 1024 * 1024 * 1024)
            return $"{bytes / (1024 * 1024):F1} MB";
        
        return $"{bytes / (1024 * 1024 * 1024):F1} GB";
    }
}