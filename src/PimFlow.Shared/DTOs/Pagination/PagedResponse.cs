namespace PimFlow.Shared.DTOs.Pagination;

/// <summary>
/// Representa una respuesta paginada con metadatos de paginación
/// </summary>
/// <typeparam name="T">Tipo de los elementos en la página</typeparam>
public class PagedResponse<T>
{
    /// <summary>
    /// Elementos de la página actual
    /// </summary>
    public IEnumerable<T> Items { get; set; } = new List<T>();

    /// <summary>
    /// Número de página actual (base 1)
    /// </summary>
    public int PageNumber { get; set; }

    /// <summary>
    /// Tamaño de página
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// Total de elementos en todas las páginas
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// Total de páginas
    /// </summary>
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);

    /// <summary>
    /// Indica si hay una página anterior
    /// </summary>
    public bool HasPreviousPage => PageNumber > 1;

    /// <summary>
    /// Indica si hay una página siguiente
    /// </summary>
    public bool HasNextPage => PageNumber < TotalPages;

    /// <summary>
    /// Número de la primera página
    /// </summary>
    public int FirstPage => 1;

    /// <summary>
    /// Número de la última página
    /// </summary>
    public int LastPage => TotalPages;

    /// <summary>
    /// Número de la página anterior (si existe)
    /// </summary>
    public int? PreviousPage => HasPreviousPage ? PageNumber - 1 : null;

    /// <summary>
    /// Número de la página siguiente (si existe)
    /// </summary>
    public int? NextPage => HasNextPage ? PageNumber + 1 : null;

    /// <summary>
    /// Índice del primer elemento en la página actual (base 1)
    /// </summary>
    public int FirstItemIndex => TotalCount == 0 ? 0 : (PageNumber - 1) * PageSize + 1;

    /// <summary>
    /// Índice del último elemento en la página actual (base 1)
    /// </summary>
    public int LastItemIndex => Math.Min(PageNumber * PageSize, TotalCount);

    /// <summary>
    /// Información de paginación como texto
    /// </summary>
    public string PaginationInfo => TotalCount == 0 
        ? "No hay elementos" 
        : $"Mostrando {FirstItemIndex}-{LastItemIndex} de {TotalCount} elementos";

    /// <summary>
    /// Constructor vacío
    /// </summary>
    public PagedResponse() { }

    /// <summary>
    /// Constructor con parámetros
    /// </summary>
    public PagedResponse(IEnumerable<T> items, int pageNumber, int pageSize, int totalCount)
    {
        Items = items;
        PageNumber = pageNumber;
        PageSize = pageSize;
        TotalCount = totalCount;
    }

    /// <summary>
    /// Crea una respuesta paginada vacía
    /// </summary>
    public static PagedResponse<T> Empty(int pageNumber = 1, int pageSize = 10)
    {
        return new PagedResponse<T>(new List<T>(), pageNumber, pageSize, 0);
    }

    /// <summary>
    /// Crea una respuesta paginada a partir de una lista completa
    /// </summary>
    public static PagedResponse<T> Create(IEnumerable<T> allItems, int pageNumber, int pageSize)
    {
        var totalCount = allItems.Count();
        var items = allItems
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize);

        return new PagedResponse<T>(items, pageNumber, pageSize, totalCount);
    }
}
