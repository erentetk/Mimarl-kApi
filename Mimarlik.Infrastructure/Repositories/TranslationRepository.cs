using Microsoft.EntityFrameworkCore;
using Mimarlik.Core.Entities;
using Mimarlik.Core.Interfaces;
using Mimarlik.Infrastructure.Data;

namespace Mimarlik.Infrastructure.Repositories;

public class TranslationRepository : GenericRepository<Translation>, ITranslationRepository
{
    public TranslationRepository(MimarlikDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Translation>> GetByEntityAsync(string entityName, int entityId)
    {
        return await _dbSet
            .Where(t => t.EntityName == entityName && t.EntityId == entityId)
            .Include(t => t.Language)
            .ToListAsync();
    }

    public async Task<Translation?> GetByEntityFieldAsync(string entityName, int entityId, string fieldName, int languageId)
    {
        return await _dbSet
            .FirstOrDefaultAsync(t => 
                t.EntityName == entityName && 
                t.EntityId == entityId && 
                t.FieldName == fieldName && 
                t.LanguageId == languageId);
    }

    public async Task<IEnumerable<Translation>> GetByLanguageAsync(int languageId)
    {
        return await _dbSet
            .Where(t => t.LanguageId == languageId)
            .ToListAsync();
    }

    public async Task DeleteByEntityAsync(string entityName, int entityId)
    {
        var translations = await _dbSet
            .Where(t => t.EntityName == entityName && t.EntityId == entityId)
            .ToListAsync();
            
        _dbSet.RemoveRange(translations);
    }

    public async Task UpsertTranslationAsync(string entityName, int entityId, string fieldName, int languageId, string value)
    {
        var existingTranslation = await GetByEntityFieldAsync(entityName, entityId, fieldName, languageId);
        
        if (existingTranslation != null)
        {
            existingTranslation.Value = value;
            existingTranslation.UpdatedAt = DateTime.UtcNow;
        }
        else
        {
            var newTranslation = new Translation
            {
                EntityName = entityName,
                EntityId = entityId,
                FieldName = fieldName,
                LanguageId = languageId,
                Value = value
            };
            
            await _dbSet.AddAsync(newTranslation);
        }
    }
}