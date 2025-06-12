using AutoMapper;
using PimFlow.Domain.Article;
using PimFlow.Domain.Interfaces;
using PimFlow.Shared.DTOs;
using PimFlow.Server.Mappers;

namespace PimFlow.Server.Services;

/// <summary>
/// Implementation of Article commands (CQRS - Command side)
/// Follows Single Responsibility Principle - only handles write operations
/// Follows Dependency Inversion Principle - depends on abstractions
/// </summary>
public class ArticleCommandService : IArticleCommandService
{
    private readonly IArticleRepository _articleRepository;
    private readonly IArticleAttributeValueRepository _attributeValueRepository;
    private readonly IArticleValidationService _validationService;
    private readonly IMapper _mapper;

    public ArticleCommandService(
        IArticleRepository articleRepository,
        IArticleAttributeValueRepository attributeValueRepository,
        IArticleValidationService validationService,
        IMapper mapper)
    {
        _articleRepository = articleRepository;
        _attributeValueRepository = attributeValueRepository;
        _validationService = validationService;
        _mapper = mapper;
    }

    public async Task<ArticleDto> CreateArticleAsync(CreateArticleDto createArticleDto)
    {
        // Validate using dedicated validation service
        var validationResult = await _validationService.ValidateCreateAsync(createArticleDto);
        if (!validationResult.IsValid)
        {
            throw new InvalidOperationException(string.Join("; ", validationResult.Errors));
        }

        var article = _mapper.Map<Article>(createArticleDto);
        var createdArticle = await _articleRepository.CreateAsync(article);

        // Handle custom attributes
        if (createArticleDto.CustomAttributes.Any())
        {
            await SaveCustomAttributesAsync(createdArticle.Id, createArticleDto.CustomAttributes);
            // Reload to get attributes
            createdArticle = await _articleRepository.GetByIdAsync(createdArticle.Id) ?? createdArticle;
        }

        return _mapper.Map<ArticleDto>(createdArticle);
    }

    public async Task<ArticleDto?> UpdateArticleAsync(int id, UpdateArticleDto updateArticleDto)
    {
        var existingArticle = await _articleRepository.GetByIdAsync(id);
        if (existingArticle == null)
            return null;

        // Validate using dedicated validation service
        var validationResult = await _validationService.ValidateUpdateAsync(id, updateArticleDto);
        if (!validationResult.IsValid)
        {
            throw new InvalidOperationException(string.Join("; ", validationResult.Errors));
        }

        // Update properties using cleaner approach
        UpdateArticleProperties(existingArticle, updateArticleDto);

        var updatedArticle = await _articleRepository.UpdateAsync(existingArticle);

        // Handle custom attributes update
        if (updateArticleDto.CustomAttributes != null)
        {
            await SaveCustomAttributesAsync(id, updateArticleDto.CustomAttributes);
            // Reload to get updated attributes
            updatedArticle = await _articleRepository.GetByIdAsync(id);
        }

        return updatedArticle != null ? _mapper.Map<ArticleDto>(updatedArticle) : null;
    }

    public async Task<bool> DeleteArticleAsync(int id)
    {
        return await _articleRepository.DeleteAsync(id);
    }

    private static void UpdateArticleProperties(Article existingArticle, UpdateArticleDto updateArticleDto)
    {
        if (!string.IsNullOrEmpty(updateArticleDto.SKU))
            existingArticle.SKU = updateArticleDto.SKU;

        if (!string.IsNullOrEmpty(updateArticleDto.Name))
            existingArticle.Name = updateArticleDto.Name;

        if (!string.IsNullOrEmpty(updateArticleDto.Description))
            existingArticle.Description = updateArticleDto.Description;

        if (updateArticleDto.Type.HasValue)
            existingArticle.Type = DomainEnumMapper.ToDomain(updateArticleDto.Type.Value);

        if (!string.IsNullOrEmpty(updateArticleDto.Brand))
            existingArticle.Brand = updateArticleDto.Brand;

        if (updateArticleDto.CategoryId.HasValue)
            existingArticle.CategoryId = updateArticleDto.CategoryId.Value;

        if (updateArticleDto.SupplierId.HasValue)
            existingArticle.SupplierId = updateArticleDto.SupplierId.Value;

        if (updateArticleDto.IsActive.HasValue)
            existingArticle.IsActive = updateArticleDto.IsActive.Value;

        existingArticle.UpdatedAt = DateTime.UtcNow;
    }

    private async Task SaveCustomAttributesAsync(int articleId, Dictionary<string, object> customAttributes)
    {
        await _attributeValueRepository.SaveAttributesForArticleAsync(articleId, customAttributes);
    }
}
