using PimFlow.Domain.Entities;
using PimFlow.Domain.Interfaces;
using PimFlow.Shared.DTOs;

namespace PimFlow.Server.Services;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _categoryRepository;

    public CategoryService(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync()
    {
        var categories = await _categoryRepository.GetAllAsync();
        return categories.Select(MapToDto);
    }

    public async Task<IEnumerable<CategoryDto>> GetActiveCategoriesAsync()
    {
        var categories = await _categoryRepository.GetActiveAsync();
        return categories.Select(MapToDto);
    }

    public async Task<IEnumerable<CategoryDto>> GetRootCategoriesAsync()
    {
        var categories = await _categoryRepository.GetRootCategoriesAsync();
        return categories.Select(MapToDto);
    }

    public async Task<IEnumerable<CategoryDto>> GetSubCategoriesAsync(int parentId)
    {
        var categories = await _categoryRepository.GetSubCategoriesAsync(parentId);
        return categories.Select(MapToDto);
    }

    public async Task<CategoryDto?> GetCategoryByIdAsync(int id)
    {
        var category = await _categoryRepository.GetByIdAsync(id);
        return category != null ? MapToDto(category) : null;
    }

    public async Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto createCategoryDto)
    {
        var category = new Category
        {
            Name = createCategoryDto.Name,
            Description = createCategoryDto.Description,
            ParentCategoryId = createCategoryDto.ParentCategoryId,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        var createdCategory = await _categoryRepository.CreateAsync(category);
        return MapToDto(createdCategory);
    }

    public async Task<CategoryDto?> UpdateCategoryAsync(int id, UpdateCategoryDto updateCategoryDto)
    {
        var existingCategory = await _categoryRepository.GetByIdAsync(id);
        if (existingCategory == null)
            return null;

        // Validate that we're not creating a circular reference
        if (updateCategoryDto.ParentCategoryId.HasValue)
        {
            if (await WouldCreateCircularReference(id, updateCategoryDto.ParentCategoryId.Value))
            {
                throw new InvalidOperationException("No se puede establecer esta categoría padre porque crearía una referencia circular.");
            }
        }

        // Update properties
        if (!string.IsNullOrEmpty(updateCategoryDto.Name))
            existingCategory.Name = updateCategoryDto.Name;

        if (!string.IsNullOrEmpty(updateCategoryDto.Description))
            existingCategory.Description = updateCategoryDto.Description;

        if (updateCategoryDto.ParentCategoryId.HasValue)
            existingCategory.ParentCategoryId = updateCategoryDto.ParentCategoryId.Value;

        if (updateCategoryDto.IsActive.HasValue)
            existingCategory.IsActive = updateCategoryDto.IsActive.Value;

        existingCategory.UpdatedAt = DateTime.UtcNow;

        var updatedCategory = await _categoryRepository.UpdateAsync(existingCategory);
        return updatedCategory != null ? MapToDto(updatedCategory) : null;
    }

    public async Task<bool> DeleteCategoryAsync(int id)
    {
        // Check if category has subcategories or articles
        var category = await _categoryRepository.GetByIdAsync(id);
        if (category == null)
            return false;

        if (category.SubCategories.Any())
        {
            throw new InvalidOperationException("No se puede eliminar una categoría que tiene subcategorías.");
        }

        if (category.Articles.Any())
        {
            throw new InvalidOperationException("No se puede eliminar una categoría que tiene artículos asociados.");
        }

        return await _categoryRepository.DeleteAsync(id);
    }

    private async Task<bool> WouldCreateCircularReference(int categoryId, int parentCategoryId)
    {
        // Check if the proposed parent is actually a descendant of this category
        var parentCategory = await _categoryRepository.GetByIdAsync(parentCategoryId);

        while (parentCategory != null)
        {
            if (parentCategory.Id == categoryId)
                return true;

            if (parentCategory.ParentCategoryId == null)
                break;

            parentCategory = await _categoryRepository.GetByIdAsync(parentCategory.ParentCategoryId.Value);
        }

        return false;
    }

    private static CategoryDto MapToDto(Category category)
    {
        return new CategoryDto
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description,
            CreatedAt = category.CreatedAt,
            UpdatedAt = category.UpdatedAt,
            IsActive = category.IsActive,
            ParentCategoryId = category.ParentCategoryId,
            ParentCategoryName = category.ParentCategory?.Name,
            ArticleCount = category.Articles?.Count ?? 0,
            SubCategoryCount = category.SubCategories?.Count ?? 0
        };
    }
}
