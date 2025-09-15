using AutoMapper;
using Mimarlik.Application.DTOs;
using Mimarlik.Core.Entities;

namespace Mimarlik.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Category, CategoryDto>().ReverseMap();
        CreateMap<CreateCategoryDto, Category>();
        CreateMap<UpdateCategoryDto, Category>();

        CreateMap<Project, ProjectDto>().ReverseMap();
        CreateMap<CreateProjectDto, Project>();
        CreateMap<UpdateProjectDto, Project>();

        CreateMap<Photo, PhotoDto>()
            .ForMember(dest => dest.Url, opt => opt.Ignore()) // Will be set by service
            .ReverseMap();
        CreateMap<CreatePhotoDto, Photo>();
        CreateMap<UpdatePhotoDto, Photo>();

        CreateMap<Photo, SliderPhotoDto>()
            .ForMember(dest => dest.Url, opt => opt.Ignore())
            .ForMember(dest => dest.ProjectTitle, opt => opt.MapFrom(src => src.Project != null ? src.Project.Title : null))
            .ForMember(dest => dest.ProjectSlug, opt => opt.MapFrom(src => src.Project != null ? src.Project.Slug : null));

        CreateMap<ContentBlock, ContentBlockDto>().ReverseMap();
        CreateMap<CreateContentBlockDto, ContentBlock>();
        CreateMap<UpdateContentBlockDto, ContentBlock>();

        CreateMap<Language, LanguageDto>().ReverseMap();
        CreateMap<CreateLanguageDto, Language>();
        CreateMap<UpdateLanguageDto, Language>();

        CreateMap<Translation, TranslationDto>().ReverseMap();
        CreateMap<CreateTranslationDto, Translation>();
        CreateMap<UpdateTranslationDto, Translation>();
    }
}