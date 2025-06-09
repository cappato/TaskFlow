using PimFlow.Shared.Common;
using PimFlow.Shared.ViewModels;

namespace PimFlow.Shared.Services;

/// <summary>
/// Interfaz de servicio para operaciones de artículos desde el frontend
/// Usa ViewModels y ApiResponse para mejor desacoplamiento
/// </summary>
public interface IArticleService
{
    /// <summary>
    /// Obtiene todos los artículos
    /// </summary>
    Task<ApiResponse<List<ArticleViewModel>>> GetAllArticlesAsync();

    /// <summary>
    /// Obtiene un artículo por ID
    /// </summary>
    Task<ApiResponse<ArticleViewModel>> GetArticleByIdAsync(int id);

    /// <summary>
    /// Busca artículos por término
    /// </summary>
    Task<ApiResponse<List<ArticleViewModel>>> SearchArticlesAsync(string searchTerm);

    /// <summary>
    /// Obtiene artículos por atributo personalizado
    /// </summary>
    Task<ApiResponse<List<ArticleViewModel>>> GetArticlesByAttributeAsync(string attributeName, string value);

    /// <summary>
    /// Crea un nuevo artículo
    /// </summary>
    Task<ApiResponse<ArticleViewModel>> CreateArticleAsync(CreateArticleViewModel createModel);

    /// <summary>
    /// Actualiza un artículo existente
    /// </summary>
    Task<ApiResponse<ArticleViewModel>> UpdateArticleAsync(int id, UpdateArticleViewModel updateModel);

    /// <summary>
    /// Elimina un artículo
    /// </summary>
    Task<ApiResponse> DeleteArticleAsync(int id);

    /// <summary>
    /// Valida un artículo antes de crear/actualizar
    /// </summary>
    Task<ApiResponse> ValidateArticleAsync(CreateArticleViewModel model);

    /// <summary>
    /// Verifica si un SKU está disponible
    /// </summary>
    Task<ApiResponse<bool>> IsSKUAvailableAsync(string sku, int? excludeId = null);

    /// <summary>
    /// Obtiene estadísticas de artículos
    /// </summary>
    Task<ApiResponse<ArticleStatsViewModel>> GetArticleStatsAsync();
}

/// <summary>
/// ViewModel para estadísticas de artículos
/// </summary>
public class ArticleStatsViewModel
{
    public int TotalArticles { get; set; }
    public int ActiveArticles { get; set; }
    public int InactiveArticles { get; set; }
    public int TotalVariants { get; set; }
    public Dictionary<string, int> ArticlesByType { get; set; } = new();
    public Dictionary<string, int> ArticlesByBrand { get; set; } = new();
    public DateTime LastUpdated { get; set; } = DateTime.Now;

    public double ActivePercentage => TotalArticles > 0 ? (double)ActiveArticles / TotalArticles * 100 : 0;
    public double InactivePercentage => TotalArticles > 0 ? (double)InactiveArticles / TotalArticles * 100 : 0;
}
