using PimFlow.Domain.Article.Enums;
using PimFlow.Domain.Common;

namespace PimFlow.Domain.Article;

/// <summary>
/// Evento que se dispara cuando se crea un nuevo artículo
/// </summary>
public class ArticleCreatedEvent : DomainEventBase
{
    public override string EventName => "ArticleCreated";

    /// <summary>
    /// ID del artículo creado
    /// </summary>
    public int ArticleId { get; }

    /// <summary>
    /// SKU del artículo creado
    /// </summary>
    public string SKU { get; }

    /// <summary>
    /// Nombre del artículo creado
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Tipo del artículo creado
    /// </summary>
    public ArticleType Type { get; }

    /// <summary>
    /// Marca del artículo creado
    /// </summary>
    public string Brand { get; }

    /// <summary>
    /// ID de la categoría (si tiene)
    /// </summary>
    public int? CategoryId { get; }

    public ArticleCreatedEvent(int articleId, string sku, string name, ArticleType type, string brand, int? categoryId = null)
    {
        ArticleId = articleId;
        SKU = sku ?? throw new ArgumentNullException(nameof(sku));
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Type = type;
        Brand = brand ?? throw new ArgumentNullException(nameof(brand));
        CategoryId = categoryId;
    }
}

/// <summary>
/// Evento que se dispara cuando se actualiza un artículo
/// </summary>
public class ArticleUpdatedEvent : DomainEventBase
{
    public override string EventName => "ArticleUpdated";

    /// <summary>
    /// ID del artículo actualizado
    /// </summary>
    public int ArticleId { get; }

    /// <summary>
    /// SKU del artículo
    /// </summary>
    public string SKU { get; }

    /// <summary>
    /// Nombre actualizado del artículo
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Campos que fueron modificados
    /// </summary>
    public IReadOnlyList<string> ModifiedFields { get; }

    public ArticleUpdatedEvent(int articleId, string sku, string name, IEnumerable<string> modifiedFields)
    {
        ArticleId = articleId;
        SKU = sku ?? throw new ArgumentNullException(nameof(sku));
        Name = name ?? throw new ArgumentNullException(nameof(name));
        ModifiedFields = modifiedFields?.ToList().AsReadOnly() ?? throw new ArgumentNullException(nameof(modifiedFields));
    }
}

/// <summary>
/// Evento que se dispara cuando se elimina (desactiva) un artículo
/// </summary>
public class ArticleDeletedEvent : DomainEventBase
{
    public override string EventName => "ArticleDeleted";

    /// <summary>
    /// ID del artículo eliminado
    /// </summary>
    public int ArticleId { get; }

    /// <summary>
    /// SKU del artículo eliminado
    /// </summary>
    public string SKU { get; }

    /// <summary>
    /// Nombre del artículo eliminado
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Razón de la eliminación
    /// </summary>
    public string Reason { get; }

    public ArticleDeletedEvent(int articleId, string sku, string name, string reason = "Manual deletion")
    {
        ArticleId = articleId;
        SKU = sku ?? throw new ArgumentNullException(nameof(sku));
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Reason = reason ?? "Manual deletion";
    }
}

/// <summary>
/// Evento que se dispara cuando se cambia la categoría de un artículo
/// </summary>
public class ArticleCategoryChangedEvent : DomainEventBase
{
    public override string EventName => "ArticleCategoryChanged";

    /// <summary>
    /// ID del artículo
    /// </summary>
    public int ArticleId { get; }

    /// <summary>
    /// SKU del artículo
    /// </summary>
    public string SKU { get; }

    /// <summary>
    /// ID de la categoría anterior (null si no tenía)
    /// </summary>
    public int? PreviousCategoryId { get; }

    /// <summary>
    /// ID de la nueva categoría (null si se removió)
    /// </summary>
    public int? NewCategoryId { get; }

    public ArticleCategoryChangedEvent(int articleId, string sku, int? previousCategoryId, int? newCategoryId)
    {
        ArticleId = articleId;
        SKU = sku ?? throw new ArgumentNullException(nameof(sku));
        PreviousCategoryId = previousCategoryId;
        NewCategoryId = newCategoryId;
    }
}
