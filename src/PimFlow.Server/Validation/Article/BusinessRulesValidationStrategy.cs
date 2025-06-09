using PimFlow.Domain.Interfaces;
using PimFlow.Shared.DTOs;

namespace PimFlow.Server.Validation.Article;

/// <summary>
/// Validates business rules for Article creation
/// Follows Open/Closed Principle - new business rules can be added as new strategies
/// </summary>
public class BusinessRulesValidationStrategy : IArticleCreateValidationStrategy
{
    private readonly IArticleRepository _articleRepository;
    private readonly ICategoryRepository _categoryRepository;

    public int Priority => 2; // After basic validation
    public string Name => "Business Rules Validation";
    public ValidationCategory Category => ValidationCategory.BusinessRules;

    public BusinessRulesValidationStrategy(
        IArticleRepository articleRepository,
        ICategoryRepository categoryRepository)
    {
        _articleRepository = articleRepository;
        _categoryRepository = categoryRepository;
    }

    public async Task<ValidationResult> ValidateAsync(CreateArticleDto item)
    {
        var errors = new List<string>();
        var warnings = new List<string>();

        // SKU uniqueness validation
        if (!string.IsNullOrWhiteSpace(item.SKU))
        {
            if (await _articleRepository.ExistsBySKUAsync(item.SKU))
                errors.Add($"Ya existe un artículo con SKU: {item.SKU}");
        }

        // Category validation
        if (item.CategoryId.HasValue && item.CategoryId > 0)
        {
            var category = await _categoryRepository.GetByIdAsync(item.CategoryId.Value);
            if (category == null)
                errors.Add($"La categoría con ID {item.CategoryId} no existe");
            else if (!category.IsActive)
                warnings.Add($"La categoría '{category.Name}' está inactiva");
        }
        else
        {
            errors.Add("Categoría es requerida");
        }

        // Supplier validation (simplified - no repository available yet)
        if (item.SupplierId.HasValue && item.SupplierId <= 0)
        {
            errors.Add("ID del proveedor debe ser válido si se especifica");
        }

        // Business logic validation
        if (item.CustomAttributes?.Count > 20)
            warnings.Add("El artículo tiene muchos atributos personalizados (>20), considere revisar la estructura");

        var result = new ValidationResult
        {
            IsValid = !errors.Any(),
            Errors = errors,
            Warnings = warnings
        };

        return result;
    }
}

/// <summary>
/// Validates business rules for Article updates
/// </summary>
public class BusinessRulesUpdateValidationStrategy : IArticleUpdateValidationStrategy
{
    private readonly IArticleRepository _articleRepository;
    private readonly ICategoryRepository _categoryRepository;

    public int Priority => 2;
    public string Name => "Business Rules Update Validation";
    public ValidationCategory Category => ValidationCategory.BusinessRules;

    public BusinessRulesUpdateValidationStrategy(
        IArticleRepository articleRepository,
        ICategoryRepository categoryRepository)
    {
        _articleRepository = articleRepository;
        _categoryRepository = categoryRepository;
    }

    public async Task<ValidationResult> ValidateAsync((int Id, UpdateArticleDto Dto) item)
    {
        var errors = new List<string>();
        var warnings = new List<string>();
        var dto = item.Dto;

        // Article existence validation
        var existingArticle = await _articleRepository.GetByIdAsync(item.Id);
        if (existingArticle == null)
        {
            errors.Add($"El artículo con ID {item.Id} no existe");
            return ValidationResult.Failure(errors.ToArray());
        }

        // SKU uniqueness validation (if SKU is being changed)
        if (!string.IsNullOrWhiteSpace(dto.SKU) && dto.SKU != existingArticle.SKU)
        {
            if (await _articleRepository.ExistsBySKUAsync(dto.SKU))
                errors.Add($"Ya existe un artículo con SKU: {dto.SKU}");
        }

        // Category validation
        if (dto.CategoryId.HasValue && dto.CategoryId > 0)
        {
            var category = await _categoryRepository.GetByIdAsync(dto.CategoryId.Value);
            if (category == null)
                errors.Add($"La categoría con ID {dto.CategoryId} no existe");
            else if (!category.IsActive)
                warnings.Add($"La categoría '{category.Name}' está inactiva");
        }

        // Supplier validation (simplified - no repository available yet)
        if (dto.SupplierId.HasValue && dto.SupplierId <= 0)
        {
            errors.Add("ID del proveedor debe ser válido si se especifica");
        }

        var result = new ValidationResult
        {
            IsValid = !errors.Any(),
            Errors = errors,
            Warnings = warnings
        };

        return result;
    }
}
