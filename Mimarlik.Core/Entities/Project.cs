using Mimarlik.Core.Enums;

namespace Mimarlik.Core.Entities;

public class Project : BaseEntity
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

    // SEO fields
    public string MetaTitle { get; set; } = string.Empty;
    public string MetaDescription { get; set; } = string.Empty;
    public string MetaKeywords { get; set; } = string.Empty;

    // Navigation properties
    public Category? Category { get; set; }
    public ICollection<Photo> Photos { get; set; } = new List<Photo>();
    public ICollection<ContentBlock> ContentBlocks { get; set; } = new List<ContentBlock>();
}