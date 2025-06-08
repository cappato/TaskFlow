using Microsoft.EntityFrameworkCore;
using TaskFlow.Server.Data;
using TaskFlow.Domain.Entities;
using TaskFlow.Domain.Interfaces;

namespace TaskFlow.Server.Repositories;

public class ArticleAttributeValueRepository : IArticleAttributeValueRepository
{
    private readonly TaskFlowDbContext _context;

    public ArticleAttributeValueRepository(TaskFlowDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ArticleAttributeValue>> GetByArticleIdAsync(int articleId)
    {
        return await _context.ArticleAttributeValues
            .Include(av => av.CustomAttribute)
            .Where(av => av.ArticleId == articleId)
            .ToListAsync();
    }

    public async Task<ArticleAttributeValue?> GetByArticleAndAttributeAsync(int articleId, int customAttributeId)
    {
        return await _context.ArticleAttributeValues
            .Include(av => av.CustomAttribute)
            .FirstOrDefaultAsync(av => av.ArticleId == articleId && av.CustomAttributeId == customAttributeId);
    }

    public async Task<ArticleAttributeValue> CreateAsync(ArticleAttributeValue attributeValue)
    {
        attributeValue.CreatedAt = DateTime.UtcNow;
        _context.ArticleAttributeValues.Add(attributeValue);
        await _context.SaveChangesAsync();
        return attributeValue;
    }

    public async Task<ArticleAttributeValue?> UpdateAsync(ArticleAttributeValue attributeValue)
    {
        var existing = await _context.ArticleAttributeValues.FindAsync(attributeValue.Id);
        if (existing == null)
            return null;

        attributeValue.UpdatedAt = DateTime.UtcNow;
        _context.Entry(existing).CurrentValues.SetValues(attributeValue);
        await _context.SaveChangesAsync();
        return existing;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var attributeValue = await _context.ArticleAttributeValues.FindAsync(id);
        if (attributeValue == null)
            return false;

        _context.ArticleAttributeValues.Remove(attributeValue);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteByArticleIdAsync(int articleId)
    {
        var attributeValues = await _context.ArticleAttributeValues
            .Where(av => av.ArticleId == articleId)
            .ToListAsync();

        if (attributeValues.Any())
        {
            _context.ArticleAttributeValues.RemoveRange(attributeValues);
            await _context.SaveChangesAsync();
        }

        return true;
    }

    public async Task SaveAttributesForArticleAsync(int articleId, Dictionary<string, object> attributes)
    {
        // 1. Remove existing attribute values for this article
        await DeleteByArticleIdAsync(articleId);

        // 2. Get all custom attributes by name
        var attributeNames = attributes.Keys.ToList();
        var customAttributes = await _context.CustomAttributes
            .Where(ca => attributeNames.Contains(ca.Name))
            .ToListAsync();

        // 3. Create new attribute values
        var newAttributeValues = new List<ArticleAttributeValue>();

        foreach (var kvp in attributes)
        {
            var customAttribute = customAttributes.FirstOrDefault(ca => ca.Name == kvp.Key);
            if (customAttribute != null)
            {
                var attributeValue = new ArticleAttributeValue
                {
                    ArticleId = articleId,
                    CustomAttributeId = customAttribute.Id,
                    Value = kvp.Value?.ToString() ?? string.Empty,
                    CreatedAt = DateTime.UtcNow
                };
                newAttributeValues.Add(attributeValue);
            }
        }

        if (newAttributeValues.Any())
        {
            _context.ArticleAttributeValues.AddRange(newAttributeValues);
            await _context.SaveChangesAsync();
        }
    }
}
