using PimFlow.Domain.ValueObjects;
using PimFlow.Domain.Common;

namespace PimFlow.Domain.Entities;

public class Category
{
    public int Id { get; set; }

    // Value Objects para encapsular validaciones
    private string _name = string.Empty;

    public string Name
    {
        get => _name;
        set => _name = value; // Setter simple para Entity Framework
    }

    public string Description { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsActive { get; set; } = true;
    public int? ParentCategoryId { get; set; }

    // Navigation properties
    public virtual Category? ParentCategory { get; set; }
    public virtual ICollection<Category> SubCategories { get; set; } = new List<Category>();
    public virtual ICollection<Article> Articles { get; set; } = new List<Article>();

    /// <summary>
    /// Métodos de negocio que usan Value Objects para validación
    /// </summary>
    public Result SetName(string name)
    {
        if (!ProductName.IsValid(name))
            return Result.Failure("El nombre debe tener entre 2 y 200 caracteres");

        _name = ProductName.Create(name).Value;
        return Result.Success();
    }

    /// <summary>
    /// Verifica si la categoría puede ser eliminada
    /// Reglas de negocio: No se puede eliminar si tiene subcategorías activas o artículos activos
    /// </summary>
    public Result<DeletionInfo> CanBeDeleted()
    {
        var activeSubCategories = SubCategories.Count(sc => sc.IsActive);
        var activeArticles = Articles.Count(a => a.IsActive);

        if (activeSubCategories > 0)
            return Result.Failure<DeletionInfo>($"No se puede eliminar una categoría que tiene {activeSubCategories} subcategorías activas");

        if (activeArticles > 0)
            return Result.Failure<DeletionInfo>($"No se puede eliminar una categoría que tiene {activeArticles} artículos activos");

        return Result.Success(new DeletionInfo(activeSubCategories, activeArticles));
    }

    /// <summary>
    /// Marca la categoría como eliminada (soft delete)
    /// </summary>
    public Result MarkAsDeleted()
    {
        var canDelete = CanBeDeleted();
        if (canDelete.IsFailure)
            return Result.Failure(canDelete.Error);

        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
        return Result.Success();
    }

    /// <summary>
    /// Verifica si establecer un padre específico crearía una referencia circular
    /// </summary>
    public bool WouldCreateCircularReference(int proposedParentId, Func<int, Category?> getCategoryById)
    {
        if (proposedParentId == Id)
            return true;

        var parentCategory = getCategoryById(proposedParentId);

        while (parentCategory != null)
        {
            if (parentCategory.Id == Id)
                return true;

            if (parentCategory.ParentCategoryId == null)
                break;

            parentCategory = getCategoryById(parentCategory.ParentCategoryId.Value);
        }

        return false;
    }

    /// <summary>
    /// Factory method para crear una categoría válida
    /// </summary>
    public static Result<Category> Create(string name, string description = "", int? parentCategoryId = null)
    {
        var category = new Category
        {
            Description = description,
            ParentCategoryId = parentCategoryId,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        var nameResult = category.SetName(name);
        if (nameResult.IsFailure)
            return Result.Failure<Category>(nameResult.Error);

        return Result.Success(category);
    }
}
