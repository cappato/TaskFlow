using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PimFlow.Domain.Common;

namespace PimFlow.Server.Events;

/// <summary>
/// Implementación del dispatcher de eventos de dominio
/// Utiliza el service provider para resolver handlers dinámicamente
/// </summary>
public class DomainEventDispatcher : IDomainEventDispatcher
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<DomainEventDispatcher> _logger;

    public DomainEventDispatcher(IServiceProvider serviceProvider, ILogger<DomainEventDispatcher> logger)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Publica un evento de dominio a todos sus manejadores
    /// </summary>
    public async Task PublishAsync(IDomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        if (domainEvent == null)
        {
            _logger.LogWarning("Attempted to publish null domain event");
            return;
        }

        _logger.LogInformation("Publishing domain event: {EventName} (ID: {EventId})", 
            domainEvent.EventName, domainEvent.EventId);

        var eventType = domainEvent.GetType();
        var handlerType = typeof(IDomainEventHandler<>).MakeGenericType(eventType);

        using var scope = _serviceProvider.CreateScope();
        var handlers = scope.ServiceProvider.GetServices(handlerType);

        var tasks = new List<Task>();

        foreach (var handler in handlers)
        {
            try
            {
                var handleMethod = handlerType.GetMethod("HandleAsync");
                if (handleMethod != null)
                {
                    var invokeResult = handleMethod.Invoke(handler, new object[] { domainEvent, cancellationToken });
                    if (invokeResult is Task task)
                    {
                        tasks.Add(task);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error invoking handler {HandlerType} for event {EventName}",
                    handler?.GetType().Name ?? "Unknown", domainEvent.EventName);
            }
        }

        if (tasks.Any())
        {
            try
            {
                await Task.WhenAll(tasks);
                _logger.LogInformation("Successfully published domain event: {EventName} to {HandlerCount} handlers", 
                    domainEvent.EventName, tasks.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error publishing domain event: {EventName}", domainEvent.EventName);
                throw;
            }
        }
        else
        {
            _logger.LogInformation("No handlers found for domain event: {EventName}", domainEvent.EventName);
        }
    }

    /// <summary>
    /// Publica múltiples eventos de dominio
    /// </summary>
    public async Task PublishAsync(IEnumerable<IDomainEvent> domainEvents, CancellationToken cancellationToken = default)
    {
        if (domainEvents == null)
        {
            _logger.LogWarning("Attempted to publish null domain events collection");
            return;
        }

        var eventsList = domainEvents.ToList();
        
        if (!eventsList.Any())
        {
            _logger.LogInformation("No domain events to publish");
            return;
        }

        _logger.LogInformation("Publishing {EventCount} domain events", eventsList.Count);

        var tasks = eventsList.Select(domainEvent => PublishAsync(domainEvent, cancellationToken));

        try
        {
            await Task.WhenAll(tasks);
            _logger.LogInformation("Successfully published all {EventCount} domain events", eventsList.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error publishing domain events batch");
            throw;
        }
    }
}
