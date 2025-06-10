namespace PimFlow.Domain.Events;

/// <summary>
/// Interfaz para manejadores de eventos de dominio
/// Implementa el patrón Observer para reaccionar a eventos
/// </summary>
/// <typeparam name="TEvent">Tipo de evento que maneja</typeparam>
public interface IDomainEventHandler<in TEvent> where TEvent : IDomainEvent
{
    /// <summary>
    /// Maneja el evento de dominio de forma asíncrona
    /// </summary>
    /// <param name="domainEvent">El evento a manejar</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Task que representa la operación asíncrona</returns>
    Task HandleAsync(TEvent domainEvent, CancellationToken cancellationToken = default);
}

/// <summary>
/// Interfaz para el dispatcher de eventos de dominio
/// Coordina la publicación y manejo de eventos
/// </summary>
public interface IDomainEventDispatcher
{
    /// <summary>
    /// Publica un evento de dominio a todos sus manejadores
    /// </summary>
    /// <param name="domainEvent">El evento a publicar</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Task que representa la operación asíncrona</returns>
    Task PublishAsync(IDomainEvent domainEvent, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Publica múltiples eventos de dominio
    /// </summary>
    /// <param name="domainEvents">Los eventos a publicar</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Task que representa la operación asíncrona</returns>
    Task PublishAsync(IEnumerable<IDomainEvent> domainEvents, CancellationToken cancellationToken = default);
}
