using PimFlow.Shared.DTOs;
using PimFlow.Shared.DTOs.Pagination;
using PimFlow.Domain.Article.Enums;
using PimFlow.Domain.CustomAttribute.Enums;


namespace PimFlow.Server.Services;

/// <summary>
/// INTERFACE SEGREGATION PRINCIPLE (ISP) - Interfaces segregadas por responsabilidad
/// </summary>

/// <summary>
/// Interface for Article read operations only
/// Follows ISP - clients that only need to read articles don't depend on write operations
/// </summary>
public interface IArticleReader
{
    Task<IEnumerable<ArticleDto>> GetAllArticlesAsync();
    Task<PagedResponse<ArticleDto>> GetArticlesPagedAsync(PagedRequest request);
    Task<ArticleDto?> GetArticleByIdAsync(int id);
    Task<ArticleDto?> GetArticleBySKUAsync(string sku);
    Task<IEnumerable<ArticleDto>> SearchArticlesAsync(string searchTerm);
}

/// <summary>
/// Interface for Article filtering operations
/// Follows ISP - specialized interface for filtering needs
/// </summary>
public interface IArticleFilter
{
    Task<IEnumerable<ArticleDto>> GetArticlesByCategoryIdAsync(int categoryId);
    Task<IEnumerable<ArticleDto>> GetArticlesByTypeAsync(ArticleType type);
    Task<IEnumerable<ArticleDto>> GetArticlesByBrandAsync(string brand);
    Task<IEnumerable<ArticleDto>> GetArticlesByAttributeAsync(string attributeName, string value);
}

/// <summary>
/// Interface for Article write operations only
/// Follows ISP - clients that only need to write articles don't depend on read operations
/// </summary>
public interface IArticleWriter
{
    Task<ArticleDto> CreateArticleAsync(CreateArticleDto createArticleDto);
    Task<ArticleDto?> UpdateArticleAsync(int id, UpdateArticleDto updateArticleDto);
    Task<bool> DeleteArticleAsync(int id);
}

/// <summary>
/// Facade interface that combines all Article operations for backward compatibility
/// Follows ISP by inheriting from segregated interfaces instead of defining everything
/// </summary>
public interface IArticleService : IArticleReader, IArticleFilter, IArticleWriter
{
    // This interface is now composed of segregated interfaces
    // Clients can depend on specific interfaces (IArticleReader, IArticleWriter, etc.)
    // or on the full interface for backward compatibility
}
