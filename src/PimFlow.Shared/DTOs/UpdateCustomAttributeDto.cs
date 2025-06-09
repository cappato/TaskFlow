using PimFlow.Shared.Enums;

namespace PimFlow.Shared.DTOs;

public class UpdateCustomAttributeDto
{
    public string? Name { get; set; }
    public string? DisplayName { get; set; }
    public AttributeType? Type { get; set; }
    public bool? IsRequired { get; set; }
    public string? DefaultValue { get; set; }
    public string? ValidationRules { get; set; }
    public int? SortOrder { get; set; }
    public bool? IsActive { get; set; }
}
