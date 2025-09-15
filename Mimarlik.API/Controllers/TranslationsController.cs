using Microsoft.AspNetCore.Mvc;
using Mimarlik.Application.DTOs;
using Mimarlik.Application.Interfaces;

namespace Mimarlik.API.Controllers;

[Route("api/[controller]")]
public class TranslationsController : BaseApiController
{
    private readonly ITranslationService _translationService;

    public TranslationsController(ITranslationService translationService)
    {
        _translationService = translationService;
    }

    [HttpGet("entity/{entityName}/{entityId}")]
    public async Task<ActionResult<IEnumerable<TranslationDto>>> GetByEntity(string entityName, int entityId)
    {
        var translations = await _translationService.GetByEntityAsync(entityName, entityId);
        return Ok(translations);
    }

    [HttpGet("entity/{entityName}/{entityId}/field/{fieldName}/language/{languageId}")]
    public async Task<ActionResult<TranslationDto>> GetByEntityField(string entityName, int entityId, string fieldName, int languageId)
    {
        var translation = await _translationService.GetByEntityFieldAsync(entityName, entityId, fieldName, languageId);
        return HandleResult(translation);
    }

    [HttpGet("language/{languageId}")]
    public async Task<ActionResult<IEnumerable<TranslationDto>>> GetByLanguage(int languageId)
    {
        var translations = await _translationService.GetByLanguageAsync(languageId);
        return Ok(translations);
    }

    [HttpPost]
    public async Task<ActionResult<TranslationDto>> Create([FromBody] CreateTranslationDto createTranslationDto)
    {
        try
        {
            var translation = await _translationService.CreateAsync(createTranslationDto);
            return CreatedAtAction(nameof(GetByEntityField), 
                new { 
                    entityName = translation.EntityName,
                    entityId = translation.EntityId,
                    fieldName = translation.FieldName,
                    languageId = translation.LanguageId
                }, translation);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<TranslationDto>> Update(int id, [FromBody] UpdateTranslationDto updateTranslationDto)
    {
        try
        {
            var translation = await _translationService.UpdateAsync(id, updateTranslationDto);
            return Ok(translation);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("upsert")]
    public async Task<ActionResult> UpsertTranslation([FromBody] UpsertTranslationRequest request)
    {
        try
        {
            await _translationService.UpsertTranslationAsync(
                request.EntityName, 
                request.EntityId, 
                request.FieldName, 
                request.LanguageId, 
                request.Value);
            return Ok(new { message = "Translation saved successfully" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("bulk-upsert")]
    public async Task<ActionResult> BulkUpsertTranslations([FromBody] BulkUpsertTranslationRequest request)
    {
        try
        {
            foreach (var translation in request.Translations)
            {
                await _translationService.UpsertTranslationAsync(
                    request.EntityName,
                    request.EntityId,
                    translation.Key,
                    request.LanguageId,
                    translation.Value);
            }
            return Ok(new { message = "Translations saved successfully" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        try
        {
            var result = await _translationService.DeleteAsync(id);
            return HandleBoolResult(result, "Translation deleted successfully");
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("entity/{entityName}/{entityId}")]
    public async Task<ActionResult> DeleteByEntity(string entityName, int entityId)
    {
        try
        {
            await _translationService.DeleteByEntityAsync(entityName, entityId);
            return Ok(new { message = "Entity translations deleted successfully" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}

public class UpsertTranslationRequest
{
    public string EntityName { get; set; } = string.Empty;
    public int EntityId { get; set; }
    public string FieldName { get; set; } = string.Empty;
    public int LanguageId { get; set; }
    public string Value { get; set; } = string.Empty;
}

public class BulkUpsertTranslationRequest
{
    public string EntityName { get; set; } = string.Empty;
    public int EntityId { get; set; }
    public int LanguageId { get; set; }
    public Dictionary<string, string> Translations { get; set; } = new();
}