namespace PimFlow.Domain.Events;

/// <summary>
/// Clase base abstracta para eventos de dominio
/// Proporciona implementación común para todos los eventos
/// </summary>
public abstract class DomainEventBase : IDomainEvent
{
    /// <summary>
    /// Identificador único del evento
    /// </summary>
    public Guid EventId { get; }
    
    /// <summary>
    /// Fecha y hora cuando ocurrió el evento
    /// </summary>
    public DateTime OccurredOn { get; }
    
    /// <summary>
    /// Nombre del evento para identificación y logging
    /// </summary>
    public abstract string EventName { get; }
    
    /// <summary>
    /// Versión del evento para evolución del schema
    /// </summary>
    public virtual int Version => 1;

    /// <summary>
    /// Constructor protegido para eventos de dominio
    /// </summary>
    protected DomainEventBase()
    {
        EventId = Guid.NewGuid();
        OccurredOn = DateTime.UtcNow;
    }

    /// <summary>
    /// Constructor protegido con fecha específica (para testing)
    /// </summary>
    /// <param name="occurredOn">Fecha específica del evento</param>
    protected DomainEventBase(DateTime occurredOn)
    {
        EventId = Guid.NewGuid();
        OccurredOn = occurredOn;
    }

    /// <summary>
    /// Constructor protegido completo (para testing)
    /// </summary>
    /// <param name="eventId">ID específico del evento</param>
    /// <param name="occurredOn">Fecha específica del evento</param>
    protected DomainEventBase(Guid eventId, DateTime occurredOn)
    {
        EventId = eventId;
        OccurredOn = occurredOn;
    }

    public override string ToString()
    {
        return $"{EventName} (ID: {EventId}, Occurred: {OccurredOn:yyyy-MM-dd HH:mm:ss} UTC)";
    }
}
