using Microsoft.EntityFrameworkCore;
using TaskFlow.Server.Data;
using TaskFlow.Domain.Entities;
using TaskFlow.Domain.Interfaces;

namespace TaskFlow.Server.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly TaskFlowDbContext _context;

    public CategoryRepository(TaskFlowDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Category>> GetAllAsync()
    {
        return await _context.Categories
            .Include(c => c.ParentCategory)
            .Include(c => c.SubCategories)
            .Include(c => c.Articles)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();
    }

    public async Task<Category?> GetByIdAsync(int id)
    {
        return await _context.Categories
            .Include(c => c.ParentCategory)
            .Include(c => c.SubCategories)
            .Include(c => c.Articles)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<IEnumerable<Category>> GetActiveAsync()
    {
        return await _context.Categories
            .Include(c => c.ParentCategory)
            .Include(c => c.SubCategories)
            .Where(c => c.IsActive)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Category>> GetRootCategoriesAsync()
    {
        return await _context.Categories
            .Include(c => c.SubCategories)
            .Where(c => c.ParentCategoryId == null && c.IsActive)
            .OrderBy(c => c.Name)
            .ToListAsync();
    }

    public async Task<IEnumerable<Category>> GetSubCategoriesAsync(int parentId)
    {
        return await _context.Categories
            .Include(c => c.SubCategories)
            .Where(c => c.ParentCategoryId == parentId && c.IsActive)
            .OrderBy(c => c.Name)
            .ToListAsync();
    }

    public async Task<Category> CreateAsync(Category category)
    {
        category.CreatedAt = DateTime.UtcNow;
        _context.Categories.Add(category);
        await _context.SaveChangesAsync();

        return await GetByIdAsync(category.Id) ?? category;
    }

    public async Task<Category?> UpdateAsync(Category category)
    {
        var existingCategory = await _context.Categories.FindAsync(category.Id);
        if (existingCategory == null)
            return null;

        category.UpdatedAt = DateTime.UtcNow;
        _context.Entry(existingCategory).CurrentValues.SetValues(category);
        await _context.SaveChangesAsync();
        return await GetByIdAsync(category.Id);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var category = await _context.Categories.FindAsync(id);
        if (category == null)
            return false;

        _context.Categories.Remove(category);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.Categories.AnyAsync(c => c.Id == id);
    }
}
