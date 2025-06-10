namespace PimFlow.Contracts.Validation;

/// <summary>
/// Interfaces para reglas de validación compartidas
/// Permite que Domain implemente las reglas y Shared las use sin dependencias directas
/// </summary>
public interface IValidationRules
{
    /// <summary>
    /// Validaciones para SKU
    /// </summary>
    public interface ISku
    {
        static abstract bool IsValid(string value);
        static abstract string GetValidationMessage();
        static abstract (bool IsValid, string? ErrorMessage) Validate(string value);
    }

    /// <summary>
    /// Validaciones para Brand
    /// </summary>
    public interface IBrand
    {
        static abstract bool IsValid(string value);
        static abstract string GetValidationMessage();
        static abstract (bool IsValid, string? ErrorMessage) Validate(string value);
    }

    /// <summary>
    /// Validaciones para ProductName
    /// </summary>
    public interface IProductName
    {
        static abstract bool IsValid(string value);
        static abstract string GetValidationMessage();
        static abstract (bool IsValid, string? ErrorMessage) Validate(string value);
    }

    /// <summary>
    /// Validaciones para Email
    /// </summary>
    public interface IEmail
    {
        static abstract bool IsValid(string value);
        static abstract string GetValidationMessage();
        static abstract (bool IsValid, string? ErrorMessage) Validate(string value);
    }

    /// <summary>
    /// Validaciones para campos de texto
    /// </summary>
    public interface IText
    {
        static abstract bool IsValidLength(string value, int minLength, int maxLength);
        static abstract (bool IsValid, string? ErrorMessage) ValidateLength(string value, int minLength, int maxLength, string fieldName);
        static abstract bool IsRequired(string value);
        static abstract (bool IsValid, string? ErrorMessage) ValidateRequired(string value, string fieldName);
    }

    /// <summary>
    /// Validaciones para IDs
    /// </summary>
    public interface IId
    {
        static abstract bool IsValidId(int? id);
        static abstract (bool IsValid, string? ErrorMessage) ValidateId(int? id, string fieldName);
    }
}

/// <summary>
/// Implementación concreta de validaciones que puede ser usada por Shared
/// Sin depender de Domain
/// </summary>
public static class SharedValidationRules
{
    /// <summary>
    /// Validaciones básicas para SKU (implementación simplificada para Shared)
    /// </summary>
    public static class Sku
    {
        public static bool IsValid(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return false;

            var normalized = value.Trim().ToUpperInvariant();
            return normalized.Length >= 3 && 
                   normalized.Length <= 50 && 
                   normalized.All(c => char.IsLetterOrDigit(c) || c == '-');
        }

        public static string GetValidationMessage() => 
            "SKU debe contener solo letras mayúsculas, números y guiones, entre 3 y 50 caracteres";

        public static (bool IsValid, string? ErrorMessage) Validate(string value)
        {
            if (IsValid(value))
                return (true, null);
            
            return (false, GetValidationMessage());
        }
    }

    /// <summary>
    /// Validaciones básicas para Brand
    /// </summary>
    public static class Brand
    {
        public static bool IsValid(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return false;

            var trimmed = value.Trim();
            return trimmed.Length >= 2 && trimmed.Length <= 100;
        }

        public static string GetValidationMessage() => 
            "Marca debe tener entre 2 y 100 caracteres";

        public static (bool IsValid, string? ErrorMessage) Validate(string value)
        {
            if (IsValid(value))
                return (true, null);
            
            return (false, GetValidationMessage());
        }
    }

    /// <summary>
    /// Validaciones básicas para ProductName
    /// </summary>
    public static class ProductName
    {
        public static bool IsValid(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return false;

            var trimmed = value.Trim();
            return trimmed.Length >= 2 && trimmed.Length <= 200;
        }

        public static string GetValidationMessage() => 
            "Nombre debe tener entre 2 y 200 caracteres";

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
    public static class Text
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
    public static class Id
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

    /// <summary>
    /// Validaciones específicas para UI/Display
    /// </summary>
    public static class Display
    {
        /// <summary>
        /// Valida si un artículo es válido para mostrar en UI
        /// </summary>
        public static bool IsArticleValidForDisplay(string sku, string name, string brand)
        {
            return Text.IsRequired(sku) &&
                   Text.IsRequired(name) &&
                   Text.IsRequired(brand);
        }

        /// <summary>
        /// Valida si una categoría es válida para mostrar en UI
        /// </summary>
        public static bool IsCategoryValidForDisplay(string name)
        {
            return Text.IsRequired(name);
        }

        /// <summary>
        /// Validación completa para artículo con mensaje específico
        /// </summary>
        public static (bool IsValid, string? ErrorMessage) ValidateArticleForDisplay(
            string sku, string name, string brand)
        {
            if (IsArticleValidForDisplay(sku, name, brand))
                return (true, null);

            var missingFields = new List<string>();
            if (!Text.IsRequired(sku)) missingFields.Add("SKU");
            if (!Text.IsRequired(name)) missingFields.Add("Nombre");
            if (!Text.IsRequired(brand)) missingFields.Add("Marca");

            return (false, $"Campos requeridos para mostrar: {string.Join(", ", missingFields)}");
        }

        /// <summary>
        /// Validación completa para categoría con mensaje específico
        /// </summary>
        public static (bool IsValid, string? ErrorMessage) ValidateCategoryForDisplay(string name)
        {
            if (IsCategoryValidForDisplay(name))
                return (true, null);

            return (false, "Nombre es requerido para mostrar la categoría");
        }
    }

