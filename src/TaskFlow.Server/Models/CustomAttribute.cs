using TaskFlow.Shared.Enums;

namespace TaskFlow.Server.Models;

public class CustomAttribute
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public AttributeType Type { get; set; } = AttributeType.Text;
    public bool IsRequired { get; set; } = false;
    public string? DefaultValue { get; set; }
    public string? ValidationRules { get; set; } // JSON para reglas complejas
    public int SortOrder { get; set; } = 0;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public virtual ICollection<ArticleAttributeValue> AttributeValues { get; set; } = new List<ArticleAttributeValue>();
}
