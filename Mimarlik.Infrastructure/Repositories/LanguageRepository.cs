using Microsoft.EntityFrameworkCore;
using Mimarlik.Core.Entities;
using Mimarlik.Core.Enums;
using Mimarlik.Core.Interfaces;
using Mimarlik.Infrastructure.Data;

namespace Mimarlik.Infrastructure.Repositories;

public class LanguageRepository : GenericRepository<Language>, ILanguageRepository
{
    public LanguageRepository(MimarlikDbContext context) : base(context)
    {
    }

    public async Task<Language?> GetDefaultLanguageAsync()
    {
        return await _dbSet
            .FirstOrDefaultAsync(l => l.IsDefault && l.Status == ContentStatus.Published);
    }

    public async Task<Language?> GetByCodeAsync(string code)
    {
        return await _dbSet
            .FirstOrDefaultAsync(l => l.Code == code);
    }

    public async Task<IEnumerable<Language>> GetActiveLanguagesAsync()
    {
        return await _dbSet
            .Where(l => l.Status == ContentStatus.Published)
            .OrderBy(l => l.SortOrder)
            .ThenBy(l => l.Name)
            .ToListAsync();
    }

    public async Task<bool> CodeExistsAsync(string code, int? excludeId = null)
    {
        var query = _dbSet.Where(l => l.Code == code);
        
        if (excludeId.HasValue)
            query = query.Where(l => l.Id != excludeId.Value);
            
        return await query.AnyAsync();
    }

    public async Task SetDefaultLanguageAsync(int languageId)
    {
        // First, set all languages to non-default
        var allLanguages = await _dbSet.ToListAsync();
        foreach (var language in allLanguages)
        {
            language.IsDefault = false;
        }

        // Then set the specified language as default
        var targetLanguage = await _dbSet.FindAsync(languageId);
        if (targetLanguage != null)
        {
            targetLanguage.IsDefault = true;
        }
    }
}