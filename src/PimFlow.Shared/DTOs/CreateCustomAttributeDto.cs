using System.ComponentModel.DataAnnotations;
using PimFlow.Shared.Enums;

namespace PimFlow.Shared.DTOs;

public class CreateCustomAttributeDto
{
    [Required(ErrorMessage = "Nombre es requerido")]
    [StringLength(100, ErrorMessage = "Nombre no puede exceder 100 caracteres")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Nombre para mostrar es requerido")]
    [StringLength(150, ErrorMessage = "Nombre para mostrar no puede exceder 150 caracteres")]
    public string DisplayName { get; set; } = string.Empty;

    public AttributeType Type { get; set; } = AttributeType.Text;
    public bool IsRequired { get; set; } = false;
    public string? DefaultValue { get; set; }
    public string? ValidationRules { get; set; }
    public int SortOrder { get; set; } = 0;
}
