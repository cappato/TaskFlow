using PimFlow.Domain.Article.ValueObjects;
using PimFlow.Domain.Common;

namespace PimFlow.Domain.Category;

/// <summary>
/// Validador específico para el agregado Category
/// Centraliza todas las validaciones relacionadas con categorías
/// </summary>
public static class CategoryValidator
{
    /// <summary>
    /// Validaciones para nombre de categoría
    /// </summary>
    public static class Name
    {
        public static bool IsValid(string value) => ProductName.IsValid(value);

        public static string GetValidationMessage() =>
            "El nombre de la categoría debe tener entre 2 y 200 caracteres";

        public static Result Validate(string value)
        {
            if (IsValid(value))
                return Result.Success();

            return Result.Failure(GetValidationMessage());
        }
    }

    /// <summary>
    /// Validaciones para descripción de categoría
    /// </summary>
    public static class Description
    {
        public static bool IsValid(string value)
        {
            if (string.IsNullOrEmpty(value))
                return true; // Descripción es opcional

            var trimmed = value.Trim();
            return trimmed.Length <= 500;
        }

        public static string GetValidationMessage() =>
            "La descripción no puede exceder 500 caracteres";

        public static Result Validate(string value)
        {
            if (IsValid(value))
                return Result.Success();

            return Result.Failure(GetValidationMessage());
        }
    }

    /// <summary>
    /// Validaciones para jerarquía de categorías
    /// </summary>
    public static class Hierarchy
    {
        public static bool IsValidParent(int? parentId, int currentId)
        {
            // No puede ser padre de sí misma
            return parentId != currentId;
        }

        public static string GetValidationMessage() =>
            "Una categoría no puede ser padre de sí misma";

        public static Result ValidateParent(int? parentId, int currentId)
        {
            if (IsValidParent(parentId, currentId))
                return Result.Success();

            return Result.Failure(GetValidationMessage());
        }
    }

    /// <summary>
    /// Validación completa de una categoría
    /// </summary>
    public static Result ValidateForCreation(string name, string description = "", int? parentId = null)
    {
        var nameResult = Name.Validate(name);
        if (nameResult.IsFailure)
            return nameResult;

        var descriptionResult = Description.Validate(description);
        if (descriptionResult.IsFailure)
            return descriptionResult;

        return Result.Success();
    }

    /// <summary>
    /// Validación para actualización de categoría
    /// </summary>
    public static Result ValidateForUpdate(int categoryId, string name, string description = "", int? parentId = null)
    {
        var nameResult = Name.Validate(name);
        if (nameResult.IsFailure)
            return nameResult;

        var descriptionResult = Description.Validate(description);
        if (descriptionResult.IsFailure)
            return descriptionResult;

        var hierarchyResult = Hierarchy.ValidateParent(parentId, categoryId);
        if (hierarchyResult.IsFailure)
            return hierarchyResult;

        return Result.Success();
    }
}
