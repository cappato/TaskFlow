using System.ComponentModel.DataAnnotations;
using TaskFlow.Domain.Enums;

namespace TaskFlow.Shared.DTOs;

public class CreateArticleDto
{
    [Required(ErrorMessage = "SKU es requerido")]
    [StringLength(50, ErrorMessage = "SKU no puede exceder 50 caracteres")]
    public string SKU { get; set; } = string.Empty;

    [Required(ErrorMessage = "Nombre es requerido")]
    [StringLength(200, ErrorMessage = "Nombre no puede exceder 200 caracteres")]
    public string Name { get; set; } = string.Empty;

    [StringLength(1000, ErrorMessage = "Descripción no puede exceder 1000 caracteres")]
    public string Description { get; set; } = string.Empty;

    public ArticleType Type { get; set; } = ArticleType.Footwear;

    [StringLength(100, ErrorMessage = "Marca no puede exceder 100 caracteres")]
    public string Brand { get; set; } = string.Empty;

    public int? CategoryId { get; set; }
    public int? SupplierId { get; set; }

    // Dynamic attributes as key-value pairs
    public Dictionary<string, object> CustomAttributes { get; set; } = new();
}
