using PimFlow.Shared.DTOs;

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

        // Required field validations
        if (string.IsNullOrWhiteSpace(item.SKU))
            errors.Add("SKU es requerido");

        if (string.IsNullOrWhiteSpace(item.Name))
            errors.Add("Nombre es requerido");

        // Format validations
        if (!string.IsNullOrWhiteSpace(item.SKU))
        {
            if (item.SKU.Length < 3)
                errors.Add("SKU debe tener al menos 3 caracteres");
                
            if (item.SKU.Length > 50)
                errors.Add("SKU no puede exceder 50 caracteres");
                
            if (!IsValidSKUFormat(item.SKU))
                errors.Add("SKU contiene caracteres no válidos");
        }

        if (!string.IsNullOrWhiteSpace(item.Name) && item.Name.Length > 200)
            errors.Add("Nombre no puede exceder 200 caracteres");

        if (!string.IsNullOrWhiteSpace(item.Description) && item.Description.Length > 1000)
            errors.Add("Descripción no puede exceder 1000 caracteres");

        if (!string.IsNullOrWhiteSpace(item.Brand) && item.Brand.Length > 100)
            errors.Add("Marca no puede exceder 100 caracteres");

        var result = errors.Any() 
            ? ValidationResult.Failure(errors.ToArray()) 
            : ValidationResult.Success();

        return Task.FromResult(result);
    }

    private static bool IsValidSKUFormat(string sku)
    {
        // SKU should contain only alphanumeric characters, hyphens, and underscores
        return sku.All(c => char.IsLetterOrDigit(c) || c == '-' || c == '_');
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
