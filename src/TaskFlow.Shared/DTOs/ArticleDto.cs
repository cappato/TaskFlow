using TaskFlow.Shared.Enums;

namespace TaskFlow.Shared.DTOs;

public class ArticleDto
{
    public int Id { get; set; }
    public string SKU { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public ArticleType Type { get; set; }
    public string Brand { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsActive { get; set; }

    // Related entities
    public int? CategoryId { get; set; }
    public string? CategoryName { get; set; }
    public int? SupplierId { get; set; }
    public string? SupplierName { get; set; }

    // Dynamic attributes
    public Dictionary<string, object> CustomAttributes { get; set; } = new();

    // Variants
    public ICollection<ArticleVariantDto> Variants { get; set; } = new List<ArticleVariantDto>();
}
