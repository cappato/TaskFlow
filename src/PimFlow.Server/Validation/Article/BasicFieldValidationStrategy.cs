using PimFlow.Shared.DTOs;
using PimFlow.Contracts.Validation;

namespace PimFlow.Server.Validation.Article;

/// <summary>
/// Validates basic field requirements for Article creation
/// Follows Open/Closed Principle - can be extended without modification
/// </summary>
public class BasicFieldValidationStrategy : IArticleCreateValidationStrategy
{
    public int Priority => 1; // Highest priority - basic validation first
    public string Name => "Basic Field Validation";
    public ValidationCategory Category => ValidationCategory.Basic;

    public Task<ValidationResult> ValidateAsync(CreateArticleDto item)
    {
        var errors = new List<string>();

        // Required field validations using centralized validators
        var skuValidation = SharedValidationRules.Text.ValidateRequired(item.SKU, "SKU");
        if (!skuValidation.IsValid)
            errors.Add(skuValidation.ErrorMessage!);

        var nameValidation = SharedValidationRules.Text.ValidateRequired(item.Name, "Nombre");
        if (!nameValidation.IsValid)
            errors.Add(nameValidation.ErrorMessage!);

        // Format validations using centralized validators
        if (!string.IsNullOrWhiteSpace(item.SKU))
        {
            var skuFormatValidation = SharedValidationRules.Sku.Validate(item.SKU);
            if (!skuFormatValidation.IsValid)
                errors.Add(skuFormatValidation.ErrorMessage!);
        }

        if (!string.IsNullOrWhiteSpace(item.Name))
        {
            var nameFormatValidation = SharedValidationRules.ProductName.Validate(item.Name);
            if (!nameFormatValidation.IsValid)
                errors.Add(nameFormatValidation.ErrorMessage!);
        }

        if (!string.IsNullOrWhiteSpace(item.Description))
        {
            var descValidation = SharedValidationRules.Text.ValidateLength(item.Description, 0, 1000, "Descripción");
            if (!descValidation.IsValid)
                errors.Add(descValidation.ErrorMessage!);
        }

        if (!string.IsNullOrWhiteSpace(item.Brand))
        {
            var brandValidation = SharedValidationRules.Brand.Validate(item.Brand);
            if (!brandValidation.IsValid)
                errors.Add(brandValidation.ErrorMessage!);
        }

        var result = errors.Any() 
            ? ValidationResult.Failure(errors.ToArray()) 
            : ValidationResult.Success();

        return Task.FromResult(result);
    }


}

/// <summary>
/// Validates basic field requirements for Article updates
/// </summary>
public class BasicFieldUpdateValidationStrategy : IArticleUpdateValidationStrategy
{
    public int Priority => 1;
    public string Name => "Basic Field Update Validation";
    public ValidationCategory Category => ValidationCategory.Basic;

    public Task<ValidationResult> ValidateAsync((int Id, UpdateArticleDto Dto) item)
    {
        var errors = new List<string>();
        var dto = item.Dto;

        // ID validation
        if (item.Id <= 0)
            errors.Add("ID del artículo debe ser válido");

        // Optional field validations (only validate if provided)
        if (!string.IsNullOrWhiteSpace(dto.SKU))
        {
            if (dto.SKU.Length < 3)
                errors.Add("SKU debe tener al menos 3 caracteres");
                
            if (dto.SKU.Length > 50)
                errors.Add("SKU no puede exceder 50 caracteres");
        }

        if (!string.IsNullOrWhiteSpace(dto.Name) && dto.Name.Length > 200)
            errors.Add("Nombre no puede exceder 200 caracteres");

        if (!string.IsNullOrWhiteSpace(dto.Description) && dto.Description.Length > 1000)
            errors.Add("Descripción no puede exceder 1000 caracteres");

        if (!string.IsNullOrWhiteSpace(dto.Brand) && dto.Brand.Length > 100)
            errors.Add("Marca no puede exceder 100 caracteres");

        var result = errors.Any() 
            ? ValidationResult.Failure(errors.ToArray()) 
            : ValidationResult.Success();

        return Task.FromResult(result);
    }
}
