using AutoMapper;
using PimFlow.Domain.Entities;
using PimFlow.Domain.Interfaces;
using PimFlow.Domain.Enums;
using PimFlow.Shared.DTOs;
using PimFlow.Shared.Mappers;

namespace PimFlow.Server.Services;

public class ArticleService : IArticleService
{
    private readonly IArticleRepository _articleRepository;
    private readonly ICustomAttributeRepository _customAttributeRepository;
    private readonly IArticleAttributeValueRepository _attributeValueRepository;
    private readonly IMapper _mapper;

    public ArticleService(
        IArticleRepository articleRepository,
        ICustomAttributeRepository customAttributeRepository,
        IArticleAttributeValueRepository attributeValueRepository,
        IMapper mapper)
    {
        _articleRepository = articleRepository;
        _customAttributeRepository = customAttributeRepository;
        _attributeValueRepository = attributeValueRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ArticleDto>> GetAllArticlesAsync()
    {
        var articles = await _articleRepository.GetAllAsync();
        return _mapper.Map<IEnumerable<ArticleDto>>(articles);
    }

    public async Task<ArticleDto?> GetArticleByIdAsync(int id)
    {
        var article = await _articleRepository.GetByIdAsync(id);
        return article != null ? _mapper.Map<ArticleDto>(article) : null;
    }

    public async Task<ArticleDto?> GetArticleBySKUAsync(string sku)
    {
        var article = await _articleRepository.GetBySKUAsync(sku);
        return article != null ? _mapper.Map<ArticleDto>(article) : null;
    }

    public async Task<IEnumerable<ArticleDto>> GetArticlesByCategoryIdAsync(int categoryId)
    {
        var articles = await _articleRepository.GetByCategoryIdAsync(categoryId);
        return _mapper.Map<IEnumerable<ArticleDto>>(articles);
    }

    public async Task<IEnumerable<ArticleDto>> GetArticlesByTypeAsync(ArticleType type)
    {
        var articles = await _articleRepository.GetByTypeAsync(type);
        return _mapper.Map<IEnumerable<ArticleDto>>(articles);
    }

    public async Task<IEnumerable<ArticleDto>> GetArticlesByBrandAsync(string brand)
    {
        var articles = await _articleRepository.GetByBrandAsync(brand);
        return _mapper.Map<IEnumerable<ArticleDto>>(articles);
    }

    public async Task<IEnumerable<ArticleDto>> GetArticlesByAttributeAsync(string attributeName, string value)
    {
        var articles = await _articleRepository.GetByAttributeAsync(attributeName, value);
        return _mapper.Map<IEnumerable<ArticleDto>>(articles);
    }

    public async Task<IEnumerable<ArticleDto>> SearchArticlesAsync(string searchTerm)
    {
        var articles = await _articleRepository.SearchAsync(searchTerm);
        return _mapper.Map<IEnumerable<ArticleDto>>(articles);
    }

    public async Task<ArticleDto> CreateArticleAsync(CreateArticleDto createArticleDto)
    {
        // Validate SKU uniqueness
        if (await _articleRepository.ExistsBySKUAsync(createArticleDto.SKU))
        {
            throw new InvalidOperationException($"Ya existe un artículo con SKU: {createArticleDto.SKU}");
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

        // Update basic properties
        if (!string.IsNullOrEmpty(updateArticleDto.SKU))
        {
            if (updateArticleDto.SKU != existingArticle.SKU && await _articleRepository.ExistsBySKUAsync(updateArticleDto.SKU))
            {
                throw new InvalidOperationException($"Ya existe un artículo con SKU: {updateArticleDto.SKU}");
            }
            existingArticle.SKU = updateArticleDto.SKU;
        }

        if (!string.IsNullOrEmpty(updateArticleDto.Name))
            existingArticle.Name = updateArticleDto.Name;

        if (!string.IsNullOrEmpty(updateArticleDto.Description))
            existingArticle.Description = updateArticleDto.Description;

        if (updateArticleDto.Type.HasValue)
            existingArticle.Type = EnumMapper.ToDomain(updateArticleDto.Type.Value);

        if (!string.IsNullOrEmpty(updateArticleDto.Brand))
            existingArticle.Brand = updateArticleDto.Brand;

        if (updateArticleDto.CategoryId.HasValue)
            existingArticle.CategoryId = updateArticleDto.CategoryId.Value;

        if (updateArticleDto.SupplierId.HasValue)
            existingArticle.SupplierId = updateArticleDto.SupplierId.Value;

        if (updateArticleDto.IsActive.HasValue)
            existingArticle.IsActive = updateArticleDto.IsActive.Value;

        existingArticle.UpdatedAt = DateTime.UtcNow;

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

    private async Task SaveCustomAttributesAsync(int articleId, Dictionary<string, object> customAttributes)
    {
        await _attributeValueRepository.SaveAttributesForArticleAsync(articleId, customAttributes);
    }


}
