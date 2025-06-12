using PimFlow.Domain.Article.Enums;
using PimFlow.Shared.DTOs;
using PimFlow.Shared.DTOs.Pagination;

namespace PimFlow.Server.Services;

/// <summary>
/// Facade service that coordinates Article operations using CQRS pattern
/// Follows Single Responsibility Principle by delegating to specialized services
/// Follows Interface Segregation Principle by implementing segregated interfaces
/// Maintains backward compatibility while improving architecture
/// </summary>
public class ArticleService : IArticleService, IArticleReader, IArticleFilter, IArticleWriter
{
    private readonly IArticleQueryService _queryService;
    private readonly IArticleCommandService _commandService;

    public ArticleService(
        IArticleQueryService queryService,
        IArticleCommandService commandService)
    {
        _queryService = queryService;
        _commandService = commandService;
    }

    // Query operations - delegated to specialized query service
    public async Task<IEnumerable<ArticleDto>> GetAllArticlesAsync()
        => await _queryService.GetAllArticlesAsync();

    public async Task<PagedResponse<ArticleDto>> GetArticlesPagedAsync(PagedRequest request)
        => await _queryService.GetArticlesPagedAsync(request);

    public async Task<ArticleDto?> GetArticleByIdAsync(int id)
        => await _queryService.GetArticleByIdAsync(id);

    public async Task<ArticleDto?> GetArticleBySKUAsync(string sku)
        => await _queryService.GetArticleBySKUAsync(sku);

    public async Task<IEnumerable<ArticleDto>> GetArticlesByCategoryIdAsync(int categoryId)
        => await _queryService.GetArticlesByCategoryIdAsync(categoryId);

    public async Task<IEnumerable<ArticleDto>> GetArticlesByTypeAsync(ArticleType type)
        => await _queryService.GetArticlesByTypeAsync(type);

    public async Task<IEnumerable<ArticleDto>> GetArticlesByBrandAsync(string brand)
        => await _queryService.GetArticlesByBrandAsync(brand);

    public async Task<IEnumerable<ArticleDto>> GetArticlesByAttributeAsync(string attributeName, string value)
        => await _queryService.GetArticlesByAttributeAsync(attributeName, value);

    public async Task<IEnumerable<ArticleDto>> SearchArticlesAsync(string searchTerm)
        => await _queryService.SearchArticlesAsync(searchTerm);

    // Command operations - delegated to specialized command service
    public async Task<ArticleDto> CreateArticleAsync(CreateArticleDto createArticleDto)
        => await _commandService.CreateArticleAsync(createArticleDto);

    public async Task<ArticleDto?> UpdateArticleAsync(int id, UpdateArticleDto updateArticleDto)
        => await _commandService.UpdateArticleAsync(id, updateArticleDto);

    public async Task<bool> DeleteArticleAsync(int id)
        => await _commandService.DeleteArticleAsync(id);


}
