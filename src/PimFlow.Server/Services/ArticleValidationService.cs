using PimFlow.Domain.Interfaces;
using PimFlow.Shared.DTOs;
using PimFlow.Server.Validation;
using PimFlow.Server.Validation.Article;

namespace PimFlow.Server.Services;

/// <summary>
/// Implementation of Article validation logic using Strategy Pattern
/// Follows Single Responsibility Principle - only handles validation coordination
/// Follows Open/Closed Principle - extensible through strategy pattern (NEW STRATEGIES CAN BE ADDED)
/// </summary>
public class ArticleValidationService : IArticleValidationService
{
    private readonly IValidationPipeline<CreateArticleDto> _createPipeline;
    private readonly IValidationPipeline<(int Id, UpdateArticleDto Dto)> _updatePipeline;

    public ArticleValidationService(
        IValidationPipeline<CreateArticleDto> createPipeline,
        IValidationPipeline<(int Id, UpdateArticleDto Dto)> updatePipeline)
    {
        _createPipeline = createPipeline;
        _updatePipeline = updatePipeline;
    }

    public async Task<ValidationResult> ValidateCreateAsync(CreateArticleDto createArticleDto)
    {
        // Delegate to validation pipeline - EXTENSIBLE without modifying this code
        return await _createPipeline.ValidateAsync(createArticleDto);
    }

    public async Task<ValidationResult> ValidateUpdateAsync(int id, UpdateArticleDto updateArticleDto)
    {
        // Delegate to validation pipeline - EXTENSIBLE without modifying this code
        return await _updatePipeline.ValidateAsync((id, updateArticleDto));
    }

    public async Task<bool> IsSkuUniqueAsync(string sku, int? excludeId = null)
    {
        // This method is kept for backward compatibility
        // In the future, this logic should be moved to a specific validation strategy
        var createDto = new CreateArticleDto { SKU = sku };
        var result = await ValidateCreateAsync(createDto);

        // Check if SKU uniqueness error exists
        return !result.Errors.Any(e => e.Contains("Ya existe un artículo con SKU"));
    }
}
