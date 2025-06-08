using TaskFlow.Server.Models;
using TaskFlow.Shared.Enums;

namespace TaskFlow.Server.Repositories;

public interface IArticleRepository
{
    Task<IEnumerable<Article>> GetAllAsync();
    Task<Article?> GetByIdAsync(int id);
    Task<Article?> GetBySKUAsync(string sku);
    Task<IEnumerable<Article>> GetByCategoryIdAsync(int categoryId);
    Task<IEnumerable<Article>> GetByTypeAsync(ArticleType type);
    Task<IEnumerable<Article>> GetByBrandAsync(string brand);
    Task<IEnumerable<Article>> GetByAttributeAsync(string attributeName, string value);
    Task<IEnumerable<Article>> SearchAsync(string searchTerm);
    Task<Article> CreateAsync(Article article);
    Task<Article?> UpdateAsync(Article article);
    Task<bool> DeleteAsync(int id);
    Task<bool> ExistsBySKUAsync(string sku);
}
