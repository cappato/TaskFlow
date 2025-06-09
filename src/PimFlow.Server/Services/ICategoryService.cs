using PimFlow.Shared.DTOs;

namespace PimFlow.Server.Services;

/// <summary>
/// INTERFACE SEGREGATION PRINCIPLE (ISP) - Interfaces segregadas para Category
/// </summary>

/// <summary>
/// Interface for Category read operations only
/// Follows ISP - clients that only need to read categories don't depend on write operations
/// </summary>
public interface ICategoryReader
{
    Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync();
    Task<IEnumerable<CategoryDto>> GetActiveCategoriesAsync();
    Task<CategoryDto?> GetCategoryByIdAsync(int id);
}

/// <summary>
/// Interface for Category hierarchy operations
/// Follows ISP - specialized interface for hierarchical navigation
/// </summary>
public interface ICategoryHierarchy
{
    Task<IEnumerable<CategoryDto>> GetRootCategoriesAsync();
    Task<IEnumerable<CategoryDto>> GetSubCategoriesAsync(int parentId);
}

/// <summary>
/// Interface for Category write operations only
/// Follows ISP - clients that only need to write categories don't depend on read operations
/// </summary>
public interface ICategoryWriter
{
    Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto createCategoryDto);
    Task<CategoryDto?> UpdateCategoryAsync(int id, UpdateCategoryDto updateCategoryDto);
    Task<bool> DeleteCategoryAsync(int id);
}

/// <summary>
/// Facade interface that combines all Category operations for backward compatibility
/// Follows ISP by inheriting from segregated interfaces
/// </summary>
public interface ICategoryService : ICategoryReader, ICategoryHierarchy, ICategoryWriter
{
    // This interface is now composed of segregated interfaces
    // Clients can depend on specific interfaces (ICategoryReader, ICategoryWriter, etc.)
    // or on the full interface for backward compatibility
}
