using Microsoft.AspNetCore.Mvc;
using Mimarlik.Application.DTOs;
using Mimarlik.Application.Interfaces;

namespace Mimarlik.API.Controllers;

[Route("api/[controller]")]
public class ProjectsController : BaseApiController
{
    private readonly IProjectService _projectService;

    public ProjectsController(IProjectService projectService)
    {
        _projectService = projectService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProjectDto>>> GetAll()
    {
        var projects = await _projectService.GetAllAsync();
        return Ok(projects);
    }

    [HttpGet("published")]
    public async Task<ActionResult<IEnumerable<ProjectDto>>> GetPublished()
    {
        var projects = await _projectService.GetPublishedAsync();
        return Ok(projects);
    }

    [HttpGet("featured")]
    public async Task<ActionResult<IEnumerable<ProjectDto>>> GetFeatured()
    {
        var projects = await _projectService.GetFeaturedAsync();
        return Ok(projects);
    }

    [HttpGet("by-category/{categoryId}")]
    public async Task<ActionResult<IEnumerable<ProjectDto>>> GetByCategoryId(int categoryId)
    {
        var projects = await _projectService.GetByCategoryIdAsync(categoryId);
        return Ok(projects);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ProjectDto>> GetById(int id)
    {
        var project = await _projectService.GetByIdAsync(id);
        return HandleResult(project);
    }

    [HttpGet("{id}/details")]
    public async Task<ActionResult<ProjectDto>> GetWithDetails(int id)
    {
        var project = await _projectService.GetWithDetailsAsync(id);
        return HandleResult(project);
    }

    [HttpGet("slug/{slug}")]
    public async Task<ActionResult<ProjectDto>> GetBySlug(string slug)
    {
        var project = await _projectService.GetBySlugAsync(slug);
        return HandleResult(project);
    }

    [HttpPost]
    public async Task<ActionResult<ProjectDto>> Create([FromBody] CreateProjectDto createProjectDto)
    {
        try
        {
            var project = await _projectService.CreateAsync(createProjectDto);
            return CreatedAtAction(nameof(GetById), new { id = project.Id }, project);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ProjectDto>> Update(int id, [FromBody] UpdateProjectDto updateProjectDto)
    {
        try
        {
            var project = await _projectService.UpdateAsync(id, updateProjectDto);
            return Ok(project);
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
            var result = await _projectService.DeleteAsync(id);
            return HandleBoolResult(result, "Project deleted successfully");
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("generate-slug")]
    public async Task<ActionResult<string>> GenerateSlug([FromBody] GenerateSlugRequest request)
    {
        try
        {
            var slug = await _projectService.GenerateSlugAsync(request.Title, request.ExcludeId);
            return Ok(new { slug });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("by-status/{status}")]
    public async Task<ActionResult<IEnumerable<ProjectDto>>> GetByStatus(Core.Enums.ContentStatus status)
    {
        var projects = await _projectService.GetAllAsync();
        var filteredProjects = projects.Where(p => p.Status == status);
        return Ok(filteredProjects);
    }
}

public class GenerateSlugRequest
{
    public string Title { get; set; } = string.Empty;
    public int? ExcludeId { get; set; }
}