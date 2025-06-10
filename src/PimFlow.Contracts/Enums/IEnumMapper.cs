namespace PimFlow.Contracts.Enums;

/// <summary>
/// Interfaz para mapear enums entre diferentes capas
/// Permite desacoplar Shared de Domain manteniendo la funcionalidad de mapeo
/// </summary>
public interface IEnumMapper
{
    /// <summary>
    /// Convierte ArticleType de Domain a Shared
    /// </summary>
    TShared MapArticleTypeToShared<TDomain, TShared>(TDomain domainType) 
        where TDomain : Enum 
        where TShared : Enum;

    /// <summary>
    /// Convierte ArticleType de Shared a Domain
    /// </summary>
    TDomain MapArticleTypeToDomain<TShared, TDomain>(TShared sharedType) 
        where TShared : Enum 
        where TDomain : Enum;

    /// <summary>
    /// Convierte AttributeType de Domain a Shared
    /// </summary>
    TShared MapAttributeTypeToShared<TDomain, TShared>(TDomain domainType) 
        where TDomain : Enum 
        where TShared : Enum;

    /// <summary>
    /// Convierte AttributeType de Shared a Domain
    /// </summary>
    TDomain MapAttributeTypeToDomain<TShared, TDomain>(TShared sharedType) 
        where TShared : Enum 
        where TDomain : Enum;

    /// <summary>
    /// Convierte string a ArticleType de Domain de forma segura
    /// </summary>
    TDomain? ParseArticleTypeToDomain<TDomain>(string value) where TDomain : struct, Enum;

    /// <summary>
    /// Convierte string a AttributeType de Domain de forma segura
    /// </summary>
    TDomain? ParseAttributeTypeToDomain<TDomain>(string value) where TDomain : struct, Enum;

    /// <summary>
    /// Obtiene el nombre para mostrar de un ArticleType de Domain
    /// </summary>
    string GetArticleTypeDisplayName<TDomain>(TDomain domainType) where TDomain : Enum;

    /// <summary>
    /// Obtiene el nombre para mostrar de un AttributeType de Domain
    /// </summary>
    string GetAttributeTypeDisplayName<TDomain>(TDomain domainType) where TDomain : Enum;

    /// <summary>
    /// Obtiene todos los ArticleTypes para UI
    /// </summary>
    List<(string Value, string DisplayName, string Description)> GetAllArticleTypesForUI();

    /// <summary>
    /// Obtiene todos los AttributeTypes para UI
    /// </summary>
    List<(string Value, string DisplayName, string Description, string Icon)> GetAllAttributeTypesForUI();
}
