using Microsoft.Extensions.Logging;
using PimFlow.Domain.Events;

namespace PimFlow.Server.Events.Handlers;

/// <summary>
/// Handler para eventos de creación de categorías
/// </summary>
public class CategoryCreatedEventHandler : IDomainEventHandler<CategoryCreatedEvent>
{
    private readonly ILogger<CategoryCreatedEventHandler> _logger;

    public CategoryCreatedEventHandler(ILogger<CategoryCreatedEventHandler> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task HandleAsync(CategoryCreatedEvent domainEvent, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Category created: {Name} (ID: {CategoryId}, Parent: {ParentCategoryId})",
            domainEvent.Name, domainEvent.CategoryId, domainEvent.ParentCategoryId);

        // Aquí se pueden agregar acciones como:
        // - Actualizar jerarquías de navegación
        // - Invalidar caches de categorías
        // - Notificar a sistemas de recomendación
        // - Generar eventos para sistemas externos
        // - Etc.

        await Task.CompletedTask;
    }
}

/// <summary>
/// Handler para eventos de actualización de categorías
/// </summary>
public class CategoryUpdatedEventHandler : IDomainEventHandler<CategoryUpdatedEvent>
{
    private readonly ILogger<CategoryUpdatedEventHandler> _logger;

    public CategoryUpdatedEventHandler(ILogger<CategoryUpdatedEventHandler> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task HandleAsync(CategoryUpdatedEvent domainEvent, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Category updated: {Name} (ID: {CategoryId}). Modified fields: {ModifiedFields}",
            domainEvent.Name, domainEvent.CategoryId, string.Join(", ", domainEvent.ModifiedFields));

        // Aquí se pueden agregar acciones específicas según los campos modificados
        await Task.CompletedTask;
    }
}

/// <summary>
/// Handler para eventos de eliminación de categorías
/// </summary>
public class CategoryDeletedEventHandler : IDomainEventHandler<CategoryDeletedEvent>
{
    private readonly ILogger<CategoryDeletedEventHandler> _logger;

    public CategoryDeletedEventHandler(ILogger<CategoryDeletedEventHandler> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task HandleAsync(CategoryDeletedEvent domainEvent, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Category deleted: {Name} (ID: {CategoryId}). {DeletionSummary}. Reason: {Reason}",
            domainEvent.Name, domainEvent.CategoryId, domainEvent.DeletionSummary, domainEvent.Reason);

        // Aquí se pueden agregar acciones como:
        // - Reorganizar jerarquías
        // - Reasignar artículos huérfanos
        // - Limpiar caches
        // - Auditoría de eliminación
        // - Etc.

        await Task.CompletedTask;
    }
}

/// <summary>
/// Handler para eventos de cambio de jerarquía de categorías
/// </summary>
public class CategoryHierarchyChangedEventHandler : IDomainEventHandler<CategoryHierarchyChangedEvent>
{
    private readonly ILogger<CategoryHierarchyChangedEventHandler> _logger;

    public CategoryHierarchyChangedEventHandler(ILogger<CategoryHierarchyChangedEventHandler> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task HandleAsync(CategoryHierarchyChangedEvent domainEvent, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Category hierarchy changed: {Name} (ID: {CategoryId}) moved from parent {PreviousParentId} to {NewParentId}",
            domainEvent.Name, domainEvent.CategoryId, domainEvent.PreviousParentId, domainEvent.NewParentId);

        // Aquí se pueden agregar acciones como:
        // - Recalcular rutas de navegación
        // - Actualizar breadcrumbs
        // - Invalidar caches de jerarquía
        // - Notificar cambios a frontend
        // - Etc.

        await Task.CompletedTask;
    }
}

/// <summary>
/// Handler para eventos de artículos agregados a categorías
/// </summary>
public class ArticleAddedToCategoryEventHandler : IDomainEventHandler<ArticleAddedToCategoryEvent>
{
    private readonly ILogger<ArticleAddedToCategoryEventHandler> _logger;

    public ArticleAddedToCategoryEventHandler(ILogger<ArticleAddedToCategoryEventHandler> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task HandleAsync(ArticleAddedToCategoryEvent domainEvent, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Article {ArticleSKU} - {ArticleName} (ID: {ArticleId}) added to category {CategoryName} (ID: {CategoryId})",
            domainEvent.ArticleSKU, domainEvent.ArticleName, domainEvent.ArticleId, 
            domainEvent.CategoryName, domainEvent.CategoryId);

        // Aquí se pueden agregar acciones como:
        // - Actualizar contadores de categorías
        // - Recalcular estadísticas
        // - Invalidar caches de categorías
        // - Notificar a sistemas de recomendación
        // - Etc.

        await Task.CompletedTask;
    }
}
