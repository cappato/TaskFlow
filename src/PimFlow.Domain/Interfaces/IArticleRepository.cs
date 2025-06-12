using PimFlow.Domain.Article.Enums;
using ArticleEntity = PimFlow.Domain.Article.Article;

namespace PimFlow.Domain.Interfaces;

public interface IArticleRepository
{
    Task<IEnumerable<ArticleEntity>> GetAllAsync();
    Task<ArticleEntity?> GetByIdAsync(int id);
    Task<ArticleEntity?> GetBySKUAsync(string sku);
    Task<IEnumerable<ArticleEntity>> GetByCategoryIdAsync(int categoryId);
    Task<IEnumerable<ArticleEntity>> GetByTypeAsync(ArticleType type);
    Task<IEnumerable<ArticleEntity>> GetByBrandAsync(string brand);
    Task<IEnumerable<ArticleEntity>> GetByAttributeAsync(string attributeName, string value);
    Task<IEnumerable<ArticleEntity>> SearchAsync(string searchTerm);
    Task<ArticleEntity> CreateAsync(ArticleEntity article);
    Task<ArticleEntity?> UpdateAsync(ArticleEntity article);
    Task<bool> DeleteAsync(int id);
    Task<bool> ExistsBySKUAsync(string sku);
}
