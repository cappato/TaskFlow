using TaskFlow.Server.Models;

namespace TaskFlow.Server.Repositories;

public interface ICategoryRepository
{
    Task<IEnumerable<Category>> GetAllAsync();
    Task<IEnumerable<Category>> GetActiveAsync();
    Task<IEnumerable<Category>> GetRootCategoriesAsync();
    Task<IEnumerable<Category>> GetSubCategoriesAsync(int parentId);
    Task<Category?> GetByIdAsync(int id);
    Task<Category> CreateAsync(Category category);
    Task<Category?> UpdateAsync(Category category);
    Task<bool> DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
}
