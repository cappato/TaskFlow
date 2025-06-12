using PimFlow.Domain.Article.ValueObjects;
using PimFlow.Domain.Common;

namespace PimFlow.Domain.Article;

/// <summary>
/// Validador específico para el agregado Article
/// Centraliza todas las validaciones relacionadas con artículos
/// </summary>
public static class ArticleValidator
{
    /// <summary>
    /// Validaciones para SKU
    /// </summary>
    public static class Sku
    {
        public static bool IsValid(string value) => SKU.IsValid(value);

        public static string GetValidationMessage() =>
            "SKU debe contener solo letras mayúsculas y números, entre 3 y 50 caracteres";

        public static Result Validate(string value)
        {
            if (IsValid(value))
                return Result.Success();

            return Result.Failure(GetValidationMessage());
        }
    }

    /// <summary>
    /// Validaciones para ProductName
    /// </summary>
    public static class Name
    {
        public static bool IsValid(string value) => ProductName.IsValid(value);

        public static string GetValidationMessage() =>
            "El nombre debe tener entre 2 y 200 caracteres";

        public static Result Validate(string value)
        {
            if (IsValid(value))
                return Result.Success();

            return Result.Failure(GetValidationMessage());
        }
    }

    /// <summary>
    /// Validaciones para Brand
    /// </summary>
    public static class Brand
    {
        public static bool IsValid(string value) => ValueObjects.Brand.IsValid(value);

        public static string GetValidationMessage() =>
            "La marca debe tener entre 2 y 100 caracteres alfanuméricos";

        public static Result Validate(string value)
        {
            if (IsValid(value))
                return Result.Success();

            return Result.Failure(GetValidationMessage());
        }
    }

    /// <summary>
    /// Validación completa de un artículo
    /// </summary>
    public static Result ValidateForCreation(string sku, string name, string brand)
    {
        var skuResult = Sku.Validate(sku);
        if (skuResult.IsFailure)
            return skuResult;

        var nameResult = Name.Validate(name);
        if (nameResult.IsFailure)
            return nameResult;

        var brandResult = Brand.Validate(brand);
        if (brandResult.IsFailure)
            return brandResult;

        return Result.Success();
    }
}
