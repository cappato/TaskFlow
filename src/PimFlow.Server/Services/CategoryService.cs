using AutoMapper;
using PimFlow.Domain.Entities;
using PimFlow.Domain.Interfaces;
using PimFlow.Shared.DTOs;

namespace PimFlow.Server.Services;

/// <summary>
/// Category service implementing segregated interfaces
/// Follows Interface Segregation Principle by implementing specific interfaces
/// </summary>
public class CategoryService : ICategoryService, ICategoryReader, ICategoryHierarchy, ICategoryWriter
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IMapper _mapper;

    public CategoryService(ICategoryRepository categoryRepository, IMapper mapper)
    {
        _categoryRepository = categoryRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync()
    {
        var categories = await _categoryRepository.GetAllAsync();
        return _mapper.Map<IEnumerable<CategoryDto>>(categories);
    }

    public async Task<IEnumerable<CategoryDto>> GetActiveCategoriesAsync()
    {
        var categories = await _categoryRepository.GetActiveAsync();
        return _mapper.Map<IEnumerable<CategoryDto>>(categories);
    }

    public async Task<IEnumerable<CategoryDto>> GetRootCategoriesAsync()
    {
        var categories = await _categoryRepository.GetRootCategoriesAsync();
        return _mapper.Map<IEnumerable<CategoryDto>>(categories);
    }

    public async Task<IEnumerable<CategoryDto>> GetSubCategoriesAsync(int parentId)
    {
        var categories = await _categoryRepository.GetSubCategoriesAsync(parentId);
        return _mapper.Map<IEnumerable<CategoryDto>>(categories);
    }

    public async Task<CategoryDto?> GetCategoryByIdAsync(int id)
    {
        var category = await _categoryRepository.GetByIdAsync(id);
        return category != null ? _mapper.Map<CategoryDto>(category) : null;
    }

    public async Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto createCategoryDto)
    {
        var category = _mapper.Map<Category>(createCategoryDto);
        var createdCategory = await _categoryRepository.CreateAsync(category);
        return _mapper.Map<CategoryDto>(createdCategory);
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
        return updatedCategory != null ? _mapper.Map<CategoryDto>(updatedCategory) : null;
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


}
