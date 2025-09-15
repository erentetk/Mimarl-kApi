namespace Mimarlik.Core.Entities;

public class Translation : BaseEntity
{
    public string EntityName { get; set; } = string.Empty; // e.g., "Project", "Category", "Photo"
    public int EntityId { get; set; }
    public string FieldName { get; set; } = string.Empty; // e.g., "Title", "Description", "AltText"
    public string Value { get; set; } = string.Empty;
    public int LanguageId { get; set; }

    // Navigation properties
    public Language Language { get; set; } = null!;
}