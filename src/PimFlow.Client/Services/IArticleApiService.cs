using PimFlow.Shared.DTOs;
using PimFlow.Shared.Enums;

namespace PimFlow.Client.Services;

public interface IArticleApiService
{
    Task<IEnumerable<ArticleDto>> GetAllArticlesAsync();
    Task<ArticleDto?> GetArticleByIdAsync(int id);
    Task<IEnumerable<ArticleDto>> GetArticlesByAttributeAsync(string attributeName, string value);
    Task<ArticleDto> CreateArticleAsync(CreateArticleDto createArticleDto);
    Task<ArticleDto?> UpdateArticleAsync(int id, UpdateArticleDto updateArticleDto);
    Task<bool> DeleteArticleAsync(int id);
}
