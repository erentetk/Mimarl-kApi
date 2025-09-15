using AutoMapper;
using Mimarlik.Application.DTOs;
using Mimarlik.Application.Interfaces;
using Mimarlik.Core.Entities;
using Mimarlik.Core.Interfaces;

namespace Mimarlik.Application.Services;

public class LanguageService : ILanguageService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public LanguageService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<LanguageDto>> GetAllAsync()
    {
        var languages = await _unitOfWork.Languages.GetAllAsync();
        return _mapper.Map<IEnumerable<LanguageDto>>(languages);
    }

    public async Task<IEnumerable<LanguageDto>> GetActiveAsync()
    {
        var languages = await _unitOfWork.Languages.GetActiveLanguagesAsync();
        return _mapper.Map<IEnumerable<LanguageDto>>(languages);
    }

    public async Task<LanguageDto?> GetByIdAsync(int id)
    {
        var language = await _unitOfWork.Languages.GetByIdAsync(id);
        return language != null ? _mapper.Map<LanguageDto>(language) : null;
    }

    public async Task<LanguageDto?> GetByCodeAsync(string code)
    {
        var language = await _unitOfWork.Languages.GetByCodeAsync(code);
        return language != null ? _mapper.Map<LanguageDto>(language) : null;
    }

    public async Task<LanguageDto?> GetDefaultAsync()
    {
        var language = await _unitOfWork.Languages.GetDefaultLanguageAsync();
        return language != null ? _mapper.Map<LanguageDto>(language) : null;
    }

    public async Task<LanguageDto> CreateAsync(CreateLanguageDto createLanguageDto)
    {
        // Check if code already exists
        var codeExists = await _unitOfWork.Languages.CodeExistsAsync(createLanguageDto.Code);
        if (codeExists)
        {
            throw new InvalidOperationException($"Language with code '{createLanguageDto.Code}' already exists.");
        }

        var language = _mapper.Map<Language>(createLanguageDto);

        // If this is the first language or marked as default, set it as default
        var existingLanguages = await _unitOfWork.Languages.GetAllAsync();
        if (!existingLanguages.Any() || createLanguageDto.IsDefault)
        {
            await _unitOfWork.Languages.SetDefaultLanguageAsync(0); // Reset all to non-default first
            language.IsDefault = true;
        }

        language = await _unitOfWork.Languages.AddAsync(language);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<LanguageDto>(language);
    }

    public async Task<LanguageDto> UpdateAsync(int id, UpdateLanguageDto updateLanguageDto)
    {
        var existingLanguage = await _unitOfWork.Languages.GetByIdAsync(id);
        if (existingLanguage == null)
        {
            throw new KeyNotFoundException($"Language with ID {id} not found.");
        }

        // Check if code already exists (exclude current language)
        var codeExists = await _unitOfWork.Languages.CodeExistsAsync(updateLanguageDto.Code, id);
        if (codeExists)
        {
            throw new InvalidOperationException($"Language with code '{updateLanguageDto.Code}' already exists.");
        }

        _mapper.Map(updateLanguageDto, existingLanguage);

        // Handle default language setting
        if (updateLanguageDto.IsDefault)
        {
            await _unitOfWork.Languages.SetDefaultLanguageAsync(id);
        }

        await _unitOfWork.Languages.UpdateAsync(existingLanguage);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<LanguageDto>(existingLanguage);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var language = await _unitOfWork.Languages.GetByIdAsync(id);
        if (language == null)
        {
            return false;
        }

        // Check if this is the default language
        if (language.IsDefault)
        {
            var allLanguages = await _unitOfWork.Languages.GetAllAsync();
            if (allLanguages.Count() <= 1)
            {
                throw new InvalidOperationException("Cannot delete the last language.");
            }
            throw new InvalidOperationException("Cannot delete the default language. Please set another language as default first.");
        }

        await _unitOfWork.BeginTransactionAsync();
        try
        {
            // Delete all translations for this language
            var translations = await _unitOfWork.Translations.GetByLanguageAsync(id);
            await _unitOfWork.Translations.DeleteRangeAsync(translations);

            // Delete the language
            await _unitOfWork.Languages.DeleteAsync(language);
            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitTransactionAsync();

            return true;
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }

    public async Task<bool> SetDefaultAsync(int id)
    {
        var language = await _unitOfWork.Languages.GetByIdAsync(id);
        if (language == null)
        {
            return false;
        }

        await _unitOfWork.Languages.SetDefaultLanguageAsync(id);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }
}