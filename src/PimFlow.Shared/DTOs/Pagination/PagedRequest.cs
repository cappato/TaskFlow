namespace PimFlow.Shared.DTOs.Pagination;

/// <summary>
/// Representa una solicitud paginada con parámetros de paginación
/// </summary>
public class PagedRequest
{
    private int _pageNumber = 1;
    private int _pageSize = 10;

    /// <summary>
    /// Número de página (base 1)
    /// </summary>
    public int PageNumber
    {
        get => _pageNumber;
        set => _pageNumber = value < 1 ? 1 : value;
    }

    /// <summary>
    /// Tamaño de página (elementos por página)
    /// </summary>
    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = value switch
        {
            < 1 => 10,
            > 100 => 100, // Límite máximo para evitar sobrecarga
            _ => value
        };
    }

    /// <summary>
    /// Término de búsqueda opcional
    /// </summary>
    public string? SearchTerm { get; set; }

    /// <summary>
    /// Campo por el cual ordenar
    /// </summary>
    public string? SortBy { get; set; }

    /// <summary>
    /// Dirección del ordenamiento (asc/desc)
    /// </summary>
    public string? SortDirection { get; set; } = "asc";

    /// <summary>
    /// Calcula el número de elementos a saltar
    /// </summary>
    public int Skip => (PageNumber - 1) * PageSize;

    /// <summary>
    /// Indica si el ordenamiento es descendente
    /// </summary>
    public bool IsDescending => SortDirection?.ToLower() == "desc";
}
