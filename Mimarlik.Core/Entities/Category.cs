using Mimarlik.Core.Enums;

namespace Mimarlik.Core.Entities;

public class Category : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public int? ParentId { get; set; }
    public ContentStatus Status { get; set; } = ContentStatus.Published;
    public int SortOrder { get; set; } = 0;

    // Navigation properties
    public Category? Parent { get; set; }
    public ICollection<Category> Children { get; set; } = new List<Category>();
    public ICollection<Project> Projects { get; set; } = new List<Project>();
}