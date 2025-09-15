namespace Mimarlik.Application.DTOs;

public class TranslationDto
{
    public int Id { get; set; }
    public string EntityName { get; set; } = string.Empty;
    public int EntityId { get; set; }
    public string FieldName { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public int LanguageId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public LanguageDto? Language { get; set; }
}

public class CreateTranslationDto
{
    public string EntityName { get; set; } = string.Empty;
    public int EntityId { get; set; }
    public string FieldName { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public int LanguageId { get; set; }
}

public class UpdateTranslationDto
{
    public string Value { get; set; } = string.Empty;
}

public class TranslationRequestDto
{
    public Dictionary<string, string> Translations { get; set; } = new();
}