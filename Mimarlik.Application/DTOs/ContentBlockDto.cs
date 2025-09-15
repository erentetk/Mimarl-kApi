using Mimarlik.Core.Enums;

namespace Mimarlik.Application.DTOs;

public class ContentBlockDto
{
    public int Id { get; set; }
    public int ProjectId { get; set; }
    public ContentBlockType Type { get; set; }
    public string Content { get; set; } = string.Empty;
    public string Properties { get; set; } = string.Empty;
    public int SortOrder { get; set; }
    public ContentStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class CreateContentBlockDto
{
    public ContentBlockType Type { get; set; }
    public string Content { get; set; } = string.Empty;
    public string Properties { get; set; } = string.Empty;
    public int SortOrder { get; set; } = 0;
    public ContentStatus Status { get; set; } = ContentStatus.Published;
}

public class UpdateContentBlockDto
{
    public ContentBlockType Type { get; set; }
    public string Content { get; set; } = string.Empty;
    public string Properties { get; set; } = string.Empty;
    public int SortOrder { get; set; }
    public ContentStatus Status { get; set; }
}