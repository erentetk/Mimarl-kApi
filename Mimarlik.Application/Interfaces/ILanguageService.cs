using Mimarlik.Application.DTOs;

namespace Mimarlik.Application.Interfaces;

public interface ILanguageService
{
    Task<IEnumerable<LanguageDto>> GetAllAsync();
    Task<IEnumerable<LanguageDto>> GetActiveAsync();
    Task<LanguageDto?> GetByIdAsync(int id);
    Task<LanguageDto?> GetByCodeAsync(string code);
    Task<LanguageDto?> GetDefaultAsync();
    Task<LanguageDto> CreateAsync(CreateLanguageDto createLanguageDto);
    Task<LanguageDto> UpdateAsync(int id, UpdateLanguageDto updateLanguageDto);
    Task<bool> DeleteAsync(int id);
    Task<bool> SetDefaultAsync(int id);
}