namespace PimFlow.Domain.CustomAttribute;

public class ArticleAttributeValue
{
    public int Id { get; set; }
    public int ArticleId { get; set; }
    public int CustomAttributeId { get; set; }
    public string Value { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public virtual PimFlow.Domain.Article.Article Article { get; set; } = null!;
    public virtual CustomAttribute CustomAttribute { get; set; } = null!;
}
