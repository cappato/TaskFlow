using PimFlow.Domain.Article.ValueObjects;
using PimFlow.Domain.User.ValueObjects;
using PimFlow.Contracts.Validation;

namespace PimFlow.Domain.Validation;

/// <summary>
/// Validadores centralizados que exponen las validaciones de Value Objects
/// Fuente única de verdad para todas las validaciones de formato
/// Delega a SharedValidationRules para mantener consistencia
/// </summary>
public static class DomainValidators
{
    /// <summary>
    /// Validaciones para SKU - delega a Value Object para autoridad final
    /// </summary>
    public static class SkuValidator
    {
        public static bool IsValid(string value) => SKU.IsValid(value);

        public static string GetValidationMessage() =>
            SharedValidationRules.Sku.GetValidationMessage();

        public static (bool IsValid, string? ErrorMessage) Validate(string value)
        {
            if (IsValid(value))
                return (true, null);

            return (false, GetValidationMessage());
        }
    }

    /// <summary>
    /// Validaciones para Brand - delega a Value Object para autoridad final
    /// </summary>
    public static class BrandValidator
    {
        public static bool IsValid(string value) => Brand.IsValid(value);

        public static string GetValidationMessage() =>
            SharedValidationRules.Brand.GetValidationMessage();

        public static (bool IsValid, string? ErrorMessage) Validate(string value)
        {
            if (IsValid(value))
                return (true, null);

            return (false, GetValidationMessage());
        }
    }

    /// <summary>
    /// Validaciones para ProductName - delega a Value Object para autoridad final
    /// </summary>
    public static class ProductNameValidator
    {
        public static bool IsValid(string value) => ProductName.IsValid(value);

        public static string GetValidationMessage() =>
            SharedValidationRules.ProductName.GetValidationMessage();

        public static (bool IsValid, string? ErrorMessage) Validate(string value)
        {
            if (IsValid(value))
                return (true, null);

            return (false, GetValidationMessage());
        }
    }

    /// <summary>
    /// Validaciones para Email - delega a Value Object para autoridad final
    /// </summary>
    public static class EmailValidator
    {
        public static bool IsValid(string value) => Email.IsValid(value);

        public static string GetValidationMessage() =>
            "Email debe tener un formato válido y no exceder 200 caracteres";

        public static (bool IsValid, string? ErrorMessage) Validate(string value)
        {
            if (IsValid(value))
                return (true, null);

            return (false, GetValidationMessage());
        }
    }

    /// <summary>
    /// Validaciones comunes para campos de texto
    /// </summary>
    public static class TextValidator
    {
        public static bool IsValidLength(string value, int minLength, int maxLength)
        {
            if (string.IsNullOrWhiteSpace(value))
                return false;
            
            var trimmed = value.Trim();
            return trimmed.Length >= minLength && trimmed.Length <= maxLength;
        }
        
        public static (bool IsValid, string? ErrorMessage) ValidateLength(
            string value, 
            int minLength, 
            int maxLength, 
            string fieldName)
        {
            if (IsValidLength(value, minLength, maxLength))
                return (true, null);
            
            return (false, $"{fieldName} debe tener entre {minLength} y {maxLength} caracteres");
        }
        
        public static bool IsRequired(string value)
        {
            return !string.IsNullOrWhiteSpace(value);
        }
        
        public static (bool IsValid, string? ErrorMessage) ValidateRequired(string value, string fieldName)
        {
            if (IsRequired(value))
                return (true, null);
            
            return (false, $"{fieldName} es requerido");
        }
    }

    /// <summary>
    /// Validaciones para IDs
    /// </summary>
    public static class IdValidator
    {
        public static bool IsValidId(int? id)
        {
            return id.HasValue && id.Value > 0;
        }
        
        public static (bool IsValid, string? ErrorMessage) ValidateId(int? id, string fieldName)
        {
            if (IsValidId(id))
                return (true, null);
            
            return (false, $"{fieldName} debe ser mayor a 0");
        }
    }
}
