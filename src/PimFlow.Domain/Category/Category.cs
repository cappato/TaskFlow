using PimFlow.Domain.Category.ValueObjects;
using PimFlow.Domain.Article.ValueObjects;
using PimFlow.Domain.Common;

namespace PimFlow.Domain.Category;

public class Category : AggregateRoot
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
    public virtual ICollection<PimFlow.Domain.Article.Article> Articles { get; set; } = new List<PimFlow.Domain.Article.Article>();

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
    /// Marca la categoría como eliminada (soft delete) y publica evento
    /// </summary>
    public Result MarkAsDeleted(string reason = "Manual deletion")
    {
        var canDelete = CanBeDeleted();
        if (canDelete.IsFailure)
            return Result.Failure(canDelete.Error);

        IsActive = false;
        UpdatedAt = DateTime.UtcNow;

        // Publicar evento de dominio
        AddDomainEvent(new CategoryDeletedEvent(Id, Name, canDelete.Value.Summary, reason));

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

    /// <summary>
    /// Cambia la categoría padre y publica evento
    /// </summary>
    public Result ChangeParentTo(int? newParentId, Func<int, Category?> getCategoryById)
    {
        // Verificar referencias circulares
        if (newParentId.HasValue && WouldCreateCircularReference(newParentId.Value, getCategoryById))
        {
            return Result.Failure("No se puede establecer esta categoría padre porque crearía una referencia circular");
        }

        var previousParentId = ParentCategoryId;

        if (previousParentId == newParentId)
            return Result.Success(); // No hay cambio

        ParentCategoryId = newParentId;
        UpdatedAt = DateTime.UtcNow;

        // Publicar evento de dominio
        AddDomainEvent(new CategoryHierarchyChangedEvent(Id, Name, previousParentId, newParentId));

        return Result.Success();
    }

    /// <summary>
    /// Actualiza la categoría y publica evento con campos modificados
    /// </summary>
    public Result UpdateWith(string? name = null, string? description = null, List<string>? modifiedFields = null)
    {
        var fieldsChanged = modifiedFields ?? new List<string>();

        if (!string.IsNullOrEmpty(name) && name != Name)
        {
            var nameResult = SetName(name);
            if (nameResult.IsFailure)
                return nameResult;
            fieldsChanged.Add("Name");
        }

        if (!string.IsNullOrEmpty(description) && description != Description)
        {
            Description = description;
            fieldsChanged.Add("Description");
        }

        if (fieldsChanged.Any())
        {
            UpdatedAt = DateTime.UtcNow;

            // Publicar evento de dominio
            AddDomainEvent(new CategoryUpdatedEvent(Id, Name, fieldsChanged));
        }

        return Result.Success();
    }

    /// <summary>
    /// Método interno para publicar evento de creación (llamado después de persistir)
    /// </summary>
    internal void PublishCreatedEvent()
    {
        AddDomainEvent(new CategoryCreatedEvent(Id, Name, Description, ParentCategoryId));
    }

    /// <summary>
    /// Método para notificar cuando se agrega un artículo a esta categoría
    /// </summary>
    internal void NotifyArticleAdded(int articleId, string articleSKU, string articleName)
    {
        AddDomainEvent(new ArticleAddedToCategoryEvent(Id, Name, articleId, articleSKU, articleName));
    }
}
