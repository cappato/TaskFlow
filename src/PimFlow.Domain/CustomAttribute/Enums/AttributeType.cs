namespace PimFlow.Domain.CustomAttribute.Enums;

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
