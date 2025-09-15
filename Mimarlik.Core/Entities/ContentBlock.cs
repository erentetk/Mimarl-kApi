using Mimarlik.Core.Enums;

namespace Mimarlik.Core.Entities;

public class ContentBlock : BaseEntity
{
    public int ProjectId { get; set; }
    public ContentBlockType Type { get; set; }
    public string Content { get; set; } = string.Empty;
    public string Properties { get; set; } = string.Empty; // JSON for additional properties
    public int SortOrder { get; set; } = 0;
    public ContentStatus Status { get; set; } = ContentStatus.Published;

    // Navigation properties
    public Project Project { get; set; } = null!;
}