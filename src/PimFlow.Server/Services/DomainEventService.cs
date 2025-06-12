using Microsoft.Extensions.Logging;
using PimFlow.Domain.Common;


namespace PimFlow.Server.Services;

/// <summary>
/// Servicio para manejar la publicación de eventos de dominio
/// Se encarga de extraer eventos de aggregates y publicarlos
/// </summary>
public interface IDomainEventService
{
    /// <summary>
    /// Publica todos los eventos pendientes de un aggregate root
    /// </summary>
    Task PublishEventsAsync(AggregateRoot aggregateRoot, CancellationToken cancellationToken = default);

    /// <summary>
    /// Publica todos los eventos pendientes de múltiples aggregate roots
    /// </summary>
    Task PublishEventsAsync(IEnumerable<AggregateRoot> aggregateRoots, CancellationToken cancellationToken = default);
}

/// <summary>
/// Implementación del servicio de eventos de dominio
/// </summary>
public class DomainEventService : IDomainEventService
{
    private readonly IDomainEventDispatcher _eventDispatcher;
    private readonly ILogger<DomainEventService> _logger;

    public DomainEventService(IDomainEventDispatcher eventDispatcher, ILogger<DomainEventService> logger)
    {
        _eventDispatcher = eventDispatcher ?? throw new ArgumentNullException(nameof(eventDispatcher));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Publica todos los eventos pendientes de un aggregate root
    /// </summary>
    public async Task PublishEventsAsync(AggregateRoot aggregateRoot, CancellationToken cancellationToken = default)
    {
        if (aggregateRoot == null)
        {
            _logger.LogWarning("Attempted to publish events from null aggregate root");
            return;
        }

        if (!aggregateRoot.HasDomainEvents)
        {
            _logger.LogDebug("No domain events to publish for aggregate {AggregateType}", 
                aggregateRoot.GetType().Name);
            return;
        }

        var events = aggregateRoot.DomainEvents.ToList();
        
        _logger.LogInformation("Publishing {EventCount} domain events from aggregate {AggregateType}",
            events.Count, aggregateRoot.GetType().Name);

        try
        {
            // Publicar eventos
            await _eventDispatcher.PublishAsync(events, cancellationToken);

            // Limpiar eventos después de publicar exitosamente
            aggregateRoot.ClearDomainEvents();

            _logger.LogInformation("Successfully published and cleared {EventCount} domain events from aggregate {AggregateType}",
                events.Count, aggregateRoot.GetType().Name);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error publishing domain events from aggregate {AggregateType}. Events will remain in aggregate.",
                aggregateRoot.GetType().Name);
            throw;
        }
    }

    /// <summary>
    /// Publica todos los eventos pendientes de múltiples aggregate roots
    /// </summary>
    public async Task PublishEventsAsync(IEnumerable<AggregateRoot> aggregateRoots, CancellationToken cancellationToken = default)
    {
        if (aggregateRoots == null)
        {
            _logger.LogWarning("Attempted to publish events from null aggregate roots collection");
            return;
        }

        var aggregatesList = aggregateRoots.ToList();
        
        if (!aggregatesList.Any())
        {
            _logger.LogDebug("No aggregate roots provided for event publishing");
            return;
        }

        var aggregatesWithEvents = aggregatesList.Where(ar => ar.HasDomainEvents).ToList();
        
        if (!aggregatesWithEvents.Any())
        {
            _logger.LogDebug("No domain events to publish from {AggregateCount} aggregates", aggregatesList.Count);
            return;
        }

        _logger.LogInformation("Publishing domain events from {AggregateCount} aggregates with events",
            aggregatesWithEvents.Count);

        var tasks = aggregatesWithEvents.Select(aggregate => PublishEventsAsync(aggregate, cancellationToken));

        try
        {
            await Task.WhenAll(tasks);
            _logger.LogInformation("Successfully published domain events from all {AggregateCount} aggregates",
                aggregatesWithEvents.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error publishing domain events from multiple aggregates");
            throw;
        }
    }
}
