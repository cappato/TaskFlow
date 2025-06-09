namespace PimFlow.Shared.Common;

/// <summary>
/// Respuesta estándar de la API que encapsula el resultado y manejo de errores
/// Implementa un patrón consistente para todas las respuestas de la API
/// </summary>
/// <typeparam name="T">Tipo de datos retornados</typeparam>
public class ApiResponse<T>
{
    public bool IsSuccess { get; set; }
    public T? Data { get; set; }
    public string? ErrorMessage { get; set; }
    public List<string> ValidationErrors { get; set; } = new();
    public string? ErrorCode { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Crea una respuesta exitosa con datos
    /// </summary>
    public static ApiResponse<T> Success(T data)
    {
        return new ApiResponse<T>
        {
            IsSuccess = true,
            Data = data
        };
    }

    /// <summary>
    /// Crea una respuesta exitosa sin datos
    /// </summary>
    public static ApiResponse<T> Success()
    {
        return new ApiResponse<T>
        {
            IsSuccess = true
        };
    }

    /// <summary>
    /// Crea una respuesta de error con mensaje
    /// </summary>
    public static ApiResponse<T> Error(string errorMessage, string? errorCode = null)
    {
        return new ApiResponse<T>
        {
            IsSuccess = false,
            ErrorMessage = errorMessage,
            ErrorCode = errorCode
        };
    }

    /// <summary>
    /// Crea una respuesta de error con errores de validación
    /// </summary>
    public static ApiResponse<T> ValidationError(List<string> validationErrors)
    {
        return new ApiResponse<T>
        {
            IsSuccess = false,
            ErrorMessage = "Errores de validación",
            ValidationErrors = validationErrors,
            ErrorCode = "VALIDATION_ERROR"
        };
    }

    /// <summary>
    /// Crea una respuesta de error con un solo error de validación
    /// </summary>
    public static ApiResponse<T> ValidationError(string validationError)
    {
        return ValidationError(new List<string> { validationError });
    }

    /// <summary>
    /// Crea una respuesta de error de recurso no encontrado
    /// </summary>
    public static ApiResponse<T> NotFound(string? message = null)
    {
        return new ApiResponse<T>
        {
            IsSuccess = false,
            ErrorMessage = message ?? "Recurso no encontrado",
            ErrorCode = "NOT_FOUND"
        };
    }

    /// <summary>
    /// Crea una respuesta de error de acceso no autorizado
    /// </summary>
    public static ApiResponse<T> Unauthorized(string? message = null)
    {
        return new ApiResponse<T>
        {
            IsSuccess = false,
            ErrorMessage = message ?? "Acceso no autorizado",
            ErrorCode = "UNAUTHORIZED"
        };
    }

    /// <summary>
    /// Crea una respuesta de error interno del servidor
    /// </summary>
    public static ApiResponse<T> InternalError(string? message = null)
    {
        return new ApiResponse<T>
        {
            IsSuccess = false,
            ErrorMessage = message ?? "Error interno del servidor",
            ErrorCode = "INTERNAL_ERROR"
        };
    }

    /// <summary>
    /// Indica si hay errores de validación
    /// </summary>
    public bool HasValidationErrors => ValidationErrors.Any();

    /// <summary>
    /// Obtiene todos los errores como una lista
    /// </summary>
    public List<string> GetAllErrors()
    {
        var errors = new List<string>();
        
        if (!string.IsNullOrEmpty(ErrorMessage))
            errors.Add(ErrorMessage);
            
        errors.AddRange(ValidationErrors);
        
        return errors;
    }

    /// <summary>
    /// Obtiene todos los errores como un string concatenado
    /// </summary>
    public string GetErrorsAsString(string separator = "; ")
    {
        return string.Join(separator, GetAllErrors());
    }
}

/// <summary>
/// Respuesta de API sin datos específicos
/// </summary>
public class ApiResponse : ApiResponse<object>
{
    /// <summary>
    /// Crea una respuesta exitosa sin datos
    /// </summary>
    public new static ApiResponse Success()
    {
        return new ApiResponse
        {
            IsSuccess = true
        };
    }

    /// <summary>
    /// Crea una respuesta de error con mensaje
    /// </summary>
    public new static ApiResponse Error(string errorMessage, string? errorCode = null)
    {
        return new ApiResponse
        {
            IsSuccess = false,
            ErrorMessage = errorMessage,
            ErrorCode = errorCode
        };
    }

    /// <summary>
    /// Crea una respuesta de error con errores de validación
    /// </summary>
    public new static ApiResponse ValidationError(List<string> validationErrors)
    {
        return new ApiResponse
        {
            IsSuccess = false,
            ErrorMessage = "Errores de validación",
            ValidationErrors = validationErrors,
            ErrorCode = "VALIDATION_ERROR"
        };
    }

    /// <summary>
    /// Crea una respuesta de error con un solo error de validación
    /// </summary>
    public new static ApiResponse ValidationError(string validationError)
    {
        return ValidationError(new List<string> { validationError });
    }
}

/// <summary>
/// Extensiones para trabajar con ApiResponse
/// </summary>
public static class ApiResponseExtensions
{
    /// <summary>
    /// Convierte un Result a ApiResponse
    /// </summary>
    public static ApiResponse<T> ToApiResponse<T>(this PimFlow.Domain.Common.Result<T> result)
    {
        if (result.IsSuccess)
            return ApiResponse<T>.Success(result.Value);
        
        return ApiResponse<T>.Error(result.Error);
    }

    /// <summary>
    /// Convierte un Result a ApiResponse sin datos
    /// </summary>
    public static ApiResponse ToApiResponse(this PimFlow.Domain.Common.Result result)
    {
        if (result.IsSuccess)
            return ApiResponse.Success();
        
        return ApiResponse.Error(result.Error);
    }
}
