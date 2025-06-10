using PimFlow.Domain.Enums;
using PimFlow.Shared.DTOs;
using PimFlow.Shared.DTOs.Pagination;

namespace PimFlow.Server.Services;

/// <summary>
/// Service interface for Article queries (CQRS - Query side)
/// Follows Single Responsibility Principle by handling only read operations
/// </summary>
public interface IArticleQueryService
{
    /// <summary>
    /// Get all articles with their related data
    /// </summary>
    Task<IEnumerable<ArticleDto>> GetAllArticlesAsync();

    /// <summary>
    /// Get articles with pagination support
    /// </summary>
    Task<PagedResponse<ArticleDto>> GetArticlesPagedAsync(PagedRequest request);

    /// <summary>
    /// Get article by unique identifier
    /// </summary>
    Task<ArticleDto?> GetArticleByIdAsync(int id);

    /// <summary>
    /// Get article by unique SKU
    /// </summary>
    Task<ArticleDto?> GetArticleBySKUAsync(string sku);

    /// <summary>
    /// Get articles filtered by category
    /// </summary>
    Task<IEnumerable<ArticleDto>> GetArticlesByCategoryIdAsync(int categoryId);

    /// <summary>
    /// Get articles filtered by type
    /// </summary>
    Task<IEnumerable<ArticleDto>> GetArticlesByTypeAsync(ArticleType type);

    /// <summary>
    /// Get articles filtered by brand
    /// </summary>
    Task<IEnumerable<ArticleDto>> GetArticlesByBrandAsync(string brand);

    /// <summary>
    /// Get articles filtered by custom attribute value
    /// </summary>
    Task<IEnumerable<ArticleDto>> GetArticlesByAttributeAsync(string attributeName, string value);

    /// <summary>
    /// Search articles by term across multiple fields
    /// </summary>
    Task<IEnumerable<ArticleDto>> SearchArticlesAsync(string searchTerm);
}
