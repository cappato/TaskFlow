namespace PimFlow.Shared.Enums;

/// <summary>
/// Tipos de artículos disponibles en el sistema
/// Enum compartido entre frontend y backend
/// </summary>
public enum ArticleType
{
    Footwear = 0,    // Calzado
    Clothing = 1,    // Ropa
    Accessory = 2    // Accesorios
}

/// <summary>
/// Extensiones para el enum ArticleType
/// </summary>
public static class ArticleTypeExtensions
{
    /// <summary>
    /// Obtiene el nombre para mostrar en UI
    /// </summary>
    public static string GetDisplayName(this ArticleType type)
    {
        return type switch
        {
            ArticleType.Footwear => "Calzado",
            ArticleType.Clothing => "Ropa",
            ArticleType.Accessory => "Accesorios",
            _ => type.ToString()
        };
    }

    /// <summary>
    /// Obtiene la descripción del tipo de artículo
    /// </summary>
    public static string GetDescription(this ArticleType type)
    {
        return type switch
        {
            ArticleType.Footwear => "Zapatillas, zapatos, botas y todo tipo de calzado deportivo",
            ArticleType.Clothing => "Remeras, pantalones, shorts, camperas y ropa deportiva",
            ArticleType.Accessory => "Gorras, mochilas, pelotas y accesorios deportivos",
            _ => string.Empty
        };
    }

    /// <summary>
    /// Obtiene el ícono CSS para mostrar en UI
    /// </summary>
    public static string GetIcon(this ArticleType type)
    {
        return type switch
        {
            ArticleType.Footwear => "fas fa-shoe-prints",
            ArticleType.Clothing => "fas fa-tshirt",
            ArticleType.Accessory => "fas fa-hat-cowboy",
            _ => "fas fa-box"
        };
    }

    /// <summary>
    /// Obtiene todos los tipos disponibles para UI
    /// </summary>
    public static List<(ArticleType Type, string DisplayName, string Description)> GetAllForUI()
    {
        return Enum.GetValues<ArticleType>()
            .Select(type => (type, type.GetDisplayName(), type.GetDescription()))
            .ToList();
    }

    /// <summary>
    /// Convierte string a ArticleType de forma segura
    /// </summary>
    public static ArticleType? ParseSafe(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        if (Enum.TryParse<ArticleType>(value, true, out var result))
            return result;

        return null;
    }
}
