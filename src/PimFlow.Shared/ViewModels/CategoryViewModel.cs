using System.ComponentModel.DataAnnotations;

namespace PimFlow.Shared.ViewModels;

/// <summary>
/// ViewModel específico para UI de categorías
/// Contiene validaciones de UI separadas de las validaciones de negocio
/// </summary>
public class CategoryViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Nombre es requerido")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Nombre debe tener entre 2 y 100 caracteres")]
    public string Name { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "Descripción no puede exceder 500 caracteres")]
    public string Description { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsActive { get; set; } = true;
    public int? ParentCategoryId { get; set; }

    // Propiedades para UI jerárquica
    public string? ParentCategoryName { get; set; }
    public List<CategoryViewModel> SubCategories { get; set; } = new();
    public int ArticleCount { get; set; }
    public int Level { get; set; } // Para mostrar jerarquía en UI

    // Propiedades calculadas para UI
    public string DisplayName => Level > 0 ? $"{"  ".PadLeft(Level * 2)}- {Name}" : Name;
    public string StatusText => IsActive ? "Activa" : "Inactiva";
    public string CreatedAtFormatted => CreatedAt.ToString("dd/MM/yyyy");
    public string UpdatedAtFormatted => UpdatedAt?.ToString("dd/MM/yyyy") ?? "-";
    public string HierarchyPath { get; set; } = string.Empty;

    // Validación para UI
    public bool IsValidForDisplay => !string.IsNullOrWhiteSpace(Name);
    public bool HasSubCategories => SubCategories.Any();
    public bool HasArticles => ArticleCount > 0;
    public bool CanBeDeleted => !HasSubCategories && !HasArticles;

    // Métodos de utilidad para UI
    public string GetFullPath()
    {
        if (string.IsNullOrEmpty(ParentCategoryName))
            return Name;
        
        return $"{ParentCategoryName} > {Name}";
    }

    public void AddSubCategory(CategoryViewModel subCategory)
    {
        subCategory.Level = Level + 1;
        SubCategories.Add(subCategory);
    }

    public bool IsDescendantOf(int categoryId)
    {
        return ParentCategoryId == categoryId || 
               SubCategories.Any(sub => sub.IsDescendantOf(categoryId));
    }
}

/// <summary>
/// ViewModel para crear categorías desde UI
/// </summary>
public class CreateCategoryViewModel
{
    [Required(ErrorMessage = "Nombre es requerido")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Nombre debe tener entre 2 y 100 caracteres")]
    public string Name { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "Descripción no puede exceder 500 caracteres")]
    public string Description { get; set; } = string.Empty;

    public int? ParentCategoryId { get; set; }

    // Propiedades para UI
    public string? ParentCategoryName { get; set; }
    public List<CategoryViewModel> AvailableParentCategories { get; set; } = new();

    // Métodos de utilidad para formularios
    public void ClearForm()
    {
        Name = string.Empty;
        Description = string.Empty;
        ParentCategoryId = null;
        ParentCategoryName = null;
    }

    public bool IsFormValid()
    {
        return !string.IsNullOrWhiteSpace(Name);
    }

    public void SetParentCategory(CategoryViewModel? parent)
    {
        if (parent != null)
        {
            ParentCategoryId = parent.Id;
            ParentCategoryName = parent.Name;
        }
        else
        {
            ParentCategoryId = null;
            ParentCategoryName = null;
        }
    }
}

/// <summary>
/// ViewModel para actualizar categorías desde UI
/// </summary>
public class UpdateCategoryViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Nombre es requerido")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Nombre debe tener entre 2 y 100 caracteres")]
    public string Name { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "Descripción no puede exceder 500 caracteres")]
    public string Description { get; set; } = string.Empty;

    public int? ParentCategoryId { get; set; }

    // Propiedades para UI
    public string? ParentCategoryName { get; set; }
    public List<CategoryViewModel> AvailableParentCategories { get; set; } = new();
    public List<CategoryViewModel> SubCategories { get; set; } = new();
    public int ArticleCount { get; set; }

    // Propiedades para tracking de cambios en UI
    public bool HasChanges { get; set; }
    public DateTime LastModified { get; set; } = DateTime.Now;

    // Validaciones específicas para actualización
    public bool CanChangeParent => !SubCategories.Any();
    public bool CanBeDeleted => !SubCategories.Any() && ArticleCount == 0;

    public void MarkAsChanged()
    {
        HasChanges = true;
        LastModified = DateTime.Now;
    }

    public void MarkAsSaved()
    {
        HasChanges = false;
    }

    public bool WouldCreateCircularReference(int? newParentId)
    {
        if (!newParentId.HasValue)
            return false;

        // No puede ser padre de sí mismo
        if (newParentId.Value == Id)
            return true;

        // No puede ser padre de sus descendientes
        return IsDescendantOf(newParentId.Value);
    }

    private bool IsDescendantOf(int categoryId)
    {
        return SubCategories.Any(sub => sub.Id == categoryId || sub.IsDescendantOf(categoryId));
    }
}
