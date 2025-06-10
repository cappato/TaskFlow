using PimFlow.Domain.Enums;
using PimFlow.Domain.ValueObjects;
using PimFlow.Domain.Common;
using PimFlow.Domain.Events;

namespace PimFlow.Domain.Entities;

public class Article : AggregateRoot
{
    public int Id { get; set; }

    // Value Objects para encapsular validaciones
    private string _sku = string.Empty;
    private string _name = string.Empty;
    private string _brand = string.Empty;

    public string SKU
    {
        get => _sku;
        set => _sku = value; // Setter simple para Entity Framework
    }

    public string Name
    {
        get => _name;
        set => _name = value; // Setter simple para Entity Framework
    }

    public string Brand
    {
        get => _brand;
        set => _brand = value; // Setter simple para Entity Framework
    }

    public string Description { get; set; } = string.Empty;
    public ArticleType Type { get; set; } = ArticleType.Footwear;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsActive { get; set; } = true;

    // Foreign keys
    public int? CategoryId { get; set; }
    public int? SupplierId { get; set; }

    // Navigation properties
    public virtual Category? Category { get; set; }
    public virtual User? Supplier { get; set; }
    public virtual ICollection<ArticleAttributeValue> AttributeValues { get; set; } = new List<ArticleAttributeValue>();
    public virtual ICollection<ArticleVariant> Variants { get; set; } = new List<ArticleVariant>();

    /// <summary>
    /// Métodos de negocio que usan Value Objects para validación
    /// </summary>
    public Result SetSKU(string sku)
    {
        if (!ValueObjects.SKU.IsValid(sku))
            return Result.Failure("SKU debe contener solo letras mayúsculas y números, entre 3 y 50 caracteres");

        _sku = ValueObjects.SKU.Create(sku).Value;
        return Result.Success();
    }

    public Result SetName(string name)
    {
        if (!ProductName.IsValid(name))
            return Result.Failure("El nombre debe tener entre 2 y 200 caracteres");

        _name = ProductName.Create(name).Value;
        return Result.Success();
    }

    public Result SetBrand(string brand)
    {
        if (!ValueObjects.Brand.IsValid(brand))
            return Result.Failure("La marca debe tener entre 2 y 100 caracteres");

        _brand = ValueObjects.Brand.Create(brand).Value;
        return Result.Success();
    }

    /// <summary>
    /// Factory method para crear un artículo válido
    /// </summary>
    public static Result<Article> Create(string sku, string name, string brand, ArticleType type, string description = "")
    {
        var article = new Article
        {
            Description = description,
            Type = type,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        var skuResult = article.SetSKU(sku);
        if (skuResult.IsFailure)
            return Result.Failure<Article>(skuResult.Error);

        var nameResult = article.SetName(name);
        if (nameResult.IsFailure)
            return Result.Failure<Article>(nameResult.Error);

        var brandResult = article.SetBrand(brand);
        if (brandResult.IsFailure)
            return Result.Failure<Article>(brandResult.Error);

        return Result.Success(article);
    }

    /// <summary>
    /// Marca el artículo como eliminado (soft delete) y publica evento
    /// </summary>
    public Result MarkAsDeleted(string reason = "Manual deletion")
    {
        if (!IsActive)
            return Result.Failure("El artículo ya está eliminado");

        IsActive = false;
        UpdatedAt = DateTime.UtcNow;

        // Publicar evento de dominio
        AddDomainEvent(new ArticleDeletedEvent(Id, SKU, Name, reason));

        return Result.Success();
    }

    /// <summary>
    /// Cambia la categoría del artículo y publica evento
    /// </summary>
    public Result ChangeCategoryTo(int? newCategoryId)
    {
        var previousCategoryId = CategoryId;

        if (previousCategoryId == newCategoryId)
            return Result.Success(); // No hay cambio

        CategoryId = newCategoryId;
        UpdatedAt = DateTime.UtcNow;

        // Publicar evento de dominio
        AddDomainEvent(new ArticleCategoryChangedEvent(Id, SKU, previousCategoryId, newCategoryId));

        return Result.Success();
    }

    /// <summary>
    /// Actualiza el artículo y publica evento con campos modificados
    /// </summary>
    public Result UpdateWith(string? name = null, string? description = null, string? brand = null,
                           ArticleType? type = null, List<string>? modifiedFields = null)
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

        if (!string.IsNullOrEmpty(brand) && brand != Brand)
        {
            var brandResult = SetBrand(brand);
            if (brandResult.IsFailure)
                return brandResult;
            fieldsChanged.Add("Brand");
        }

        if (type.HasValue && type.Value != Type)
        {
            Type = type.Value;
            fieldsChanged.Add("Type");
        }

        if (fieldsChanged.Any())
        {
            UpdatedAt = DateTime.UtcNow;

            // Publicar evento de dominio
            AddDomainEvent(new ArticleUpdatedEvent(Id, SKU, Name, fieldsChanged));
        }

        return Result.Success();
    }

    /// <summary>
    /// Método interno para publicar evento de creación (llamado después de persistir)
    /// </summary>
    internal void PublishCreatedEvent()
    {
        AddDomainEvent(new ArticleCreatedEvent(Id, SKU, Name, Type, Brand, CategoryId));
    }
}
