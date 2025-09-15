using Mimarlik.Core.Enums;

namespace Mimarlik.Application.DTOs;

public class ProjectDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public DateTime? CompletionDate { get; set; }
    public string Client { get; set; } = string.Empty;
    public decimal? Area { get; set; }
    public string AreaUnit { get; set; } = "m²";
    public int? CategoryId { get; set; }
    public ContentStatus Status { get; set; }
    public int SortOrder { get; set; }
    public bool IsFeatured { get; set; }
    public string MetaTitle { get; set; } = string.Empty;
    public string MetaDescription { get; set; } = string.Empty;
    public string MetaKeywords { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public CategoryDto? Category { get; set; }
    public List<PhotoDto> Photos { get; set; } = new();
    public List<ContentBlockDto> ContentBlocks { get; set; } = new();
}

public class CreateProjectDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public DateTime? CompletionDate { get; set; }
    public string Client { get; set; } = string.Empty;
    public decimal? Area { get; set; }
    public string AreaUnit { get; set; } = "m²";
    public int? CategoryId { get; set; }
    public ContentStatus Status { get; set; } = ContentStatus.Published;
    public int SortOrder { get; set; } = 0;
    public bool IsFeatured { get; set; } = false;
    public string MetaTitle { get; set; } = string.Empty;
    public string MetaDescription { get; set; } = string.Empty;
    public string MetaKeywords { get; set; } = string.Empty;
    public List<CreateContentBlockDto> ContentBlocks { get; set; } = new();
}

public class UpdateProjectDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public DateTime? CompletionDate { get; set; }
    public string Client { get; set; } = string.Empty;
    public decimal? Area { get; set; }
    public string AreaUnit { get; set; } = "m²";
    public int? CategoryId { get; set; }
    public ContentStatus Status { get; set; }
    public int SortOrder { get; set; }
    public bool IsFeatured { get; set; }
    public string MetaTitle { get; set; } = string.Empty;
    public string MetaDescription { get; set; } = string.Empty;
    public string MetaKeywords { get; set; } = string.Empty;
    public List<CreateContentBlockDto> ContentBlocks { get; set; } = new();
}