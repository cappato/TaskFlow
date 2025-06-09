using AutoMapper;
using PimFlow.Domain.Interfaces;
using PimFlow.Domain.Enums;
using PimFlow.Shared.DTOs;

namespace PimFlow.Server.Services;

/// <summary>
/// Implementation of Article queries (CQRS - Query side)
/// Follows Single Responsibility Principle - only handles read operations
/// Follows Dependency Inversion Principle - depends on abstractions
/// </summary>
public class ArticleQueryService : IArticleQueryService
{
    private readonly IArticleRepository _articleRepository;
    private readonly IMapper _mapper;

    public ArticleQueryService(IArticleRepository articleRepository, IMapper mapper)
    {
        _articleRepository = articleRepository;
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
}
