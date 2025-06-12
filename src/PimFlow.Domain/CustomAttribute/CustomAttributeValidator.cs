using PimFlow.Domain.CustomAttribute.Enums;
using PimFlow.Domain.Article.ValueObjects;
using PimFlow.Domain.Common;
using System.Text.Json;

namespace PimFlow.Domain.CustomAttribute;

/// <summary>
/// Validador específico para el agregado CustomAttribute
/// Centraliza todas las validaciones relacionadas con atributos personalizados
/// </summary>
public static class CustomAttributeValidator
{
    /// <summary>
    /// Validaciones para nombre de atributo
    /// </summary>
    public static class Name
    {
        public static bool IsValid(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return false;

            var trimmed = value.Trim();
            return trimmed.Length >= 2 && trimmed.Length <= 100 && 
                   char.IsLetter(trimmed[0]) && // Debe empezar con letra
                   trimmed.All(c => char.IsLetterOrDigit(c) || c == '_'); // Solo letras, números y _
        }

        public static string GetValidationMessage() =>
            "El nombre debe tener entre 2 y 100 caracteres, empezar con letra y contener solo letras, números y guiones bajos";

        public static Result Validate(string value)
        {
            if (IsValid(value))
                return Result.Success();

            return Result.Failure(GetValidationMessage());
        }
    }

    /// <summary>
    /// Validaciones para nombre de visualización
    /// </summary>
    public static class DisplayName
    {
        public static bool IsValid(string value) => ProductName.IsValid(value);

        public static string GetValidationMessage() =>
            "El nombre de visualización debe tener entre 2 y 200 caracteres";

        public static Result Validate(string value)
        {
            if (IsValid(value))
                return Result.Success();

            return Result.Failure(GetValidationMessage());
        }
    }

    /// <summary>
    /// Validaciones para tipo de atributo
    /// </summary>
    public static class Type
    {
        public static bool IsValid(AttributeType type)
        {
            return Enum.IsDefined(typeof(AttributeType), type);
        }

        public static string GetValidationMessage() =>
            "El tipo de atributo debe ser válido";

        public static Result Validate(AttributeType type)
        {
            if (IsValid(type))
                return Result.Success();

            return Result.Failure(GetValidationMessage());
        }
    }

    /// <summary>
    /// Validaciones para valor por defecto
    /// </summary>
    public static class DefaultValue
    {
        public static bool IsValid(string? value, AttributeType type)
        {
            if (string.IsNullOrEmpty(value))
                return true; // Valor por defecto es opcional

            return type switch
            {
                AttributeType.Text => value.Length <= 1000,
                AttributeType.Number => decimal.TryParse(value, out _),
                AttributeType.Integer => int.TryParse(value, out _),
                AttributeType.Boolean => bool.TryParse(value, out _),
                AttributeType.Date => DateTime.TryParse(value, out _),
                AttributeType.DateTime => DateTime.TryParse(value, out _),
                AttributeType.Color => IsValidHexColor(value),
                AttributeType.Url => Uri.TryCreate(value, UriKind.Absolute, out _),
                AttributeType.Email => PimFlow.Domain.User.ValueObjects.Email.IsValid(value),
                AttributeType.Select => true, // Se validará contra las opciones
                AttributeType.MultiSelect => true, // Se validará contra las opciones
                _ => false
            };
        }

        private static bool IsValidHexColor(string value)
        {
            return value.StartsWith("#") && value.Length == 7 && 
                   value[1..].All(c => char.IsDigit(c) || (c >= 'A' && c <= 'F') || (c >= 'a' && c <= 'f'));
        }

        public static string GetValidationMessage(AttributeType type) =>
            type switch
            {
                AttributeType.Number => "El valor por defecto debe ser un número válido",
                AttributeType.Integer => "El valor por defecto debe ser un entero válido",
                AttributeType.Boolean => "El valor por defecto debe ser true o false",
                AttributeType.Date => "El valor por defecto debe ser una fecha válida",
                AttributeType.DateTime => "El valor por defecto debe ser una fecha y hora válida",
                AttributeType.Color => "El valor por defecto debe ser un color hexadecimal válido (#RRGGBB)",
                AttributeType.Url => "El valor por defecto debe ser una URL válida",
                AttributeType.Email => "El valor por defecto debe ser un email válido",
                _ => "El valor por defecto no es válido para este tipo de atributo"
            };

        public static Result Validate(string? value, AttributeType type)
        {
            if (IsValid(value, type))
                return Result.Success();

            return Result.Failure(GetValidationMessage(type));
        }
    }

    /// <summary>
    /// Validaciones para reglas de validación JSON
    /// </summary>
    public static class ValidationRules
    {
        public static bool IsValid(string? rules)
        {
            if (string.IsNullOrEmpty(rules))
                return true; // Reglas son opcionales

            try
            {
                JsonDocument.Parse(rules);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static string GetValidationMessage() =>
            "Las reglas de validación deben ser un JSON válido";

        public static Result Validate(string? rules)
        {
            if (IsValid(rules))
                return Result.Success();

            return Result.Failure(GetValidationMessage());
        }
    }

    /// <summary>
    /// Validación completa de un atributo personalizado
    /// </summary>
    public static Result ValidateForCreation(
        string name, 
        string displayName, 
        AttributeType type, 
        string? defaultValue = null, 
        string? validationRules = null)
    {
        var nameResult = Name.Validate(name);
        if (nameResult.IsFailure)
            return nameResult;

        var displayNameResult = DisplayName.Validate(displayName);
        if (displayNameResult.IsFailure)
            return displayNameResult;

        var typeResult = Type.Validate(type);
        if (typeResult.IsFailure)
            return typeResult;

        var defaultValueResult = DefaultValue.Validate(defaultValue, type);
        if (defaultValueResult.IsFailure)
            return defaultValueResult;

        var rulesResult = ValidationRules.Validate(validationRules);
        if (rulesResult.IsFailure)
            return rulesResult;

        return Result.Success();
    }
}