    /// <summary>
    /// Validaciones para mappers y transformaciones
    /// </summary>
    public static class Mapping
    {
        /// <summary>
        /// Valida datos básicos para crear categoría
        /// </summary>
        public static (bool IsValid, List<string> Errors) ValidateCreateCategory(string name)
        {
            var errors = new List<string>();

            var nameValidation = Text.ValidateRequired(name, "Nombre");
            if (!nameValidation.IsValid)
                errors.Add(nameValidation.ErrorMessage!);

            return (!errors.Any(), errors);
        }

        /// <summary>
        /// Valida datos para actualizar categoría
        /// </summary>
        public static (bool IsValid, List<string> Errors) ValidateUpdateCategory(
            int id, string name, int? parentCategoryId = null)
        {
            var errors = new List<string>();

            var idValidation = Id.ValidateId(id, "ID de categoría");
            if (!idValidation.IsValid)
                errors.Add(idValidation.ErrorMessage!);

            var nameValidation = Text.ValidateRequired(name, "Nombre");
            if (!nameValidation.IsValid)
                errors.Add(nameValidation.ErrorMessage!);

            // Validación de referencia circular se debe hacer en el contexto específico
            // donde se tiene acceso a la jerarquía completa

            return (!errors.Any(), errors);
        }

        /// <summary>
        /// Valida datos para crear artículo
        /// </summary>
        public static (bool IsValid, List<string> Errors) ValidateCreateArticle(
            string sku, string name, string brand, string type)
        {
            var errors = new List<string>();

            var skuValidation = Text.ValidateRequired(sku, "SKU");
            if (!skuValidation.IsValid)
                errors.Add(skuValidation.ErrorMessage!);

            var nameValidation = Text.ValidateRequired(name, "Nombre");
            if (!nameValidation.IsValid)
                errors.Add(nameValidation.ErrorMessage!);

            var brandValidation = Text.ValidateRequired(brand, "Marca");
            if (!brandValidation.IsValid)
                errors.Add(brandValidation.ErrorMessage!);

            var typeValidation = ArticleType.ValidateType(type);
            if (!typeValidation.IsValid)
                errors.Add(typeValidation.ErrorMessage!);

            return (!errors.Any(), errors);
        }

        /// <summary>
        /// Valida datos para actualizar artículo
        /// </summary>
        public static (bool IsValid, List<string> Errors) ValidateUpdateArticle(
            int id, string sku, string name, string brand, string type)
        {
            var errors = new List<string>();

            var idValidation = Id.ValidateId(id, "ID de artículo");
            if (!idValidation.IsValid)
                errors.Add(idValidation.ErrorMessage!);

            // Reutilizar validaciones de creación
            var createValidation = ValidateCreateArticle(sku, name, brand, type);
            errors.AddRange(createValidation.Errors);

            return (!errors.Any(), errors);
        }
    }

    /// <summary>
    /// Validaciones para tipos de artículo
    /// </summary>
    public static class ArticleType
    {
        // Lista de tipos válidos sin depender de Domain
        private static readonly string[] ValidTypes =
        {
            "Simple", "Variable", "Grouped", "External", "Virtual"
        };

        /// <summary>
        /// Valida si un string representa un tipo de artículo válido
        /// </summary>
        public static bool IsValidType(string type)
        {
            if (string.IsNullOrWhiteSpace(type))
                return false;

            return ValidTypes.Contains(type, StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Valida tipo de artículo con mensaje específico
        /// </summary>
        public static (bool IsValid, string? ErrorMessage) ValidateType(string type)
        {
            if (IsValidType(type))
                return (true, null);

            return (false, "Tipo de artículo inválido");
        }
    }

    /// <summary>
    /// Validaciones para tipos de atributos
    /// </summary>
    public static class AttributeType
    {
        /// <summary>
        /// Valida si un valor es compatible con un tipo de atributo específico
        /// </summary>
        public static bool IsValidValue(string attributeType, object? value)
        {
            if (value == null)
                return true;

            var stringValue = value.ToString();
            if (string.IsNullOrEmpty(stringValue))
                return true;

            return attributeType?.ToLowerInvariant() switch
            {
                "text" => true,
                "number" => decimal.TryParse(stringValue, out _),
                "integer" => int.TryParse(stringValue, out _),
                "boolean" => bool.TryParse(stringValue, out _),
                "date" => DateTime.TryParse(stringValue, out _),
                "datetime" => DateTime.TryParse(stringValue, out _),
                "select" => true, // Validación específica en el contexto
                "multiselect" => true, // Validación específica en el contexto
                "color" => stringValue.StartsWith("#") && stringValue.Length == 7,
                "url" => Uri.TryCreate(stringValue, UriKind.Absolute, out _),
                "email" => stringValue.Contains("@") && stringValue.Contains("."),
                _ => true
            };
        }

        /// <summary>
        /// Valida valor de atributo con mensaje específico
        /// </summary>
        public static (bool IsValid, string? ErrorMessage) ValidateValue(
            string attributeType, object? value, string fieldName)
        {
            if (IsValidValue(attributeType, value))
                return (true, null);

            return (false, $"Valor inválido para {fieldName} de tipo {attributeType}");
        }
    }
}
