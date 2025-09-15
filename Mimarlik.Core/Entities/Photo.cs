using Mimarlik.Core.Enums;

namespace Mimarlik.Core.Entities;

public class Photo : BaseEntity
{
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
    public int? ProjectId { get; set; } // Nullable for homepage slider photos
    public ContentStatus Status { get; set; } = ContentStatus.Published;
    public int SortOrder { get; set; } = 0;
    public bool IsHomepageSlider { get; set; } = false;
    public string SliderText { get; set; } = string.Empty; // Text overlay for slider
    public bool HasWatermark { get; set; } = false;

    // Navigation properties
    public Project? Project { get; set; }
}