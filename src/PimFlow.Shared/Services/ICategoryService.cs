using PimFlow.Shared.Common;
using PimFlow.Shared.ViewModels;

namespace PimFlow.Shared.Services;

/// <summary>
/// Interfaz de servicio para operaciones de categorías desde el frontend
/// Usa ViewModels y ApiResponse para mejor desacoplamiento
/// </summary>
public interface ICategoryService
{
    /// <summary>
    /// Obtiene todas las categorías con jerarquía
    /// </summary>
    Task<ApiResponse<List<CategoryViewModel>>> GetAllCategoriesAsync();

    /// <summary>
    /// Obtiene todas las categorías en formato plano
    /// </summary>
    Task<ApiResponse<List<CategoryViewModel>>> GetFlatCategoriesAsync();

    /// <summary>
    /// Obtiene una categoría por ID
    /// </summary>
    Task<ApiResponse<CategoryViewModel>> GetCategoryByIdAsync(int id);

    /// <summary>
    /// Obtiene las categorías raíz (sin padre)
    /// </summary>
    Task<ApiResponse<List<CategoryViewModel>>> GetRootCategoriesAsync();

    /// <summary>
    /// Obtiene las subcategorías de una categoría
    /// </summary>
    Task<ApiResponse<List<CategoryViewModel>>> GetSubCategoriesAsync(int parentId);

    /// <summary>
    /// Crea una nueva categoría
    /// </summary>
    Task<ApiResponse<CategoryViewModel>> CreateCategoryAsync(CreateCategoryViewModel createModel);

    /// <summary>
    /// Actualiza una categoría existente
    /// </summary>
    Task<ApiResponse<CategoryViewModel>> UpdateCategoryAsync(int id, UpdateCategoryViewModel updateModel);

    /// <summary>
    /// Elimina una categoría
    /// </summary>
    Task<ApiResponse> DeleteCategoryAsync(int id);

    /// <summary>
    /// Valida una categoría antes de crear/actualizar
    /// </summary>
    Task<ApiResponse> ValidateCategoryAsync(CreateCategoryViewModel model);

    /// <summary>
    /// Verifica si una categoría puede ser eliminada
    /// </summary>
    Task<ApiResponse<bool>> CanDeleteCategoryAsync(int id);

    /// <summary>
    /// Obtiene las categorías disponibles como padre para una categoría específica
    /// </summary>
    Task<ApiResponse<List<CategoryViewModel>>> GetAvailableParentCategoriesAsync(int? excludeCategoryId = null);

    /// <summary>
    /// Verifica si establecer una categoría padre crearía una referencia circular
    /// </summary>
    Task<ApiResponse<bool>> WouldCreateCircularReferenceAsync(int categoryId, int parentId);

    /// <summary>
    /// Obtiene estadísticas de categorías
    /// </summary>
    Task<ApiResponse<CategoryStatsViewModel>> GetCategoryStatsAsync();
}

/// <summary>
/// ViewModel para estadísticas de categorías
/// </summary>
public class CategoryStatsViewModel
{
    public int TotalCategories { get; set; }
    public int RootCategories { get; set; }
    public int CategoriesWithSubcategories { get; set; }
    public int CategoriesWithArticles { get; set; }
    public int MaxDepthLevel { get; set; }
    public Dictionary<int, int> CategoriesByLevel { get; set; } = new();
    public List<CategoryUsageViewModel> TopCategoriesByArticleCount { get; set; } = new();
    public DateTime LastUpdated { get; set; } = DateTime.Now;

    public double CategoriesWithSubcategoriesPercentage => 
        TotalCategories > 0 ? (double)CategoriesWithSubcategories / TotalCategories * 100 : 0;
    
    public double CategoriesWithArticlesPercentage => 
        TotalCategories > 0 ? (double)CategoriesWithArticles / TotalCategories * 100 : 0;
}

/// <summary>
/// ViewModel para uso de categorías
/// </summary>
public class CategoryUsageViewModel
{
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public string HierarchyPath { get; set; } = string.Empty;
    public int ArticleCount { get; set; }
    public int SubcategoryCount { get; set; }
}
