using DomainEnums = PimFlow.Domain.Enums;
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
    public static SharedEnums.ArticleType ToShared(DomainEnums.ArticleType domainType)
    {
        return domainType switch
        {
            DomainEnums.ArticleType.Footwear => SharedEnums.ArticleType.Footwear,
            DomainEnums.ArticleType.Clothing => SharedEnums.ArticleType.Clothing,
            DomainEnums.ArticleType.Accessory => SharedEnums.ArticleType.Accessory,
            _ => SharedEnums.ArticleType.Footwear
        };
    }

    /// <summary>
    /// Convierte ArticleType de Shared a Domain
    /// </summary>
    public static DomainEnums.ArticleType ToDomain(SharedEnums.ArticleType sharedType)
    {
        return sharedType switch
        {
            SharedEnums.ArticleType.Footwear => DomainEnums.ArticleType.Footwear,
            SharedEnums.ArticleType.Clothing => DomainEnums.ArticleType.Clothing,
            SharedEnums.ArticleType.Accessory => DomainEnums.ArticleType.Accessory,
            _ => DomainEnums.ArticleType.Footwear
        };
    }

    /// <summary>
    /// Convierte AttributeType de Domain a Shared
    /// </summary>
    public static SharedEnums.AttributeType ToShared(DomainEnums.AttributeType domainType)
    {
        return domainType switch
        {
            DomainEnums.AttributeType.Text => SharedEnums.AttributeType.Text,
            DomainEnums.AttributeType.Number => SharedEnums.AttributeType.Number,
            DomainEnums.AttributeType.Integer => SharedEnums.AttributeType.Integer,
            DomainEnums.AttributeType.Boolean => SharedEnums.AttributeType.Boolean,
            DomainEnums.AttributeType.Date => SharedEnums.AttributeType.Date,
            DomainEnums.AttributeType.DateTime => SharedEnums.AttributeType.DateTime,
            DomainEnums.AttributeType.Select => SharedEnums.AttributeType.Select,
            DomainEnums.AttributeType.MultiSelect => SharedEnums.AttributeType.MultiSelect,
            DomainEnums.AttributeType.Color => SharedEnums.AttributeType.Color,
            DomainEnums.AttributeType.Url => SharedEnums.AttributeType.Url,
            DomainEnums.AttributeType.Email => SharedEnums.AttributeType.Email,
            _ => SharedEnums.AttributeType.Text
        };
    }

    /// <summary>
    /// Convierte AttributeType de Shared a Domain
    /// </summary>
    public static DomainEnums.AttributeType ToDomain(SharedEnums.AttributeType sharedType)
    {
        return sharedType switch
        {
            SharedEnums.AttributeType.Text => DomainEnums.AttributeType.Text,
            SharedEnums.AttributeType.Number => DomainEnums.AttributeType.Number,
            SharedEnums.AttributeType.Integer => DomainEnums.AttributeType.Integer,
            SharedEnums.AttributeType.Boolean => DomainEnums.AttributeType.Boolean,
            SharedEnums.AttributeType.Date => DomainEnums.AttributeType.Date,
            SharedEnums.AttributeType.DateTime => DomainEnums.AttributeType.DateTime,
            SharedEnums.AttributeType.Select => DomainEnums.AttributeType.Select,
            SharedEnums.AttributeType.MultiSelect => DomainEnums.AttributeType.MultiSelect,
            SharedEnums.AttributeType.Color => DomainEnums.AttributeType.Color,
            SharedEnums.AttributeType.Url => DomainEnums.AttributeType.Url,
            SharedEnums.AttributeType.Email => DomainEnums.AttributeType.Email,
            _ => DomainEnums.AttributeType.Text
        };
    }

    /// <summary>
    /// Convierte string a ArticleType de Domain de forma segura
    /// </summary>
    public static DomainEnums.ArticleType? ParseArticleTypeToDomain(string value)
    {
        var sharedType = SharedEnums.ArticleTypeExtensions.ParseSafe(value);
        return sharedType.HasValue ? ToDomain(sharedType.Value) : null;
    }

    /// <summary>
    /// Convierte string a AttributeType de Domain de forma segura
    /// </summary>
    public static DomainEnums.AttributeType? ParseAttributeTypeToDomain(string value)
    {
        var sharedType = SharedEnums.AttributeTypeExtensions.ParseSafe(value);
        return sharedType.HasValue ? ToDomain(sharedType.Value) : null;
    }

    /// <summary>
    /// Obtiene el nombre para mostrar de un ArticleType de Domain
    /// </summary>
    public static string GetArticleTypeDisplayName(DomainEnums.ArticleType domainType)
    {
        return ToShared(domainType).GetDisplayName();
    }

    /// <summary>
    /// Obtiene el nombre para mostrar de un AttributeType de Domain
    /// </summary>
    public static string GetAttributeTypeDisplayName(DomainEnums.AttributeType domainType)
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
