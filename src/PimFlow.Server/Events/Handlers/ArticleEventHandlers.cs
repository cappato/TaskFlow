using Microsoft.Extensions.Logging;
using PimFlow.Domain.Events;

namespace PimFlow.Server.Events.Handlers;

/// <summary>
/// Handler para eventos de creación de artículos
/// Ejemplo de cómo manejar eventos de dominio para logging, notificaciones, etc.
/// </summary>
public class ArticleCreatedEventHandler : IDomainEventHandler<ArticleCreatedEvent>
{
    private readonly ILogger<ArticleCreatedEventHandler> _logger;

    public ArticleCreatedEventHandler(ILogger<ArticleCreatedEventHandler> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task HandleAsync(ArticleCreatedEvent domainEvent, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Article created: {SKU} - {Name} (ID: {ArticleId}, Type: {Type}, Brand: {Brand})",
            domainEvent.SKU, domainEvent.Name, domainEvent.ArticleId, domainEvent.Type, domainEvent.Brand);

        // Aquí se pueden agregar otras acciones como:
        // - Enviar notificaciones
        // - Actualizar caches
        // - Sincronizar con sistemas externos
        // - Generar reportes
        // - Etc.

        await Task.CompletedTask; // Simular trabajo asíncrono
    }
}

/// <summary>
/// Handler para eventos de actualización de artículos
/// </summary>
public class ArticleUpdatedEventHandler : IDomainEventHandler<ArticleUpdatedEvent>
{
    private readonly ILogger<ArticleUpdatedEventHandler> _logger;

    public ArticleUpdatedEventHandler(ILogger<ArticleUpdatedEventHandler> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task HandleAsync(ArticleUpdatedEvent domainEvent, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Article updated: {SKU} - {Name} (ID: {ArticleId}). Modified fields: {ModifiedFields}",
            domainEvent.SKU, domainEvent.Name, domainEvent.ArticleId, string.Join(", ", domainEvent.ModifiedFields));

        // Aquí se pueden agregar acciones específicas según los campos modificados
        // Por ejemplo, si cambió el nombre, actualizar índices de búsqueda
        // Si cambió la categoría, recalcular estadísticas, etc.

        await Task.CompletedTask;
    }
}

/// <summary>
/// Handler para eventos de eliminación de artículos
/// </summary>
public class ArticleDeletedEventHandler : IDomainEventHandler<ArticleDeletedEvent>
{
    private readonly ILogger<ArticleDeletedEventHandler> _logger;

    public ArticleDeletedEventHandler(ILogger<ArticleDeletedEventHandler> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task HandleAsync(ArticleDeletedEvent domainEvent, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Article deleted: {SKU} - {Name} (ID: {ArticleId}). Reason: {Reason}",
            domainEvent.SKU, domainEvent.Name, domainEvent.ArticleId, domainEvent.Reason);

        // Aquí se pueden agregar acciones como:
        // - Archivar datos relacionados
        // - Limpiar caches
        // - Notificar a sistemas dependientes
        // - Generar auditoría
        // - Etc.

        await Task.CompletedTask;
    }
}

/// <summary>
/// Handler para eventos de cambio de categoría de artículos
/// </summary>
public class ArticleCategoryChangedEventHandler : IDomainEventHandler<ArticleCategoryChangedEvent>
{
    private readonly ILogger<ArticleCategoryChangedEventHandler> _logger;

    public ArticleCategoryChangedEventHandler(ILogger<ArticleCategoryChangedEventHandler> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task HandleAsync(ArticleCategoryChangedEvent domainEvent, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Article category changed: {SKU} (ID: {ArticleId}) moved from category {PreviousCategoryId} to {NewCategoryId}",
            domainEvent.SKU, domainEvent.ArticleId, domainEvent.PreviousCategoryId, domainEvent.NewCategoryId);

        // Aquí se pueden agregar acciones como:
        // - Recalcular estadísticas de categorías
        // - Actualizar índices de búsqueda
        // - Notificar cambios a sistemas de recomendación
        // - Etc.

        await Task.CompletedTask;
    }
}
