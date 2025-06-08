using TaskFlow.Shared.DTOs;

namespace TaskFlow.Server.Services;

public interface ICategoryService
{
    Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync();
    Task<IEnumerable<CategoryDto>> GetActiveCategoriesAsync();
    Task<IEnumerable<CategoryDto>> GetRootCategoriesAsync();
    Task<IEnumerable<CategoryDto>> GetSubCategoriesAsync(int parentId);
    Task<CategoryDto?> GetCategoryByIdAsync(int id);
    Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto createCategoryDto);
    Task<CategoryDto?> UpdateCategoryAsync(int id, UpdateCategoryDto updateCategoryDto);
    Task<bool> DeleteCategoryAsync(int id);
}
