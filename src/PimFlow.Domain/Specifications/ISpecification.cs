namespace PimFlow.Domain.Specifications;

/// <summary>
/// Interfaz base para el patrón Specification
/// Permite encapsular reglas de negocio de forma reutilizable
/// </summary>
/// <typeparam name="T">Tipo de entidad a evaluar</typeparam>
public interface ISpecification<T>
{
    /// <summary>
    /// Evalúa si la entidad cumple con la especificación
    /// </summary>
    /// <param name="entity">Entidad a evaluar</param>
    /// <returns>True si cumple la especificación</returns>
    bool IsSatisfiedBy(T entity);

    /// <summary>
    /// Mensaje de error cuando la especificación no se cumple
    /// </summary>
    string ErrorMessage { get; }

    /// <summary>
    /// Combina dos specifications con operador AND
    /// </summary>
    ISpecification<T> And(ISpecification<T> other);

    /// <summary>
    /// Combina dos specifications con operador OR
    /// </summary>
    ISpecification<T> Or(ISpecification<T> other);

    /// <summary>
    /// Niega la specification actual
    /// </summary>
    ISpecification<T> Not();
}

/// <summary>
/// Clase base abstracta para implementar specifications
/// </summary>
/// <typeparam name="T">Tipo de entidad a evaluar</typeparam>
public abstract class Specification<T> : ISpecification<T>
{
    public abstract bool IsSatisfiedBy(T entity);
    public abstract string ErrorMessage { get; }

    /// <summary>
    /// Combina dos specifications con operador AND
    /// </summary>
    public virtual ISpecification<T> And(ISpecification<T> other)
    {
        return new AndSpecification<T>(this, other);
    }

    /// <summary>
    /// Combina dos specifications con operador OR
    /// </summary>
    public virtual ISpecification<T> Or(ISpecification<T> other)
    {
        return new OrSpecification<T>(this, other);
    }

    /// <summary>
    /// Niega la specification actual
    /// </summary>
    public virtual ISpecification<T> Not()
    {
        return new NotSpecification<T>(this);
    }
}

/// <summary>
/// Specification que combina dos specifications con AND
/// </summary>
internal class AndSpecification<T> : Specification<T>
{
    private readonly ISpecification<T> _left;
    private readonly ISpecification<T> _right;

    public AndSpecification(ISpecification<T> left, ISpecification<T> right)
    {
        _left = left;
        _right = right;
    }

    public override bool IsSatisfiedBy(T entity)
    {
        return _left.IsSatisfiedBy(entity) && _right.IsSatisfiedBy(entity);
    }

    public override string ErrorMessage =>
        $"{_left.ErrorMessage} y {_right.ErrorMessage}";
}

/// <summary>
/// Specification que combina dos specifications con OR
/// </summary>
internal class OrSpecification<T> : Specification<T>
{
    private readonly ISpecification<T> _left;
    private readonly ISpecification<T> _right;

    public OrSpecification(ISpecification<T> left, ISpecification<T> right)
    {
        _left = left;
        _right = right;
    }

    public override bool IsSatisfiedBy(T entity)
    {
        return _left.IsSatisfiedBy(entity) || _right.IsSatisfiedBy(entity);
    }

    public override string ErrorMessage => 
        $"{_left.ErrorMessage} o {_right.ErrorMessage}";
}

/// <summary>
/// Specification que niega otra specification
/// </summary>
internal class NotSpecification<T> : Specification<T>
{
    private readonly ISpecification<T> _specification;

    public NotSpecification(ISpecification<T> specification)
    {
        _specification = specification;
    }

    public override bool IsSatisfiedBy(T entity)
    {
        return !_specification.IsSatisfiedBy(entity);
    }

    public override string ErrorMessage => 
        $"No debe cumplir: {_specification.ErrorMessage}";
}
