using Microsoft.AspNetCore.Mvc;
using Mimarlik.Application.DTOs;
using Mimarlik.Application.Interfaces;

namespace Mimarlik.API.Controllers;

[Route("api/[controller]")]
public class CategoriesController : BaseApiController
{
    private readonly ICategoryService _categoryService;

    public CategoriesController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CategoryDto>>> GetAll()
    {
        var categories = await _categoryService.GetAllAsync();
        return Ok(categories);
    }

    [HttpGet("with-children")]
    public async Task<ActionResult<IEnumerable<CategoryDto>>> GetAllWithChildren()
    {
        var categories = await _categoryService.GetAllWithChildrenAsync();
        return Ok(categories);
    }

    [HttpGet("by-parent/{parentId?}")]
    public async Task<ActionResult<IEnumerable<CategoryDto>>> GetByParentId(int? parentId)
    {
        var categories = await _categoryService.GetByParentIdAsync(parentId);
        return Ok(categories);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CategoryDto>> GetById(int id)
    {
        var category = await _categoryService.GetByIdAsync(id);
        return HandleResult(category);
    }

    [HttpGet("slug/{slug}")]
    public async Task<ActionResult<CategoryDto>> GetBySlug(string slug)
    {
        var category = await _categoryService.GetBySlugAsync(slug);
        return HandleResult(category);
    }

    [HttpPost]
    public async Task<ActionResult<CategoryDto>> Create([FromBody] CreateCategoryDto createCategoryDto)
    {
        try
        {
            var category = await _categoryService.CreateAsync(createCategoryDto);
            return CreatedAtAction(nameof(GetById), new { id = category.Id }, category);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<CategoryDto>> Update(int id, [FromBody] UpdateCategoryDto updateCategoryDto)
    {
        try
        {
            var category = await _categoryService.UpdateAsync(id, updateCategoryDto);
            return Ok(category);
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

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        try
        {
            var canDelete = await _categoryService.CanDeleteAsync(id);
            if (!canDelete)
            {
                return BadRequest(new { message = "Category cannot be deleted. It may have child categories or associated projects." });
            }

            var result = await _categoryService.DeleteAsync(id);
            return HandleBoolResult(result, "Category deleted successfully");
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("{id}/can-delete")]
    public async Task<ActionResult<bool>> CanDelete(int id)
    {
        var canDelete = await _categoryService.CanDeleteAsync(id);
        return Ok(new { canDelete });
    }

    [HttpGet("by-status/{status}")]
    public async Task<ActionResult<IEnumerable<CategoryDto>>> GetByStatus(Core.Enums.ContentStatus status)
    {
        var categories = await _categoryService.GetAllAsync();
        var filteredCategories = categories.Where(c => c.Status == status);
        return Ok(filteredCategories);
    }
}