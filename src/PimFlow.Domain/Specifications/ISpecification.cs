namespace PimFlow.Domain.Specifications;

/// <summary>
/// Interfaz base para el patrón Specification
/// Permite encapsular reglas de negocio de forma reutilizable
///
/// LISKOV SUBSTITUTION PRINCIPLE (LSP) CONTRACTS:
/// - Todas las implementaciones DEBEN mantener el mismo comportamiento para IsSatisfiedBy
/// - ErrorMessage NUNCA debe ser null o vacío cuando IsSatisfiedBy retorna false
/// - Las operaciones And, Or, Not DEBEN preservar la semántica lógica
/// - Las implementaciones NO deben lanzar excepciones en condiciones normales
/// </summary>
/// <typeparam name="T">Tipo de entidad a evaluar</typeparam>
public interface ISpecification<T>
{
    /// <summary>
    /// Evalúa si la entidad cumple con la especificación
    ///
    /// LSP CONTRACT:
    /// - PRECONDICIÓN: entity no debe ser null (todas las implementaciones deben validar esto)
    /// - POSTCONDICIÓN: Retorna true si y solo si la entidad cumple TODOS los criterios
    /// - INVARIANTE: El resultado debe ser determinístico para la misma entidad
    /// </summary>
    /// <param name="entity">Entidad a evaluar</param>
    /// <returns>True si cumple la especificación</returns>
    /// <exception cref="ArgumentNullException">Cuando entity es null</exception>
    bool IsSatisfiedBy(T entity);

    /// <summary>
    /// Mensaje de error cuando la especificación no se cumple
    ///
    /// LSP CONTRACT:
    /// - POSTCONDICIÓN: NUNCA debe retornar null o string vacío
    /// - INVARIANTE: Debe ser descriptivo y útil para el usuario
    /// </summary>
    string ErrorMessage { get; }

    /// <summary>
    /// Combina dos specifications con operador AND
    ///
    /// LSP CONTRACT:
    /// - PRECONDICIÓN: other no debe ser null
    /// - POSTCONDICIÓN: Retorna especificación que es true solo cuando AMBAS son true
    /// - INVARIANTE: Operación debe ser conmutativa (A.And(B) ≡ B.And(A))
    /// </summary>
    ISpecification<T> And(ISpecification<T> other);

    /// <summary>
    /// Combina dos specifications con operador OR
    ///
    /// LSP CONTRACT:
    /// - PRECONDICIÓN: other no debe ser null
    /// - POSTCONDICIÓN: Retorna especificación que es true cuando AL MENOS UNA es true
    /// - INVARIANTE: Operación debe ser conmutativa (A.Or(B) ≡ B.Or(A))
    /// </summary>
    ISpecification<T> Or(ISpecification<T> other);

    /// <summary>
    /// Niega la specification actual
    ///
    /// LSP CONTRACT:
    /// - POSTCONDICIÓN: Retorna especificación que es true cuando esta es false
    /// - INVARIANTE: Doble negación debe retornar al estado original (spec.Not().Not() ≡ spec)
    /// </summary>
    ISpecification<T> Not();
}

/// <summary>
/// Clase base abstracta para implementar specifications
/// Implementa los contratos LSP definidos en ISpecification
/// </summary>
/// <typeparam name="T">Tipo de entidad a evaluar</typeparam>
public abstract class Specification<T> : ISpecification<T>
{
    /// <summary>
    /// Implementación base que valida contratos LSP
    /// </summary>
    public virtual bool IsSatisfiedBy(T entity)
    {
        // LSP CONTRACT: Validar precondición
        if (entity == null)
            throw new ArgumentNullException(nameof(entity), "Entity cannot be null");

        return IsSatisfiedByCore(entity);
    }

    /// <summary>
    /// Método abstracto para implementar la lógica específica
    /// Las subclases deben implementar este método en lugar de IsSatisfiedBy
    /// </summary>
    protected abstract bool IsSatisfiedByCore(T entity);

    /// <summary>
    /// LSP CONTRACT: ErrorMessage nunca debe ser null o vacío
    /// </summary>
    public abstract string ErrorMessage { get; }

