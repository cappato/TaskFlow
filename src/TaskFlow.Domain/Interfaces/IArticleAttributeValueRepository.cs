using TaskFlow.Domain.Entities;

namespace TaskFlow.Domain.Interfaces;

public interface IArticleAttributeValueRepository
{
    Task<IEnumerable<ArticleAttributeValue>> GetByArticleIdAsync(int articleId);
    Task<ArticleAttributeValue?> GetByArticleAndAttributeAsync(int articleId, int customAttributeId);
    Task<ArticleAttributeValue> CreateAsync(ArticleAttributeValue attributeValue);
    Task<ArticleAttributeValue?> UpdateAsync(ArticleAttributeValue attributeValue);
    Task<bool> DeleteAsync(int id);
    Task<bool> DeleteByArticleIdAsync(int articleId);
    Task SaveAttributesForArticleAsync(int articleId, Dictionary<string, object> attributes);
}
