using Mimarlik.Core.Enums;

namespace Mimarlik.Core.Entities;

public class Language : BaseEntity
{
    public string Code { get; set; } = string.Empty; // e.g., "tr", "en", "de"
    public string Name { get; set; } = string.Empty; // e.g., "Türkçe", "English", "Deutsch"
    public string NativeName { get; set; } = string.Empty; // e.g., "Türkçe", "English", "Deutsch"
    public bool IsDefault { get; set; } = false;
    public ContentStatus Status { get; set; } = ContentStatus.Published;
    public int SortOrder { get; set; } = 0;

    // Navigation properties
    public ICollection<Translation> Translations { get; set; } = new List<Translation>();
}