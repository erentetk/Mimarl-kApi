using Microsoft.AspNetCore.Mvc;
using Mimarlik.Application.DTOs;
using Mimarlik.Application.Interfaces;

namespace Mimarlik.API.Controllers;

[Route("api/[controller]")]
public class LanguagesController : BaseApiController
{
    private readonly ILanguageService _languageService;

    public LanguagesController(ILanguageService languageService)
    {
        _languageService = languageService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<LanguageDto>>> GetAll()
    {
        var languages = await _languageService.GetAllAsync();
        return Ok(languages);
    }

    [HttpGet("active")]
    public async Task<ActionResult<IEnumerable<LanguageDto>>> GetActive()
    {
        var languages = await _languageService.GetActiveAsync();
        return Ok(languages);
    }

    [HttpGet("default")]
    public async Task<ActionResult<LanguageDto>> GetDefault()
    {
        var language = await _languageService.GetDefaultAsync();
        return HandleResult(language);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<LanguageDto>> GetById(int id)
    {
        var language = await _languageService.GetByIdAsync(id);
        return HandleResult(language);
    }

    [HttpGet("code/{code}")]
    public async Task<ActionResult<LanguageDto>> GetByCode(string code)
    {
        var language = await _languageService.GetByCodeAsync(code);
        return HandleResult(language);
    }

    [HttpPost]
    public async Task<ActionResult<LanguageDto>> Create([FromBody] CreateLanguageDto createLanguageDto)
    {
        try
        {
            var language = await _languageService.CreateAsync(createLanguageDto);
            return CreatedAtAction(nameof(GetById), new { id = language.Id }, language);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<LanguageDto>> Update(int id, [FromBody] UpdateLanguageDto updateLanguageDto)
    {
        try
        {
            var language = await _languageService.UpdateAsync(id, updateLanguageDto);
            return Ok(language);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
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
            var result = await _languageService.DeleteAsync(id);
            return HandleBoolResult(result, "Language deleted successfully");
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("{id}/set-default")]
    public async Task<ActionResult> SetDefault(int id)
    {
        try
        {
            var result = await _languageService.SetDefaultAsync(id);
            return HandleBoolResult(result, "Default language set successfully");
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}