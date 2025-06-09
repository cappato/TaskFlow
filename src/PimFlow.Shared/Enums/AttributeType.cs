namespace PimFlow.Shared.Enums;

/// <summary>
/// Tipos de atributos personalizados disponibles en el sistema
/// Enum compartido entre frontend y backend
/// </summary>
public enum AttributeType
{
    Text = 0,        // Texto libre
    Number = 1,      // Número decimal
    Integer = 2,     // Número entero
    Boolean = 3,     // Verdadero/Falso
    Date = 4,        // Fecha
    DateTime = 5,    // Fecha y hora
    Select = 6,      // Lista de opciones
    MultiSelect = 7, // Múltiples opciones
    Color = 8,       // Color (hex)
    Url = 9,         // URL
    Email = 10       // Email
}

/// <summary>
/// Extensiones para el enum AttributeType
/// </summary>
public static class AttributeTypeExtensions
{
    /// <summary>
    /// Obtiene el nombre para mostrar en UI
    /// </summary>
    public static string GetDisplayName(this AttributeType type)
    {
        return type switch
        {
            AttributeType.Text => "Texto",
            AttributeType.Number => "Número Decimal",
            AttributeType.Integer => "Número Entero",
            AttributeType.Boolean => "Verdadero/Falso",
            AttributeType.Date => "Fecha",
            AttributeType.DateTime => "Fecha y Hora",
            AttributeType.Select => "Lista de Opciones",
            AttributeType.MultiSelect => "Múltiples Opciones",
            AttributeType.Color => "Color",
            AttributeType.Url => "URL",
            AttributeType.Email => "Email",
            _ => type.ToString()
        };
    }

    /// <summary>
    /// Obtiene la descripción del tipo de atributo
    /// </summary>
    public static string GetDescription(this AttributeType type)
    {
        return type switch
        {
            AttributeType.Text => "Campo de texto libre para cualquier valor",
            AttributeType.Number => "Número decimal con punto flotante",
            AttributeType.Integer => "Número entero sin decimales",
            AttributeType.Boolean => "Valor verdadero o falso (checkbox)",
            AttributeType.Date => "Fecha sin hora (dd/mm/yyyy)",
            AttributeType.DateTime => "Fecha con hora (dd/mm/yyyy hh:mm)",
            AttributeType.Select => "Lista desplegable con una opción",
            AttributeType.MultiSelect => "Lista con múltiples opciones seleccionables",
            AttributeType.Color => "Selector de color con valor hexadecimal",
            AttributeType.Url => "Dirección web válida (http/https)",
            AttributeType.Email => "Dirección de correo electrónico válida",
            _ => string.Empty
        };
    }

    /// <summary>
    /// Obtiene el tipo de input HTML para el tipo de atributo
    /// </summary>
    public static string GetHtmlInputType(this AttributeType type)
    {
        return type switch
        {
            AttributeType.Text => "text",
            AttributeType.Number => "number",
            AttributeType.Integer => "number",
            AttributeType.Boolean => "checkbox",
            AttributeType.Date => "date",
            AttributeType.DateTime => "datetime-local",
            AttributeType.Select => "select",
            AttributeType.MultiSelect => "select",
            AttributeType.Color => "color",
            AttributeType.Url => "url",
            AttributeType.Email => "email",
            _ => "text"
        };
    }

    /// <summary>
    /// Obtiene el ícono CSS para mostrar en UI
    /// </summary>
    public static string GetIcon(this AttributeType type)
    {
        return type switch
        {
            AttributeType.Text => "fas fa-font",
            AttributeType.Number => "fas fa-calculator",
            AttributeType.Integer => "fas fa-hashtag",
            AttributeType.Boolean => "fas fa-toggle-on",
            AttributeType.Date => "fas fa-calendar-day",
            AttributeType.DateTime => "fas fa-calendar-alt",
            AttributeType.Select => "fas fa-list",
            AttributeType.MultiSelect => "fas fa-list-check",
            AttributeType.Color => "fas fa-palette",
            AttributeType.Url => "fas fa-link",
            AttributeType.Email => "fas fa-envelope",
            _ => "fas fa-question"
        };
    }

    /// <summary>
    /// Indica si el tipo de atributo requiere opciones predefinidas
    /// </summary>
    public static bool RequiresOptions(this AttributeType type)
    {
        return type == AttributeType.Select || type == AttributeType.MultiSelect;
    }

    /// <summary>
    /// Indica si el tipo de atributo permite múltiples valores
    /// </summary>
    public static bool AllowsMultipleValues(this AttributeType type)
    {
        return type == AttributeType.MultiSelect;
    }

    /// <summary>
    /// Obtiene todos los tipos disponibles para UI
    /// </summary>
    public static List<(AttributeType Type, string DisplayName, string Description, string Icon)> GetAllForUI()
    {
        return Enum.GetValues<AttributeType>()
            .Select(type => (type, type.GetDisplayName(), type.GetDescription(), type.GetIcon()))
            .ToList();
    }

    /// <summary>
    /// Convierte string a AttributeType de forma segura
    /// </summary>
    public static AttributeType? ParseSafe(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        if (Enum.TryParse<AttributeType>(value, true, out var result))
            return result;

        return null;
    }

    /// <summary>
    /// Valida si un valor es compatible con el tipo de atributo
    /// </summary>
    public static bool IsValidValue(this AttributeType type, object? value)
    {
        if (value == null)
            return true;

        var stringValue = value.ToString();
        if (string.IsNullOrEmpty(stringValue))
            return true;

        return type switch
        {
            AttributeType.Text => true,
            AttributeType.Number => decimal.TryParse(stringValue, out _),
            AttributeType.Integer => int.TryParse(stringValue, out _),
            AttributeType.Boolean => bool.TryParse(stringValue, out _),
            AttributeType.Date => DateTime.TryParse(stringValue, out _),
            AttributeType.DateTime => DateTime.TryParse(stringValue, out _),
            AttributeType.Select => true, // Validación específica en el contexto
            AttributeType.MultiSelect => true, // Validación específica en el contexto
            AttributeType.Color => stringValue.StartsWith("#") && stringValue.Length == 7,
            AttributeType.Url => Uri.TryCreate(stringValue, UriKind.Absolute, out _),
            AttributeType.Email => stringValue.Contains("@") && stringValue.Contains("."),
            _ => true
        };
    }
}
