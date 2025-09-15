using Mimarlik.Core.Entities;
using Mimarlik.Core.Enums;

namespace Mimarlik.Core.Interfaces;

public interface ILanguageRepository : IGenericRepository<Language>
{
    Task<Language?> GetDefaultLanguageAsync();
    Task<Language?> GetByCodeAsync(string code);
    Task<IEnumerable<Language>> GetActiveLanguagesAsync();
    Task<bool> CodeExistsAsync(string code, int? excludeId = null);
    Task SetDefaultLanguageAsync(int languageId);
}