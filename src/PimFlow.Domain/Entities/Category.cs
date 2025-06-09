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
