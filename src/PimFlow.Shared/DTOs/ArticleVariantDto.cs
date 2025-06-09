namespace PimFlow.Shared.DTOs;

public class ArticleVariantDto
{
    public int Id { get; set; }
    public string SKU { get; set; } = string.Empty;
    public int ArticleId { get; set; }
    public string? Size { get; set; }
    public string? Color { get; set; }
    public decimal Stock { get; set; }
    public decimal WholesalePrice { get; set; }
    public decimal? RetailPrice { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
