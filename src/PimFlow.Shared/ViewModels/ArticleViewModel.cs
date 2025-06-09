using System.ComponentModel.DataAnnotations;

namespace PimFlow.Shared.ViewModels;

/// <summary>
/// ViewModel específico para UI de artículos
/// Contiene validaciones de UI separadas de las validaciones de negocio
/// </summary>
public class ArticleViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "SKU es requerido")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "SKU debe tener entre 3 y 50 caracteres")]
    [RegularExpression(@"^[A-Z0-9-]+$", ErrorMessage = "SKU solo puede contener letras mayúsculas, números y guiones")]
    public string SKU { get; set; } = string.Empty;

    [Required(ErrorMessage = "Nombre es requerido")]
    [StringLength(200, MinimumLength = 2, ErrorMessage = "Nombre debe tener entre 2 y 200 caracteres")]
    public string Name { get; set; } = string.Empty;

    [StringLength(1000, ErrorMessage = "Descripción no puede exceder 1000 caracteres")]
    public string Description { get; set; } = string.Empty;

    [Required(ErrorMessage = "Tipo es requerido")]
    public string Type { get; set; } = "Footwear";

    [Required(ErrorMessage = "Marca es requerida")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Marca debe tener entre 2 y 100 caracteres")]
    public string Brand { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsActive { get; set; } = true;

    // Related entities (solo IDs y nombres para UI)
    public int? CategoryId { get; set; }
    public string? CategoryName { get; set; }
    public int? SupplierId { get; set; }
    public string? SupplierName { get; set; }

    // Dynamic attributes para UI
    public Dictionary<string, object> CustomAttributes { get; set; } = new();

    // Propiedades calculadas para UI
    public string DisplayName => $"{Brand} - {Name}";
    public string StatusText => IsActive ? "Activo" : "Inactivo";
    public string CreatedAtFormatted => CreatedAt.ToString("dd/MM/yyyy");
    public string UpdatedAtFormatted => UpdatedAt?.ToString("dd/MM/yyyy") ?? "-";

    // Validación personalizada para UI
    public bool IsValidForDisplay => 
        !string.IsNullOrWhiteSpace(SKU) && 
        !string.IsNullOrWhiteSpace(Name) && 
        !string.IsNullOrWhiteSpace(Brand);

    // Métodos de utilidad para UI
    public string GetAttributeDisplayValue(string attributeName)
    {
        if (CustomAttributes.TryGetValue(attributeName, out var value))
        {
            return value?.ToString() ?? string.Empty;
        }
        return string.Empty;
    }

    public void SetAttributeValue(string attributeName, object value)
    {
        CustomAttributes[attributeName] = value;
    }

    public bool HasAttribute(string attributeName)
    {
        return CustomAttributes.ContainsKey(attributeName);
    }
}

/// <summary>
/// ViewModel para crear artículos desde UI
/// </summary>
public class CreateArticleViewModel
{
    [Required(ErrorMessage = "SKU es requerido")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "SKU debe tener entre 3 y 50 caracteres")]
    [RegularExpression(@"^[A-Z0-9-]+$", ErrorMessage = "SKU solo puede contener letras mayúsculas, números y guiones")]
    public string SKU { get; set; } = string.Empty;

    [Required(ErrorMessage = "Nombre es requerido")]
    [StringLength(200, MinimumLength = 2, ErrorMessage = "Nombre debe tener entre 2 y 200 caracteres")]
    public string Name { get; set; } = string.Empty;

    [StringLength(1000, ErrorMessage = "Descripción no puede exceder 1000 caracteres")]
    public string Description { get; set; } = string.Empty;

    [Required(ErrorMessage = "Tipo es requerido")]
    public string Type { get; set; } = "Footwear";

    [Required(ErrorMessage = "Marca es requerida")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Marca debe tener entre 2 y 100 caracteres")]
    public string Brand { get; set; } = string.Empty;

    public int? CategoryId { get; set; }
    public int? SupplierId { get; set; }

    public Dictionary<string, object> CustomAttributes { get; set; } = new();

    // Métodos de utilidad para formularios
    public void ClearForm()
    {
        SKU = string.Empty;
        Name = string.Empty;
        Description = string.Empty;
        Type = "Footwear";
        Brand = string.Empty;
        CategoryId = null;
        SupplierId = null;
        CustomAttributes.Clear();
    }

    public bool IsFormValid()
    {
        return !string.IsNullOrWhiteSpace(SKU) &&
               !string.IsNullOrWhiteSpace(Name) &&
               !string.IsNullOrWhiteSpace(Brand) &&
               !string.IsNullOrWhiteSpace(Type);
    }
}

/// <summary>
/// ViewModel para actualizar artículos desde UI
/// </summary>
public class UpdateArticleViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "SKU es requerido")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "SKU debe tener entre 3 y 50 caracteres")]
    [RegularExpression(@"^[A-Z0-9-]+$", ErrorMessage = "SKU solo puede contener letras mayúsculas, números y guiones")]
    public string SKU { get; set; } = string.Empty;

    [Required(ErrorMessage = "Nombre es requerido")]
    [StringLength(200, MinimumLength = 2, ErrorMessage = "Nombre debe tener entre 2 y 200 caracteres")]
    public string Name { get; set; } = string.Empty;

    [StringLength(1000, ErrorMessage = "Descripción no puede exceder 1000 caracteres")]
    public string Description { get; set; } = string.Empty;

    [Required(ErrorMessage = "Tipo es requerido")]
    public string Type { get; set; } = "Footwear";

    [Required(ErrorMessage = "Marca es requerida")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Marca debe tener entre 2 y 100 caracteres")]
    public string Brand { get; set; } = string.Empty;

    public int? CategoryId { get; set; }
    public int? SupplierId { get; set; }

    public Dictionary<string, object> CustomAttributes { get; set; } = new();

    // Propiedades para tracking de cambios en UI
    public bool HasChanges { get; set; }
    public DateTime LastModified { get; set; } = DateTime.Now;

    public void MarkAsChanged()
    {
        HasChanges = true;
        LastModified = DateTime.Now;
    }

    public void MarkAsSaved()
    {
        HasChanges = false;
    }
}
