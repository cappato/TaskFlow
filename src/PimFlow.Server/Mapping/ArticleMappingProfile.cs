using AutoMapper;
using PimFlow.Domain.Entities;
using PimFlow.Shared.DTOs;
using PimFlow.Server.Mappers;

namespace PimFlow.Server.Mapping;

/// <summary>
/// Perfil de AutoMapper para mapeo entre entidades Article y DTOs
/// Centraliza toda la lógica de mapeo para reducir acoplamiento
/// </summary>
public class ArticleMappingProfile : Profile
{
    public ArticleMappingProfile()
    {
        // Article Entity -> ArticleDto
        CreateMap<Article, ArticleDto>()
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => DomainEnumMapper.ToShared(src.Type)))
            .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category != null ? src.Category.Name : null))
            .ForMember(dest => dest.SupplierName, opt => opt.MapFrom(src => src.Supplier != null ? src.Supplier.Name : null))
            .ForMember(dest => dest.CustomAttributes, opt => opt.MapFrom(src => MapCustomAttributes(src.AttributeValues)))
            .ForMember(dest => dest.Variants, opt => opt.MapFrom(src => src.Variants));

        // ArticleVariant Entity -> ArticleVariantDto
        CreateMap<ArticleVariant, ArticleVariantDto>();

        // CreateArticleDto -> Article Entity
        CreateMap<CreateArticleDto, Article>()
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => DomainEnumMapper.ToDomain(src.Type)))
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true))
            .ForMember(dest => dest.Category, opt => opt.Ignore())
            .ForMember(dest => dest.Supplier, opt => opt.Ignore())
            .ForMember(dest => dest.AttributeValues, opt => opt.Ignore())
            .ForMember(dest => dest.Variants, opt => opt.Ignore());

        // UpdateArticleDto -> Article Entity (para actualizar propiedades)
        CreateMap<UpdateArticleDto, Article>()
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type.HasValue ? DomainEnumMapper.ToDomain(src.Type.Value) : Domain.Enums.ArticleType.Footwear))
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.Category, opt => opt.Ignore())
            .ForMember(dest => dest.Supplier, opt => opt.Ignore())
            .ForMember(dest => dest.AttributeValues, opt => opt.Ignore())
            .ForMember(dest => dest.Variants, opt => opt.Ignore());
    }

    /// <summary>
    /// Mapea los valores de atributos personalizados a un diccionario
    /// </summary>
    private static Dictionary<string, object> MapCustomAttributes(ICollection<ArticleAttributeValue>? attributeValues)
    {
        if (attributeValues == null || !attributeValues.Any())
            return new Dictionary<string, object>();

        return attributeValues.ToDictionary(
            av => av.CustomAttribute.Name,
            av => (object)av.Value
        );
    }
}

/// <summary>
/// Perfil de AutoMapper para mapeo entre entidades Category y DTOs
/// </summary>
public class CategoryMappingProfile : Profile
{
    public CategoryMappingProfile()
    {
        // Category Entity -> CategoryDto
        CreateMap<Category, CategoryDto>()
            .ForMember(dest => dest.ParentCategoryName, opt => opt.MapFrom(src => src.ParentCategory != null ? src.ParentCategory.Name : null))
            .ForMember(dest => dest.SubCategoryCount, opt => opt.MapFrom(src => src.SubCategories != null ? src.SubCategories.Count : 0))
            .ForMember(dest => dest.ArticleCount, opt => opt.MapFrom(src => src.Articles != null ? src.Articles.Count : 0));

        // CreateCategoryDto -> Category Entity
        CreateMap<CreateCategoryDto, Category>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true))
            .ForMember(dest => dest.ParentCategory, opt => opt.Ignore())
            .ForMember(dest => dest.SubCategories, opt => opt.Ignore())
            .ForMember(dest => dest.Articles, opt => opt.Ignore());

        // UpdateCategoryDto -> Category Entity
        CreateMap<UpdateCategoryDto, Category>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.ParentCategory, opt => opt.Ignore())
            .ForMember(dest => dest.SubCategories, opt => opt.Ignore())
            .ForMember(dest => dest.Articles, opt => opt.Ignore());
    }
}

/// <summary>
/// Perfil de AutoMapper para mapeo entre entidades CustomAttribute y DTOs
/// </summary>
public class CustomAttributeMappingProfile : Profile
{
    public CustomAttributeMappingProfile()
    {
        // CustomAttribute Entity -> CustomAttributeDto
        CreateMap<CustomAttribute, CustomAttributeDto>()
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => DomainEnumMapper.ToShared(src.Type)));

        // CreateCustomAttributeDto -> CustomAttribute Entity
        CreateMap<CreateCustomAttributeDto, CustomAttribute>()
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => DomainEnumMapper.ToDomain(src.Type)))
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true))
            .ForMember(dest => dest.AttributeValues, opt => opt.Ignore());

        // UpdateCustomAttributeDto -> CustomAttribute Entity
        CreateMap<UpdateCustomAttributeDto, CustomAttribute>()
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type.HasValue ? DomainEnumMapper.ToDomain(src.Type.Value) : Domain.Enums.AttributeType.Text))
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.AttributeValues, opt => opt.Ignore());
    }
}
