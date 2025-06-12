using DomainArticleEnums = PimFlow.Domain.Article.Enums;
using DomainAttributeEnums = PimFlow.Domain.CustomAttribute.Enums;
using SharedEnums = PimFlow.Shared.Enums;
using PimFlow.Shared.Enums;

namespace PimFlow.Server.Mappers;

/// <summary>
/// Mapper para convertir entre enums de Domain y Shared
/// Ubicado en Server para mantener Clean Architecture
/// </summary>
public static class DomainEnumMapper
{
    /// <summary>
    /// Convierte ArticleType de Domain a Shared
    /// </summary>
    public static SharedEnums.ArticleType ToShared(DomainArticleEnums.ArticleType domainType)
    {
        return domainType switch
        {
            DomainArticleEnums.ArticleType.Footwear => SharedEnums.ArticleType.Footwear,
            DomainArticleEnums.ArticleType.Clothing => SharedEnums.ArticleType.Clothing,
            DomainArticleEnums.ArticleType.Accessory => SharedEnums.ArticleType.Accessory,
            _ => SharedEnums.ArticleType.Footwear
        };
    }

    /// <summary>
    /// Convierte ArticleType de Shared a Domain
    /// </summary>
    public static DomainArticleEnums.ArticleType ToDomain(SharedEnums.ArticleType sharedType)
    {
        return sharedType switch
        {
            SharedEnums.ArticleType.Footwear => DomainArticleEnums.ArticleType.Footwear,
            SharedEnums.ArticleType.Clothing => DomainArticleEnums.ArticleType.Clothing,
            SharedEnums.ArticleType.Accessory => DomainArticleEnums.ArticleType.Accessory,
            _ => DomainArticleEnums.ArticleType.Footwear
        };
    }

    /// <summary>
    /// Convierte AttributeType de Domain a Shared
    /// </summary>
    public static SharedEnums.AttributeType ToShared(DomainAttributeEnums.AttributeType domainType)
    {
        return domainType switch
        {
            DomainAttributeEnums.AttributeType.Text => SharedEnums.AttributeType.Text,
            DomainAttributeEnums.AttributeType.Number => SharedEnums.AttributeType.Number,
            DomainAttributeEnums.AttributeType.Integer => SharedEnums.AttributeType.Integer,
            DomainAttributeEnums.AttributeType.Boolean => SharedEnums.AttributeType.Boolean,
            DomainAttributeEnums.AttributeType.Date => SharedEnums.AttributeType.Date,
            DomainAttributeEnums.AttributeType.DateTime => SharedEnums.AttributeType.DateTime,
            DomainAttributeEnums.AttributeType.Select => SharedEnums.AttributeType.Select,
            DomainAttributeEnums.AttributeType.MultiSelect => SharedEnums.AttributeType.MultiSelect,
            DomainAttributeEnums.AttributeType.Color => SharedEnums.AttributeType.Color,
            DomainAttributeEnums.AttributeType.Url => SharedEnums.AttributeType.Url,
            DomainAttributeEnums.AttributeType.Email => SharedEnums.AttributeType.Email,
            _ => SharedEnums.AttributeType.Text
        };
    }

    /// <summary>
    /// Convierte AttributeType de Shared a Domain
    /// </summary>
    public static DomainAttributeEnums.AttributeType ToDomain(SharedEnums.AttributeType sharedType)
    {
        return sharedType switch
        {
            SharedEnums.AttributeType.Text => DomainAttributeEnums.AttributeType.Text,
            SharedEnums.AttributeType.Number => DomainAttributeEnums.AttributeType.Number,
            SharedEnums.AttributeType.Integer => DomainAttributeEnums.AttributeType.Integer,
            SharedEnums.AttributeType.Boolean => DomainAttributeEnums.AttributeType.Boolean,
            SharedEnums.AttributeType.Date => DomainAttributeEnums.AttributeType.Date,
            SharedEnums.AttributeType.DateTime => DomainAttributeEnums.AttributeType.DateTime,
            SharedEnums.AttributeType.Select => DomainAttributeEnums.AttributeType.Select,
            SharedEnums.AttributeType.MultiSelect => DomainAttributeEnums.AttributeType.MultiSelect,
            SharedEnums.AttributeType.Color => DomainAttributeEnums.AttributeType.Color,
            SharedEnums.AttributeType.Url => DomainAttributeEnums.AttributeType.Url,
            SharedEnums.AttributeType.Email => DomainAttributeEnums.AttributeType.Email,
            _ => DomainAttributeEnums.AttributeType.Text
        };
    }

    /// <summary>
    /// Convierte string a ArticleType de Domain de forma segura
    /// </summary>
    public static DomainArticleEnums.ArticleType? ParseArticleTypeToDomain(string value)
    {
        var sharedType = SharedEnums.ArticleTypeExtensions.ParseSafe(value);
        return sharedType.HasValue ? ToDomain(sharedType.Value) : null;
    }

    /// <summary>
    /// Convierte string a AttributeType de Domain de forma segura
    /// </summary>
    public static DomainAttributeEnums.AttributeType? ParseAttributeTypeToDomain(string value)
    {
        var sharedType = SharedEnums.AttributeTypeExtensions.ParseSafe(value);
        return sharedType.HasValue ? ToDomain(sharedType.Value) : null;
    }

    /// <summary>
    /// Obtiene el nombre para mostrar de un ArticleType de Domain
    /// </summary>
    public static string GetArticleTypeDisplayName(DomainArticleEnums.ArticleType domainType)
    {
        return ToShared(domainType).GetDisplayName();
    }

    /// <summary>
    /// Obtiene el nombre para mostrar de un AttributeType de Domain
    /// </summary>
    public static string GetAttributeTypeDisplayName(DomainAttributeEnums.AttributeType domainType)
    {
        return ToShared(domainType).GetDisplayName();
    }

    /// <summary>
    /// Obtiene todos los ArticleTypes para UI
    /// </summary>
    public static List<(string Value, string DisplayName, string Description)> GetAllArticleTypesForUI()
    {
        return SharedEnums.ArticleTypeExtensions.GetAllForUI()
            .Select(item => (item.Type.ToString(), item.DisplayName, item.Description))
            .ToList();
    }

    /// <summary>
    /// Obtiene todos los AttributeTypes para UI
    /// </summary>
    public static List<(string Value, string DisplayName, string Description, string Icon)> GetAllAttributeTypesForUI()
    {
        return SharedEnums.AttributeTypeExtensions.GetAllForUI()
            .Select(item => (item.Type.ToString(), item.DisplayName, item.Description, item.Icon))
            .ToList();
    }
}
