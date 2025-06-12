namespace PimFlow.Domain.Common;

/// <summary>
/// Interfaz base para todos los eventos de dominio
/// Los eventos de dominio representan algo importante que ha ocurrido en el dominio
/// </summary>
public interface IDomainEvent
{
    /// <summary>
    /// Identificador único del evento
    /// </summary>
    Guid EventId { get; }
    
    /// <summary>
    /// Fecha y hora cuando ocurrió el evento
    /// </summary>
    DateTime OccurredOn { get; }
    
    /// <summary>
    /// Nombre del evento para identificación y logging
    /// </summary>
    string EventName { get; }
    
    /// <summary>
    /// Versión del evento para evolución del schema
    /// </summary>
    int Version { get; }
}
