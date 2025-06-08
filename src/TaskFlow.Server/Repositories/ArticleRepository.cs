using Microsoft.EntityFrameworkCore;
using TaskFlow.Server.Data;
using TaskFlow.Domain.Entities;
using TaskFlow.Domain.Enums;
using TaskFlow.Domain.Interfaces;

namespace TaskFlow.Server.Repositories;

public class ArticleRepository : IArticleRepository
{
    private readonly TaskFlowDbContext _context;

    public ArticleRepository(TaskFlowDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Article>> GetAllAsync()
    {
        return await _context.Articles
            .Include(a => a.Category)
            .Include(a => a.Supplier)
            .Include(a => a.AttributeValues)
                .ThenInclude(av => av.CustomAttribute)
            .Include(a => a.Variants)
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync();
    }

    public async Task<Article?> GetByIdAsync(int id)
    {
        return await _context.Articles
            .Include(a => a.Category)
            .Include(a => a.Supplier)
            .Include(a => a.AttributeValues)
                .ThenInclude(av => av.CustomAttribute)
            .Include(a => a.Variants)
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<Article?> GetBySKUAsync(string sku)
    {
        return await _context.Articles
            .Include(a => a.Category)
            .Include(a => a.Supplier)
            .Include(a => a.AttributeValues)
                .ThenInclude(av => av.CustomAttribute)
            .Include(a => a.Variants)
            .FirstOrDefaultAsync(a => a.SKU == sku);
    }

    public async Task<IEnumerable<Article>> GetByCategoryIdAsync(int categoryId)
    {
        return await _context.Articles
            .Include(a => a.Category)
            .Include(a => a.Supplier)
            .Include(a => a.AttributeValues)
                .ThenInclude(av => av.CustomAttribute)
            .Where(a => a.CategoryId == categoryId)
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Article>> GetByTypeAsync(ArticleType type)
    {
        return await _context.Articles
            .Include(a => a.Category)
            .Include(a => a.Supplier)
            .Include(a => a.AttributeValues)
                .ThenInclude(av => av.CustomAttribute)
            .Where(a => a.Type == type)
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Article>> GetByBrandAsync(string brand)
    {
        return await _context.Articles
            .Include(a => a.Category)
            .Include(a => a.Supplier)
            .Include(a => a.AttributeValues)
                .ThenInclude(av => av.CustomAttribute)
            .Where(a => a.Brand.ToLower().Contains(brand.ToLower()))
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Article>> GetByAttributeAsync(string attributeName, string value)
    {
        return await _context.Articles
            .Include(a => a.Category)
            .Include(a => a.Supplier)
            .Include(a => a.AttributeValues)
                .ThenInclude(av => av.CustomAttribute)
            .Where(a => a.AttributeValues.Any(av =>
                av.CustomAttribute.Name == attributeName &&
                av.Value.ToLower().Contains(value.ToLower())))
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Article>> SearchAsync(string searchTerm)
    {
        var term = searchTerm.ToLower();
        return await _context.Articles
            .Include(a => a.Category)
            .Include(a => a.Supplier)
            .Include(a => a.AttributeValues)
                .ThenInclude(av => av.CustomAttribute)
            .Where(a =>
                a.Name.ToLower().Contains(term) ||
                a.SKU.ToLower().Contains(term) ||
                a.Brand.ToLower().Contains(term) ||
                a.Description.ToLower().Contains(term))
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync();
    }

    public async Task<Article> CreateAsync(Article article)
    {
        article.CreatedAt = DateTime.UtcNow;
        _context.Articles.Add(article);
        await _context.SaveChangesAsync();
        return article;
    }

    public async Task<Article?> UpdateAsync(Article article)
    {
        var existingArticle = await _context.Articles.FindAsync(article.Id);
        if (existingArticle == null)
            return null;

        article.UpdatedAt = DateTime.UtcNow;
        _context.Entry(existingArticle).CurrentValues.SetValues(article);
        await _context.SaveChangesAsync();
        return existingArticle;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var article = await _context.Articles.FindAsync(id);
        if (article == null)
            return false;

        _context.Articles.Remove(article);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsBySKUAsync(string sku)
    {
        return await _context.Articles.AnyAsync(a => a.SKU == sku);
    }
}
