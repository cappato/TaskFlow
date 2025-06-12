using AutoMapper;
using PimFlow.Domain.Interfaces;
using PimFlow.Domain.Article.Enums;
using PimFlow.Shared.DTOs;
using PimFlow.Shared.DTOs.Pagination;

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

    public async Task<PagedResponse<ArticleDto>> GetArticlesPagedAsync(PagedRequest request)
    {
        var articles = await _articleRepository.GetAllAsync();

        // Aplicar filtro de búsqueda si existe
        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var searchTerm = request.SearchTerm.ToLower();
            articles = articles.Where(a =>
                a.Name.ToLower().Contains(searchTerm) ||
                a.SKU.ToLower().Contains(searchTerm) ||
                a.Brand.ToLower().Contains(searchTerm) ||
                (a.Description?.ToLower().Contains(searchTerm) ?? false));
        }

        // Aplicar ordenamiento
        if (!string.IsNullOrWhiteSpace(request.SortBy))
        {
            articles = request.SortBy.ToLower() switch
            {
                "name" => request.IsDescending ? articles.OrderByDescending(a => a.Name) : articles.OrderBy(a => a.Name),
                "sku" => request.IsDescending ? articles.OrderByDescending(a => a.SKU) : articles.OrderBy(a => a.SKU),
                "brand" => request.IsDescending ? articles.OrderByDescending(a => a.Brand) : articles.OrderBy(a => a.Brand),
                "createdat" => request.IsDescending ? articles.OrderByDescending(a => a.CreatedAt) : articles.OrderBy(a => a.CreatedAt),
                _ => articles.OrderBy(a => a.Name)
            };
        }
        else
        {
            articles = articles.OrderBy(a => a.Name);
        }

        var totalCount = articles.Count();
        var pagedArticles = articles
            .Skip(request.Skip)
            .Take(request.PageSize)
            .ToList();

        var articleDtos = _mapper.Map<IEnumerable<ArticleDto>>(pagedArticles);

        return new PagedResponse<ArticleDto>(articleDtos, request.PageNumber, request.PageSize, totalCount);
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
