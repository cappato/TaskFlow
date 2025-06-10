namespace PimFlow.Contracts.Common;

/// <summary>
/// Interfaz para el patrón Result que permite desacoplar Shared de Domain
/// Define el contrato básico para operaciones que pueden fallar
/// </summary>
public interface IResult
{
    bool IsSuccess { get; }
    bool IsFailure { get; }
    string Error { get; }
}

/// <summary>
/// Interfaz para Result con valor tipado
/// </summary>
/// <typeparam name="T">Tipo del valor retornado</typeparam>
public interface IResult<out T> : IResult
{
    T Value { get; }
}
