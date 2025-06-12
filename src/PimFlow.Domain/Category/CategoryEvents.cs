using PimFlow.Domain.Common;

namespace PimFlow.Domain.Category;

/// <summary>
/// Evento que se dispara cuando se crea una nueva categoría
/// </summary>
public class CategoryCreatedEvent : DomainEventBase
{
    public override string EventName => "CategoryCreated";

    /// <summary>
    /// ID de la categoría creada
    /// </summary>
    public int CategoryId { get; }

    /// <summary>
    /// Nombre de la categoría creada
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Descripción de la categoría
    /// </summary>
    public string Description { get; }

    /// <summary>
    /// ID de la categoría padre (si tiene)
    /// </summary>
    public int? ParentCategoryId { get; }

    public CategoryCreatedEvent(int categoryId, string name, string description, int? parentCategoryId = null)
    {
        CategoryId = categoryId;
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Description = description ?? string.Empty;
        ParentCategoryId = parentCategoryId;
    }
}

/// <summary>
/// Evento que se dispara cuando se actualiza una categoría
/// </summary>
public class CategoryUpdatedEvent : DomainEventBase
{
    public override string EventName => "CategoryUpdated";

    /// <summary>
    /// ID de la categoría actualizada
    /// </summary>
    public int CategoryId { get; }

    /// <summary>
    /// Nombre actualizado de la categoría
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Campos que fueron modificados
    /// </summary>
    public IReadOnlyList<string> ModifiedFields { get; }

    public CategoryUpdatedEvent(int categoryId, string name, IEnumerable<string> modifiedFields)
    {
        CategoryId = categoryId;
        Name = name ?? throw new ArgumentNullException(nameof(name));
        ModifiedFields = modifiedFields?.ToList().AsReadOnly() ?? throw new ArgumentNullException(nameof(modifiedFields));
    }
}

/// <summary>
/// Evento que se dispara cuando se elimina (desactiva) una categoría
/// </summary>
public class CategoryDeletedEvent : DomainEventBase
{
    public override string EventName => "CategoryDeleted";

    /// <summary>
    /// ID de la categoría eliminada
    /// </summary>
    public int CategoryId { get; }

    /// <summary>
    /// Nombre de la categoría eliminada
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Información sobre la eliminación
    /// </summary>
    public string DeletionSummary { get; }

    /// <summary>
    /// Razón de la eliminación
    /// </summary>
    public string Reason { get; }

    public CategoryDeletedEvent(int categoryId, string name, string deletionSummary, string reason = "Manual deletion")
    {
        CategoryId = categoryId;
        Name = name ?? throw new ArgumentNullException(nameof(name));
        DeletionSummary = deletionSummary ?? string.Empty;
        Reason = reason ?? "Manual deletion";
    }
}

/// <summary>
/// Evento que se dispara cuando se cambia la jerarquía de una categoría
/// </summary>
public class CategoryHierarchyChangedEvent : DomainEventBase
{
    public override string EventName => "CategoryHierarchyChanged";

    /// <summary>
    /// ID de la categoría
    /// </summary>
    public int CategoryId { get; }

    /// <summary>
    /// Nombre de la categoría
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// ID de la categoría padre anterior (null si no tenía)
    /// </summary>
    public int? PreviousParentId { get; }

    /// <summary>
    /// ID de la nueva categoría padre (null si se convirtió en raíz)
    /// </summary>
    public int? NewParentId { get; }

    public CategoryHierarchyChangedEvent(int categoryId, string name, int? previousParentId, int? newParentId)
    {
        CategoryId = categoryId;
        Name = name ?? throw new ArgumentNullException(nameof(name));
        PreviousParentId = previousParentId;
        NewParentId = newParentId;
    }
}

/// <summary>
/// Evento que se dispara cuando se agrega un artículo a una categoría
/// </summary>
public class ArticleAddedToCategoryEvent : DomainEventBase
{
    public override string EventName => "ArticleAddedToCategory";

    /// <summary>
    /// ID de la categoría
    /// </summary>
    public int CategoryId { get; }

    /// <summary>
    /// Nombre de la categoría
    /// </summary>
    public string CategoryName { get; }

    /// <summary>
    /// ID del artículo agregado
    /// </summary>
    public int ArticleId { get; }

    /// <summary>
    /// SKU del artículo agregado
    /// </summary>
    public string ArticleSKU { get; }

    /// <summary>
    /// Nombre del artículo agregado
    /// </summary>
    public string ArticleName { get; }

    public ArticleAddedToCategoryEvent(int categoryId, string categoryName, int articleId, string articleSKU, string articleName)
    {
        CategoryId = categoryId;
        CategoryName = categoryName ?? throw new ArgumentNullException(nameof(categoryName));
        ArticleId = articleId;
        ArticleSKU = articleSKU ?? throw new ArgumentNullException(nameof(articleSKU));
        ArticleName = articleName ?? throw new ArgumentNullException(nameof(articleName));
    }
}