    /// <summary>
    /// Combina dos specifications con operador AND
    /// Implementa contratos LSP para operaciones lógicas
    /// </summary>
    public virtual ISpecification<T> And(ISpecification<T> other)
    {
        // LSP CONTRACT: Validar precondición
        if (other == null)
            throw new ArgumentNullException(nameof(other), "Other specification cannot be null");

        return new AndSpecification<T>(this, other);
    }

    /// <summary>
    /// Combina dos specifications con operador OR
    /// Implementa contratos LSP para operaciones lógicas
    /// </summary>
    public virtual ISpecification<T> Or(ISpecification<T> other)
    {
        // LSP CONTRACT: Validar precondición
        if (other == null)
            throw new ArgumentNullException(nameof(other), "Other specification cannot be null");

        return new OrSpecification<T>(this, other);
    }

    /// <summary>
    /// Niega la specification actual
    /// Implementa contratos LSP para negación
    /// </summary>
    public virtual ISpecification<T> Not()
    {
        return new NotSpecification<T>(this);
    }
}

/// <summary>
/// Specification que combina dos specifications con AND
/// Cumple contratos LSP: preserva semántica lógica AND
/// </summary>
internal class AndSpecification<T> : Specification<T>
{
    private readonly ISpecification<T> _left;
    private readonly ISpecification<T> _right;

    public AndSpecification(ISpecification<T> left, ISpecification<T> right)
    {
        _left = left ?? throw new ArgumentNullException(nameof(left));
        _right = right ?? throw new ArgumentNullException(nameof(right));
    }

    /// <summary>
    /// LSP CONTRACT: Implementa lógica específica sin validación de null (ya hecha en base)
    /// </summary>
    protected override bool IsSatisfiedByCore(T entity)
    {
        return _left.IsSatisfiedBy(entity) && _right.IsSatisfiedBy(entity);
    }

    /// <summary>
    /// LSP CONTRACT: ErrorMessage nunca es null o vacío
    /// </summary>
    public override string ErrorMessage =>
        $"{_left.ErrorMessage} y {_right.ErrorMessage}";
}

/// <summary>
/// Specification que combina dos specifications con OR
/// Cumple contratos LSP: preserva semántica lógica OR
/// </summary>
internal class OrSpecification<T> : Specification<T>
{
    private readonly ISpecification<T> _left;
    private readonly ISpecification<T> _right;

    public OrSpecification(ISpecification<T> left, ISpecification<T> right)
    {
        _left = left ?? throw new ArgumentNullException(nameof(left));
        _right = right ?? throw new ArgumentNullException(nameof(right));
    }

    /// <summary>
    /// LSP CONTRACT: Implementa lógica específica sin validación de null (ya hecha en base)
    /// </summary>
    protected override bool IsSatisfiedByCore(T entity)
    {
        return _left.IsSatisfiedBy(entity) || _right.IsSatisfiedBy(entity);
    }

    /// <summary>
    /// LSP CONTRACT: ErrorMessage nunca es null o vacío
    /// </summary>
    public override string ErrorMessage =>
        $"{_left.ErrorMessage} o {_right.ErrorMessage}";
}

/// <summary>
/// Specification que niega otra specification
/// Cumple contratos LSP: preserva semántica lógica NOT
/// </summary>
internal class NotSpecification<T> : Specification<T>
{
    private readonly ISpecification<T> _specification;

    public NotSpecification(ISpecification<T> specification)
    {
        _specification = specification ?? throw new ArgumentNullException(nameof(specification));
    }

    /// <summary>
    /// LSP CONTRACT: Implementa lógica específica sin validación de null (ya hecha en base)
    /// </summary>
    protected override bool IsSatisfiedByCore(T entity)
    {
        return !_specification.IsSatisfiedBy(entity);
    }

    /// <summary>
    /// LSP CONTRACT: ErrorMessage nunca es null o vacío
    /// </summary>
    public override string ErrorMessage =>
        $"No debe cumplir: {_specification.ErrorMessage}";
}
