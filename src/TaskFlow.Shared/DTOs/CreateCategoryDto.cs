using System.ComponentModel.DataAnnotations;

namespace TaskFlow.Shared.DTOs;

public class CreateCategoryDto
{
    [Required(ErrorMessage = "Nombre es requerido")]
    [StringLength(100, ErrorMessage = "Nombre no puede exceder 100 caracteres")]
    public string Name { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "Descripción no puede exceder 500 caracteres")]
    public string Description { get; set; } = string.Empty;

    public int? ParentCategoryId { get; set; }
}
