using Mimarlik.Application.DTOs;

namespace Mimarlik.Application.Interfaces;

public interface ITranslationService
{
    Task<IEnumerable<TranslationDto>> GetByEntityAsync(string entityName, int entityId);
    Task<TranslationDto?> GetByEntityFieldAsync(string entityName, int entityId, string fieldName, int languageId);
    Task<IEnumerable<TranslationDto>> GetByLanguageAsync(int languageId);
    Task<TranslationDto> CreateAsync(CreateTranslationDto createTranslationDto);
    Task<TranslationDto> UpdateAsync(int id, UpdateTranslationDto updateTranslationDto);
    Task<bool> DeleteAsync(int id);
    Task DeleteByEntityAsync(string entityName, int entityId);
    Task UpsertTranslationAsync(string entityName, int entityId, string fieldName, int languageId, string value);
}