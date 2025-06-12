using CategoryEntity = PimFlow.Domain.Category.Category;

namespace PimFlow.Domain.Interfaces;

public interface ICategoryRepository
{
    Task<IEnumerable<CategoryEntity>> GetAllAsync();
    Task<IEnumerable<CategoryEntity>> GetActiveAsync();
    Task<IEnumerable<CategoryEntity>> GetRootCategoriesAsync();
    Task<IEnumerable<CategoryEntity>> GetSubCategoriesAsync(int parentId);
    Task<CategoryEntity?> GetByIdAsync(int id);
    Task<CategoryEntity> CreateAsync(CategoryEntity category);
    Task<CategoryEntity?> UpdateAsync(CategoryEntity category);
    Task<bool> DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
}
