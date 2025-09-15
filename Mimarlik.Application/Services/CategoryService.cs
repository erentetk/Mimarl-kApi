using AutoMapper;
using Mimarlik.Application.DTOs;
using Mimarlik.Application.Interfaces;
using Mimarlik.Core.Entities;
using Mimarlik.Core.Interfaces;
using System.Text.RegularExpressions;

namespace Mimarlik.Application.Services;

public class CategoryService : ICategoryService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CategoryService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<CategoryDto>> GetAllAsync()
    {
        var categories = await _unitOfWork.Categories.GetAllAsync();
        return _mapper.Map<IEnumerable<CategoryDto>>(categories);
    }

    public async Task<IEnumerable<CategoryDto>> GetAllWithChildrenAsync()
    {
        var categories = await _unitOfWork.Categories.GetAllWithChildrenAsync();
        return _mapper.Map<IEnumerable<CategoryDto>>(categories);
    }

    public async Task<IEnumerable<CategoryDto>> GetByParentIdAsync(int? parentId)
    {
        var categories = await _unitOfWork.Categories.GetByParentIdAsync(parentId);
        return _mapper.Map<IEnumerable<CategoryDto>>(categories);
    }

    public async Task<CategoryDto?> GetByIdAsync(int id)
    {
        var category = await _unitOfWork.Categories.GetByIdAsync(id);
        return category != null ? _mapper.Map<CategoryDto>(category) : null;
    }

    public async Task<CategoryDto?> GetBySlugAsync(string slug)
    {
        var category = await _unitOfWork.Categories.GetBySlugAsync(slug);
        return category != null ? _mapper.Map<CategoryDto>(category) : null;
    }

    public async Task<CategoryDto> CreateAsync(CreateCategoryDto createCategoryDto)
    {
        var category = _mapper.Map<Category>(createCategoryDto);
        
        if (string.IsNullOrEmpty(category.Slug))
        {
            category.Slug = GenerateSlug(category.Title);
        }

        category = await _unitOfWork.Categories.AddAsync(category);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<CategoryDto>(category);
    }

    public async Task<CategoryDto> UpdateAsync(int id, UpdateCategoryDto updateCategoryDto)
    {
        var existingCategory = await _unitOfWork.Categories.GetByIdAsync(id);
        if (existingCategory == null)
        {
            throw new KeyNotFoundException($"Category with ID {id} not found.");
        }

        _mapper.Map(updateCategoryDto, existingCategory);
        
        if (string.IsNullOrEmpty(existingCategory.Slug))
        {
            existingCategory.Slug = GenerateSlug(existingCategory.Title);
        }

        await _unitOfWork.Categories.UpdateAsync(existingCategory);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<CategoryDto>(existingCategory);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        if (!await CanDeleteAsync(id))
        {
            return false;
        }

        var category = await _unitOfWork.Categories.GetByIdAsync(id);
        if (category == null)
        {
            return false;
        }

        await _unitOfWork.Categories.DeleteAsync(category);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task<bool> CanDeleteAsync(int id)
    {
        // Check if category has children
        var hasChildren = await _unitOfWork.Categories.HasChildrenAsync(id);
        if (hasChildren)
        {
            return false;
        }

        // Check if category has projects
        var hasProjects = await _unitOfWork.Categories.HasProjectsAsync(id);
        if (hasProjects)
        {
            return false;
        }

        return true;
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