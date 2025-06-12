using PimFlow.Domain.Common;

namespace PimFlow.Domain.Common;

/// <summary>
/// Clase base abstracta para Aggregate Roots en DDD
/// Maneja la colección de eventos de dominio y proporciona métodos para gestionarlos
/// </summary>
public abstract class AggregateRoot
{
    private readonly List<IDomainEvent> _domainEvents = new();

    /// <summary>
    /// Eventos de dominio pendientes de publicación
    /// </summary>
    public IReadOnlyList<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    /// <summary>
    /// Indica si el aggregate tiene eventos pendientes
    /// </summary>
    public bool HasDomainEvents => _domainEvents.Any();

    /// <summary>
    /// Agrega un evento de dominio a la colección
    /// </summary>
    /// <param name="domainEvent">El evento a agregar</param>
    protected void AddDomainEvent(IDomainEvent domainEvent)
    {
        if (domainEvent == null)
            throw new ArgumentNullException(nameof(domainEvent));

        _domainEvents.Add(domainEvent);
    }

    /// <summary>
    /// Agrega múltiples eventos de dominio a la colección
    /// </summary>
    /// <param name="domainEvents">Los eventos a agregar</param>
    protected void AddDomainEvents(IEnumerable<IDomainEvent> domainEvents)
    {
        if (domainEvents == null)
            throw new ArgumentNullException(nameof(domainEvents));

        _domainEvents.AddRange(domainEvents);
    }

    /// <summary>
    /// Limpia todos los eventos de dominio
    /// Típicamente llamado después de que los eventos han sido publicados
    /// </summary>
    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }

    /// <summary>
    /// Remueve un evento específico de la colección
    /// </summary>
    /// <param name="domainEvent">El evento a remover</param>
    /// <returns>True si el evento fue removido, false si no se encontró</returns>
    public bool RemoveDomainEvent(IDomainEvent domainEvent)
    {
        return _domainEvents.Remove(domainEvent);
    }

    /// <summary>
    /// Obtiene todos los eventos de un tipo específico
    /// </summary>
    /// <typeparam name="T">Tipo de evento</typeparam>
    /// <returns>Eventos del tipo especificado</returns>
    public IEnumerable<T> GetDomainEventsOfType<T>() where T : IDomainEvent
    {
        return _domainEvents.OfType<T>();
    }

    /// <summary>
    /// Verifica si hay eventos de un tipo específico
    /// </summary>
    /// <typeparam name="T">Tipo de evento</typeparam>
    /// <returns>True si hay eventos del tipo especificado</returns>
    public bool HasDomainEventsOfType<T>() where T : IDomainEvent
    {
        return _domainEvents.OfType<T>().Any();
    }
}
