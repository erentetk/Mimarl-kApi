using Mimarlik.Core.Entities;

namespace Mimarlik.Core.Interfaces;

public interface ITranslationRepository : IGenericRepository<Translation>
{
    Task<IEnumerable<Translation>> GetByEntityAsync(string entityName, int entityId);
    Task<Translation?> GetByEntityFieldAsync(string entityName, int entityId, string fieldName, int languageId);
    Task<IEnumerable<Translation>> GetByLanguageAsync(int languageId);
    Task DeleteByEntityAsync(string entityName, int entityId);
    Task UpsertTranslationAsync(string entityName, int entityId, string fieldName, int languageId, string value);
}