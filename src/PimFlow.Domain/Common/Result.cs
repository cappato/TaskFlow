using PimFlow.Contracts.Common;

namespace PimFlow.Domain.Common;

/// <summary>
/// Representa el resultado de una operación que puede fallar
/// Implementa el patrón Result para manejo de errores sin excepciones
/// </summary>
public class Result : IResult
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public string Error { get; }

    protected Result(bool isSuccess, string error)
    {
        if (isSuccess && !string.IsNullOrEmpty(error))
            throw new InvalidOperationException("Un resultado exitoso no puede tener error");
        
        if (!isSuccess && string.IsNullOrEmpty(error))
            throw new InvalidOperationException("Un resultado fallido debe tener un mensaje de error");

        IsSuccess = isSuccess;
        Error = error;
    }

    /// <summary>
    /// Crea un resultado exitoso
    /// </summary>
    public static Result Success()
    {
        return new Result(true, string.Empty);
    }

    /// <summary>
    /// Crea un resultado fallido con mensaje de error
    /// </summary>
    public static Result Failure(string error)
    {
        return new Result(false, error);
    }

    /// <summary>
    /// Crea un resultado exitoso con valor
    /// </summary>
    public static Result<T> Success<T>(T value)
    {
        return new Result<T>(value, true, string.Empty);
    }

    /// <summary>
    /// Crea un resultado fallido con mensaje de error
    /// </summary>
    public static Result<T> Failure<T>(string error)
    {
        return new Result<T>(default!, false, error);
    }
}

/// <summary>
/// Representa el resultado de una operación que puede fallar y retorna un valor
/// </summary>
/// <typeparam name="T">Tipo del valor retornado</typeparam>
public class Result<T> : Result, IResult<T>
{
    public T Value { get; }

    protected internal Result(T value, bool isSuccess, string error) : base(isSuccess, error)
    {
        Value = value;
    }

    /// <summary>
    /// Transforma el valor del resultado si es exitoso
    /// </summary>
    public Result<TNew> Map<TNew>(Func<T, TNew> mapper)
    {
        if (IsFailure)
            return Result.Failure<TNew>(Error);

        try
        {
            var newValue = mapper(Value);
            return Result.Success(newValue);
        }
        catch (Exception ex)
        {
            return Result.Failure<TNew>(ex.Message);
        }
    }

    /// <summary>
    /// Ejecuta una acción si el resultado es exitoso
    /// </summary>
    public Result<T> OnSuccess(Action<T> action)
    {
        if (IsSuccess)
        {
            action(Value);
        }
        return this;
    }

    /// <summary>
    /// Ejecuta una acción si el resultado es fallido
    /// </summary>
    public Result<T> OnFailure(Action<string> action)
    {
        if (IsFailure)
        {
            action(Error);
        }
        return this;
    }

    /// <summary>
    /// Combina este resultado con otro resultado
    /// </summary>
    public Result<TNew> Bind<TNew>(Func<T, Result<TNew>> func)
    {
        if (IsFailure)
            return Result.Failure<TNew>(Error);

        return func(Value);
    }

    /// <summary>
    /// Obtiene el valor o lanza excepción si es fallido
    /// </summary>
    public T GetValueOrThrow()
    {
        if (IsFailure)
            throw new InvalidOperationException(Error);

        return Value;
    }

    /// <summary>
    /// Obtiene el valor o retorna un valor por defecto
    /// </summary>
    public T GetValueOrDefault(T defaultValue = default!)
    {
        return IsSuccess ? Value : defaultValue;
    }

    /// <summary>
    /// Conversión implícita desde el valor
    /// </summary>
    public static implicit operator Result<T>(T value)
    {
        return Result.Success(value);
    }
}

/// <summary>
/// Extensiones para trabajar con múltiples Results
/// </summary>
public static class ResultExtensions
{
    /// <summary>
    /// Combina múltiples resultados en uno solo
    /// Si alguno falla, retorna el primer error
    /// </summary>
    public static Result Combine(params Result[] results)
    {
        foreach (var result in results)
        {
            if (result.IsFailure)
                return result;
        }

        return Result.Success();
    }

    /// <summary>
    /// Combina múltiples resultados en uno solo
    /// Si alguno falla, retorna todos los errores combinados
    /// </summary>
    public static Result CombineAll(params Result[] results)
    {
        var errors = results
            .Where(r => r.IsFailure)
            .Select(r => r.Error)
            .ToList();

        if (errors.Any())
        {
            return Result.Failure(string.Join("; ", errors));
        }

        return Result.Success();
    }
}
