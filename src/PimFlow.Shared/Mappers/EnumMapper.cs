using SharedEnums = PimFlow.Shared.Enums;
using PimFlow.Shared.Enums;

namespace PimFlow.Shared.Mappers;

/// <summary>
/// Mapper simplificado para trabajar solo con enums de Shared
/// Los métodos que requieren Domain se han movido al proyecto Server
/// </summary>
public static class EnumMapper
{
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
