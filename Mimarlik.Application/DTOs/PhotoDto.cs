using Mimarlik.Core.Enums;

namespace Mimarlik.Application.DTOs;

public class PhotoDto
{
    public int Id { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string OriginalFileName { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public string FileSize { get; set; } = string.Empty;
    public string MimeType { get; set; } = string.Empty;
    public int Width { get; set; }
    public int Height { get; set; }
    public string AltText { get; set; } = string.Empty;
    public string Caption { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int? ProjectId { get; set; }
    public ContentStatus Status { get; set; }
    public int SortOrder { get; set; }
    public bool IsHomepageSlider { get; set; }
    public string SliderText { get; set; } = string.Empty;
    public bool HasWatermark { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string Url { get; set; } = string.Empty; // Full URL for access

    public ProjectDto? Project { get; set; }
}

public class CreatePhotoDto
{
    public string AltText { get; set; } = string.Empty;
    public string Caption { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int? ProjectId { get; set; }
    public ContentStatus Status { get; set; } = ContentStatus.Published;
    public int SortOrder { get; set; } = 0;
    public bool IsHomepageSlider { get; set; } = false;
    public string SliderText { get; set; } = string.Empty;
    public bool AddWatermark { get; set; } = true;
}

public class UpdatePhotoDto
{
    public string AltText { get; set; } = string.Empty;
    public string Caption { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int? ProjectId { get; set; }
    public ContentStatus Status { get; set; }
    public int SortOrder { get; set; }
    public bool IsHomepageSlider { get; set; }
    public string SliderText { get; set; } = string.Empty;
}

public class SliderPhotoDto
{
    public int Id { get; set; }
    public string FilePath { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string AltText { get; set; } = string.Empty;
    public string SliderText { get; set; } = string.Empty;
    public int? ProjectId { get; set; }
    public string? ProjectTitle { get; set; }
    public string? ProjectSlug { get; set; }
    public int SortOrder { get; set; }
}