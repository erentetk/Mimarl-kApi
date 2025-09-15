using AutoMapper;
using Mimarlik.Application.DTOs;
using Mimarlik.Application.Interfaces;
using Mimarlik.Core.Entities;
using Mimarlik.Core.Interfaces;

namespace Mimarlik.Application.Services;

public class TranslationService : ITranslationService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public TranslationService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<TranslationDto>> GetByEntityAsync(string entityName, int entityId)
    {
        var translations = await _unitOfWork.Translations.GetByEntityAsync(entityName, entityId);
        return _mapper.Map<IEnumerable<TranslationDto>>(translations);
    }

    public async Task<TranslationDto?> GetByEntityFieldAsync(string entityName, int entityId, string fieldName, int languageId)
    {
        var translation = await _unitOfWork.Translations.GetByEntityFieldAsync(entityName, entityId, fieldName, languageId);
        return translation != null ? _mapper.Map<TranslationDto>(translation) : null;
    }

    public async Task<IEnumerable<TranslationDto>> GetByLanguageAsync(int languageId)
    {
        var translations = await _unitOfWork.Translations.GetByLanguageAsync(languageId);
        return _mapper.Map<IEnumerable<TranslationDto>>(translations);
    }

    public async Task<TranslationDto> CreateAsync(CreateTranslationDto createTranslationDto)
    {
        // Check if language exists
        var language = await _unitOfWork.Languages.GetByIdAsync(createTranslationDto.LanguageId);
        if (language == null)
        {
            throw new KeyNotFoundException($"Language with ID {createTranslationDto.LanguageId} not found.");
        }

        var translation = _mapper.Map<Translation>(createTranslationDto);
        translation = await _unitOfWork.Translations.AddAsync(translation);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<TranslationDto>(translation);
    }

    public async Task<TranslationDto> UpdateAsync(int id, UpdateTranslationDto updateTranslationDto)
    {
        var existingTranslation = await _unitOfWork.Translations.GetByIdAsync(id);
        if (existingTranslation == null)
        {
            throw new KeyNotFoundException($"Translation with ID {id} not found.");
        }

        _mapper.Map(updateTranslationDto, existingTranslation);
        await _unitOfWork.Translations.UpdateAsync(existingTranslation);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<TranslationDto>(existingTranslation);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var translation = await _unitOfWork.Translations.GetByIdAsync(id);
        if (translation == null)
        {
            return false;
        }

        await _unitOfWork.Translations.DeleteAsync(translation);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task DeleteByEntityAsync(string entityName, int entityId)
    {
        await _unitOfWork.Translations.DeleteByEntityAsync(entityName, entityId);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task UpsertTranslationAsync(string entityName, int entityId, string fieldName, int languageId, string value)
    {
        await _unitOfWork.Translations.UpsertTranslationAsync(entityName, entityId, fieldName, languageId, value);
        await _unitOfWork.SaveChangesAsync();
    }
}