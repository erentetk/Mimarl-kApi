    using Microsoft.AspNetCore.Mvc;
using Mimarlik.Application.DTOs;
using Mimarlik.Application.Interfaces;

namespace Mimarlik.API.Controllers;

[Route("api/[controller]")]
public class PhotosController : BaseApiController
{
    private readonly IPhotoService _photoService;
    private readonly ILogger<PhotosController> _logger;

    public PhotosController(IPhotoService photoService, ILogger<PhotosController> logger)
    {
        _photoService = photoService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PhotoDto>>> GetAll()
    {
        var photos = await _photoService.GetAllAsync();
        return Ok(photos);
    }

    [HttpGet("by-project/{projectId}")]
    public async Task<ActionResult<IEnumerable<PhotoDto>>> GetByProjectId(int projectId)
    {
        var photos = await _photoService.GetByProjectIdAsync(projectId);
        return Ok(photos);
    }

    [HttpGet("slider")]
    public async Task<ActionResult<IEnumerable<SliderPhotoDto>>> GetHomepageSlider()
    {
        var photos = await _photoService.GetHomepageSliderPhotosAsync();
        return Ok(photos);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<PhotoDto>> GetById(int id)
    {
        var photo = await _photoService.GetByIdAsync(id);
        return HandleResult(photo);
    }

    [HttpPost]
    public async Task<ActionResult<PhotoDto>> Upload([FromForm] UploadPhotoRequest request)
    {
        try
        {
            _logger.LogInformation("Photo upload started. File: {FileName}, Size: {FileSize}", 
                request.File?.FileName, request.File?.Length);
                
            if (request.File == null || request.File.Length == 0)
            {
                _logger.LogWarning("No file uploaded");
                return BadRequest(new { message = "Dosya yüklenmedi. Lütfen bir dosya seçin." });
            }

            // Validate file type more strictly
            var allowedTypes = new[] { "image/jpeg", "image/jpg", "image/png", "image/gif", "image/bmp", "image/webp" };
            var fileExtension = Path.GetExtension(request.File.FileName).ToLowerInvariant();
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp" };
            
            if (!allowedTypes.Contains(request.File.ContentType.ToLowerInvariant()) || 
                !allowedExtensions.Contains(fileExtension))
            {
                _logger.LogWarning("Invalid file type: {ContentType}, Extension: {Extension}", 
                    request.File.ContentType, fileExtension);
                return BadRequest(new { message = "Geçersiz dosya türü. Sadece resim dosyaları (.jpg, .jpeg, .png, .gif, .bmp, .webp) yüklenebilir." });
            }

            // Validate file size (10MB max)
            if (request.File.Length > 10 * 1024 * 1024)
            {
                _logger.LogWarning("File too large: {FileSize} bytes, FileName: {FileName}", 
                    request.File.Length, request.File.FileName);
                return BadRequest(new { message = "Dosya çok büyük. Maksimum 10MB izin verilmektedir." });
            }

            using var stream = request.File.OpenReadStream();
            var createPhotoDto = new CreatePhotoDto
            {
                AltText = request.AltText ?? string.Empty,
                Caption = request.Caption ?? string.Empty,
                Description = request.Description ?? string.Empty,
                ProjectId = request.ProjectId,
                Status = request.Status,
                SortOrder = request.SortOrder,
                IsHomepageSlider = request.IsHomepageSlider,
                SliderText = request.SliderText ?? string.Empty,
                AddWatermark = request.AddWatermark
            };

            _logger.LogInformation("Calling photo service to upload file");
            var photo = await _photoService.UploadAsync(stream, request.File.FileName, createPhotoDto);
            
            _logger.LogInformation("Photo uploaded successfully. ID: {PhotoId}, FilePath: {FilePath}", 
                photo.Id, photo.FilePath);
                
            return CreatedAtAction(nameof(GetById), new { id = photo.Id }, photo);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Fotoğraf yükleme hatası. FileName: {FileName}, Message: {Message}", 
                request.File?.FileName, ex.Message);
            return BadRequest(new { 
                message = $"Fotoğraf yüklenirken hata oluştu: {ex.Message}",
                fileName = request.File?.FileName 
            });
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<PhotoDto>> Update(int id, [FromBody] UpdatePhotoDto updatePhotoDto)
    {
        try
        {
            var photo = await _photoService.UpdateAsync(id, updatePhotoDto);
            return Ok(photo);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        try
        {
            var result = await _photoService.DeleteAsync(id);
            return HandleBoolResult(result, "Photo deleted successfully");
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("{id}/add-to-slider")]
    public async Task<ActionResult> AddToSlider(int id, [FromBody] SliderRequest request)
    {
        try
        {
            var result = await _photoService.AddToSliderAsync(id, request.SliderText ?? string.Empty);
            return HandleBoolResult(result, "Photo added to slider successfully");
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("{id}/remove-from-slider")]
    public async Task<ActionResult> RemoveFromSlider(int id)
    {
        try
        {
            var result = await _photoService.RemoveFromSliderAsync(id);
            return HandleBoolResult(result, "Photo removed from slider successfully");
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}

public class UploadPhotoRequest
{
    public IFormFile? File { get; set; }
    public string? AltText { get; set; }
    public string? Caption { get; set; }
    public string? Description { get; set; }
    public int? ProjectId { get; set; }
    public Core.Enums.ContentStatus Status { get; set; } = Core.Enums.ContentStatus.Published;
    public int SortOrder { get; set; } = 0;
    public bool IsHomepageSlider { get; set; } = false;
    public string? SliderText { get; set; }
    public bool AddWatermark { get; set; } = true;
}

public class SliderRequest
{
    public string? SliderText { get; set; }
}