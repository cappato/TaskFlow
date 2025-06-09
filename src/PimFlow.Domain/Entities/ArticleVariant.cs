namespace PimFlow.Domain.Entities;

public class ArticleVariant
{
    public int Id { get; set; }
    public string SKU { get; set; } = string.Empty;
    public int ArticleId { get; set; }
    public string? Size { get; set; }
    public string? Color { get; set; }
    public decimal Stock { get; set; } = 0;
    public decimal WholesalePrice { get; set; } = 0;
    public decimal? RetailPrice { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public virtual Article Article { get; set; } = null!;
}
