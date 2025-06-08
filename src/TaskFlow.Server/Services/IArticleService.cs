using TaskFlow.Shared.DTOs;
using TaskFlow.Domain.Enums;

namespace TaskFlow.Server.Services;

public interface IArticleService
{
    Task<IEnumerable<ArticleDto>> GetAllArticlesAsync();
    Task<ArticleDto?> GetArticleByIdAsync(int id);
    Task<ArticleDto?> GetArticleBySKUAsync(string sku);
    Task<IEnumerable<ArticleDto>> GetArticlesByCategoryIdAsync(int categoryId);
    Task<IEnumerable<ArticleDto>> GetArticlesByTypeAsync(ArticleType type);
    Task<IEnumerable<ArticleDto>> GetArticlesByBrandAsync(string brand);
    Task<IEnumerable<ArticleDto>> GetArticlesByAttributeAsync(string attributeName, string value);
    Task<IEnumerable<ArticleDto>> SearchArticlesAsync(string searchTerm);
    Task<ArticleDto> CreateArticleAsync(CreateArticleDto createArticleDto);
    Task<ArticleDto?> UpdateArticleAsync(int id, UpdateArticleDto updateArticleDto);
    Task<bool> DeleteArticleAsync(int id);
}
