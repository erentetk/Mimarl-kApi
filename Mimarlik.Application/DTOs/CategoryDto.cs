using Mimarlik.Core.Enums;

namespace Mimarlik.Application.DTOs;

public class CategoryDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public int? ParentId { get; set; }
    public ContentStatus Status { get; set; }
    public int SortOrder { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public CategoryDto? Parent { get; set; }
    public List<CategoryDto> Children { get; set; } = new();
    public List<ProjectDto> Projects { get; set; } = new();
}

public class CreateCategoryDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public int? ParentId { get; set; }
    public ContentStatus Status { get; set; } = ContentStatus.Published;
    public int SortOrder { get; set; } = 0;
}

public class UpdateCategoryDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public int? ParentId { get; set; }
    public ContentStatus Status { get; set; }
    public int SortOrder { get; set; }
}