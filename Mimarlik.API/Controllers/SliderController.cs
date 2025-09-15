using Microsoft.AspNetCore.Mvc;
using Mimarlik.Application.DTOs;
using Mimarlik.Application.Interfaces;

namespace Mimarlik.API.Controllers;

[Route("api/[controller]")]
public class SliderController : BaseApiController
{
    private readonly IPhotoService _photoService;

    public SliderController(IPhotoService photoService)
    {
        _photoService = photoService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<SliderPhotoDto>>> GetSliderPhotos()
    {
        var photos = await _photoService.GetHomepageSliderPhotosAsync();
        return Ok(photos);
    }

    [HttpPost("add-photo/{photoId}")]
    public async Task<ActionResult> AddPhotoToSlider(int photoId, [FromBody] AddToSliderRequest request)
    {
        try
        {
            var result = await _photoService.AddToSliderAsync(photoId, request.SliderText ?? string.Empty);
            if (result)
            {
                return Ok(new { message = "Photo added to slider successfully" });
            }
            return NotFound(new { message = "Photo not found" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("remove-photo/{photoId}")]
    public async Task<ActionResult> RemovePhotoFromSlider(int photoId)
    {
        try
        {
            var result = await _photoService.RemoveFromSliderAsync(photoId);
            if (result)
            {
                return Ok(new { message = "Photo removed from slider successfully" });
            }
            return NotFound(new { message = "Photo not found" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("photo/{photoId}/text")]
    public async Task<ActionResult> UpdateSliderText(int photoId, [FromBody] UpdateSliderTextRequest request)
    {
        try
        {
            // First get the photo to update
            var photo = await _photoService.GetByIdAsync(photoId);
            if (photo == null)
            {
                return NotFound(new { message = "Photo not found" });
            }

            if (!photo.IsHomepageSlider)
            {
                return BadRequest(new { message = "Photo is not in the slider" });
            }

            var updateDto = new UpdatePhotoDto
            {
                AltText = photo.AltText,
                Caption = photo.Caption,
                Description = photo.Description,
                ProjectId = photo.ProjectId,
                Status = photo.Status,
                SortOrder = photo.SortOrder,
                IsHomepageSlider = photo.IsHomepageSlider,
                SliderText = request.SliderText ?? string.Empty
            };

            await _photoService.UpdateAsync(photoId, updateDto);
            return Ok(new { message = "Slider text updated successfully" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}

public class AddToSliderRequest
{
    public string? SliderText { get; set; }
}

public class UpdateSliderTextRequest
{
    public string? SliderText { get; set; }
}