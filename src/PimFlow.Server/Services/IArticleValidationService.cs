using PimFlow.Shared.DTOs;
using PimFlow.Server.Validation;

namespace PimFlow.Server.Services;

/// <summary>
/// Service interface for Article validation logic
/// Follows Single Responsibility Principle by handling only validation concerns
/// Follows Open/Closed Principle by allowing extension through strategy pattern
/// </summary>
public interface IArticleValidationService
{
    /// <summary>
    /// Validate article creation data
    /// </summary>
    Task<ValidationResult> ValidateCreateAsync(CreateArticleDto createArticleDto);

    /// <summary>
    /// Validate article update data
    /// </summary>
    Task<ValidationResult> ValidateUpdateAsync(int id, UpdateArticleDto updateArticleDto);

    /// <summary>
    /// Validate SKU uniqueness
    /// </summary>
    Task<bool> IsSkuUniqueAsync(string sku, int? excludeId = null);
}
