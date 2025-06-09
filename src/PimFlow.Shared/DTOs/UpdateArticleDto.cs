using PimFlow.Domain.Enums;

namespace PimFlow.Shared.DTOs;

public class UpdateArticleDto
{
    public string? SKU { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public ArticleType? Type { get; set; }
    public string? Brand { get; set; }
    public int? CategoryId { get; set; }
    public int? SupplierId { get; set; }
    public bool? IsActive { get; set; }

    // Dynamic attributes as key-value pairs
    public Dictionary<string, object>? CustomAttributes { get; set; }
}
