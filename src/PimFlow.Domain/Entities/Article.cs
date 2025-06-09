using PimFlow.Domain.Enums;

namespace PimFlow.Domain.Entities;

public class Article
{
    public int Id { get; set; }
    public string SKU { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public ArticleType Type { get; set; } = ArticleType.Footwear;
    public string Brand { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsActive { get; set; } = true;

    // Foreign keys
    public int? CategoryId { get; set; }
    public int? SupplierId { get; set; }

    // Navigation properties
    public virtual Category? Category { get; set; }
    public virtual User? Supplier { get; set; }
    public virtual ICollection<ArticleAttributeValue> AttributeValues { get; set; } = new List<ArticleAttributeValue>();
    public virtual ICollection<ArticleVariant> Variants { get; set; } = new List<ArticleVariant>();
}
