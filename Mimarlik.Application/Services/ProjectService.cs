using AutoMapper;
using Mimarlik.Application.DTOs;
using Mimarlik.Application.Interfaces;
using Mimarlik.Core.Entities;
using Mimarlik.Core.Interfaces;
using System.Text.RegularExpressions;

namespace Mimarlik.Application.Services;

public class ProjectService : IProjectService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ProjectService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ProjectDto>> GetAllAsync()
    {
        var projects = await _unitOfWork.Projects.GetAllAsync();
        return _mapper.Map<IEnumerable<ProjectDto>>(projects);
    }

    public async Task<IEnumerable<ProjectDto>> GetPublishedAsync()
    {
        var projects = await _unitOfWork.Projects.GetPublishedAsync();
        return _mapper.Map<IEnumerable<ProjectDto>>(projects);
    }

    public async Task<IEnumerable<ProjectDto>> GetFeaturedAsync()
    {
        var projects = await _unitOfWork.Projects.GetFeaturedAsync();
        return _mapper.Map<IEnumerable<ProjectDto>>(projects);
    }

    public async Task<IEnumerable<ProjectDto>> GetByCategoryIdAsync(int categoryId)
    {
        var projects = await _unitOfWork.Projects.GetByCategoryIdAsync(categoryId);
        return _mapper.Map<IEnumerable<ProjectDto>>(projects);
    }

    public async Task<ProjectDto?> GetByIdAsync(int id)
    {
        var project = await _unitOfWork.Projects.GetByIdAsync(id);
        return project != null ? _mapper.Map<ProjectDto>(project) : null;
    }

    public async Task<ProjectDto?> GetBySlugAsync(string slug)
    {
        var project = await _unitOfWork.Projects.GetBySlugAsync(slug);
        return project != null ? _mapper.Map<ProjectDto>(project) : null;
    }

    public async Task<ProjectDto?> GetWithDetailsAsync(int id)
    {
        var project = await _unitOfWork.Projects.GetWithDetailsAsync(id);
        return project != null ? _mapper.Map<ProjectDto>(project) : null;
    }

    public async Task<ProjectDto> CreateAsync(CreateProjectDto createProjectDto)
    {
        // Validate CategoryId if provided
        if (createProjectDto.CategoryId.HasValue && createProjectDto.CategoryId.Value > 0)
        {
            var categoryExists = await _unitOfWork.Categories.GetByIdAsync(createProjectDto.CategoryId.Value);
            if (categoryExists == null)
            {
                throw new ArgumentException($"Category with ID {createProjectDto.CategoryId.Value} does not exist. Either leave CategoryId empty or provide a valid category ID.");
            }
        }
        else if (createProjectDto.CategoryId == 0)
        {
            // Set to null if CategoryId is 0 (invalid foreign key)
            createProjectDto.CategoryId = null;
        }

        var project = _mapper.Map<Project>(createProjectDto);
        
        if (string.IsNullOrEmpty(project.Slug))
        {
            project.Slug = await GenerateSlugAsync(project.Title);
        }

        // First save the project to get the ID
        project = await _unitOfWork.Projects.AddAsync(project);
        await _unitOfWork.SaveChangesAsync();
        
        // Then add content blocks with the correct ProjectId
        if (createProjectDto.ContentBlocks.Any())
        {
            foreach (var contentBlockDto in createProjectDto.ContentBlocks)
            {
                var contentBlock = _mapper.Map<ContentBlock>(contentBlockDto);
                contentBlock.ProjectId = project.Id; // Now project.Id has the actual value
                await _unitOfWork.ContentBlocks.AddAsync(contentBlock);
            }
            await _unitOfWork.SaveChangesAsync(); // Save content blocks
        }

        return _mapper.Map<ProjectDto>(project);
    }

    public async Task<ProjectDto> UpdateAsync(int id, UpdateProjectDto updateProjectDto)
    {
        var existingProject = await _unitOfWork.Projects.GetWithDetailsAsync(id);
        if (existingProject == null)
        {
            throw new KeyNotFoundException($"Project with ID {id} not found.");
        }

        // If project status is being changed to Hidden or Draft, remove all its photos from slider
        if (updateProjectDto.Status != Core.Enums.ContentStatus.Published)
        {
            var projectPhotos = await _unitOfWork.Photos.GetByProjectIdAsync(id);
            foreach (var photo in projectPhotos.Where(p => p.IsHomepageSlider))
            {
                photo.IsHomepageSlider = false;
                photo.SliderText = string.Empty;
                await _unitOfWork.Photos.UpdateAsync(photo);
            }
        }

        _mapper.Map(updateProjectDto, existingProject);
        
        if (string.IsNullOrEmpty(existingProject.Slug))
        {
            existingProject.Slug = await GenerateSlugAsync(existingProject.Title, id);
        }

        // Update content blocks
        await _unitOfWork.ContentBlocks.DeleteByProjectIdAsync(id);
        
        if (updateProjectDto.ContentBlocks.Any())
        {
            foreach (var contentBlockDto in updateProjectDto.ContentBlocks)
            {
                var contentBlock = _mapper.Map<ContentBlock>(contentBlockDto);
                contentBlock.ProjectId = id;
                await _unitOfWork.ContentBlocks.AddAsync(contentBlock);
            }
        }

        await _unitOfWork.Projects.UpdateAsync(existingProject);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<ProjectDto>(existingProject);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var project = await _unitOfWork.Projects.GetWithDetailsAsync(id);
        if (project == null)
        {
            return false;
        }

        await _unitOfWork.BeginTransactionAsync();
        try
        {
            // Delete content blocks
            await _unitOfWork.ContentBlocks.DeleteByProjectIdAsync(id);

            // Delete photos and their files
            var photos = await _unitOfWork.Photos.GetByProjectIdAsync(id);
            foreach (var photo in photos)
            {
                await _unitOfWork.Photos.DeletePhotoFileAsync(photo.FilePath);
            }
            await _unitOfWork.Photos.DeleteRangeAsync(photos);

            // Delete translations
            await _unitOfWork.Translations.DeleteByEntityAsync("Project", id);

            // Delete the project itself
            await _unitOfWork.Projects.DeleteAsync(project);

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

    public async Task<string> GenerateSlugAsync(string title, int? excludeId = null)
    {
        var baseSlug = GenerateSlug(title);
        var slug = baseSlug;
        var counter = 1;

        while (await _unitOfWork.Projects.SlugExistsAsync(slug, excludeId))
        {
            slug = $"{baseSlug}-{counter}";
            counter++;
        }

        return slug;
    }

    private static string GenerateSlug(string title)
    {
        // Convert to lowercase
        var slug = title.ToLowerInvariant();

        // Replace Turkish characters
        slug = slug.Replace("ç", "c")
                   .Replace("ğ", "g")
                   .Replace("ı", "i")
                   .Replace("ö", "o")
                   .Replace("ş", "s")
                   .Replace("ü", "u");

        // Remove invalid characters
        slug = Regex.Replace(slug, @"[^a-z0-9\s-]", "");

        // Replace multiple spaces with single space
        slug = Regex.Replace(slug, @"\s+", " ").Trim();

        // Replace spaces with hyphens
        slug = slug.Replace(" ", "-");

        // Remove multiple hyphens
        slug = Regex.Replace(slug, @"-+", "-");

        return slug;
    }
}