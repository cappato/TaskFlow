using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PimFlow.Shared.Common;
using PimFlow.Contracts.Common;
using PimFlow.Server.Services;
using PimFlow.Domain.Common;

namespace PimFlow.Server.Controllers.Base;

/// <summary>
/// Controlador base que proporciona funcionalidad común para todos los controladores de API
/// Incluye manejo estándar de respuestas, logging, validaciones y Domain Events
/// </summary>
[ApiController]
public abstract class BaseApiController : ControllerBase
{
    protected readonly ILogger Logger;
    protected readonly IDomainEventService? DomainEventService;

    protected BaseApiController(ILogger logger, IDomainEventService? domainEventService = null)
    {
        Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        DomainEventService = domainEventService;
    }

    /// <summary>
    /// Crea una respuesta exitosa con datos usando ApiResponse
    /// </summary>
    protected ActionResult<ApiResponse<T>> SuccessResponse<T>(T data)
    {
        Logger.LogDebug("Returning success response with data of type {DataType}", typeof(T).Name);
        return Ok(ApiResponse<T>.Success(data));
    }

    /// <summary>
    /// Crea una respuesta exitosa sin datos usando ApiResponse
    /// </summary>
    protected ActionResult<ApiResponse> SuccessResponse()
    {
        Logger.LogDebug("Returning success response without data");
        return Ok(ApiResponse.Success());
    }

    /// <summary>
    /// Crea una respuesta de error usando ApiResponse
    /// </summary>
    protected ActionResult<ApiResponse<T>> ErrorResponse<T>(string errorMessage, string? errorCode = null)
    {
        Logger.LogWarning("Returning error response: {ErrorMessage} (Code: {ErrorCode})", errorMessage, errorCode);
        return BadRequest(ApiResponse<T>.Error(errorMessage, errorCode));
    }

    /// <summary>
    /// Crea una respuesta de error sin datos usando ApiResponse
    /// </summary>
    protected ActionResult<ApiResponse> ErrorResponse(string errorMessage, string? errorCode = null)
    {
        Logger.LogWarning("Returning error response: {ErrorMessage} (Code: {ErrorCode})", errorMessage, errorCode);
        return BadRequest(ApiResponse.Error(errorMessage, errorCode));
    }

    /// <summary>
    /// Crea una respuesta de recurso no encontrado usando ApiResponse
    /// </summary>
    protected ActionResult<ApiResponse<T>> NotFoundResponse<T>(string? message = null)
    {
        var errorMessage = message ?? "Recurso no encontrado";
        Logger.LogInformation("Returning not found response: {Message}", errorMessage);
        return NotFound(ApiResponse<T>.NotFound(errorMessage));
    }

    /// <summary>
    /// Crea una respuesta de recurso no encontrado sin datos usando ApiResponse
    /// </summary>
    protected ActionResult<ApiResponse> NotFoundResponse(string? message = null)
    {
        var errorMessage = message ?? "Recurso no encontrado";
        Logger.LogInformation("Returning not found response: {Message}", errorMessage);
        return NotFound(ApiResponse.NotFound(errorMessage));
    }

    /// <summary>
    /// Crea una respuesta de errores de validación usando ApiResponse
    /// </summary>
    protected ActionResult<ApiResponse<T>> ValidationErrorResponse<T>(IEnumerable<string> validationErrors)
    {
        var errors = validationErrors.ToList();
        Logger.LogWarning("Returning validation error response with {ErrorCount} errors: {Errors}", 
            errors.Count, string.Join(", ", errors));
        return BadRequest(ApiResponse<T>.ValidationError(errors));
    }

    /// <summary>
    /// Crea una respuesta de errores de validación sin datos usando ApiResponse
    /// </summary>
    protected ActionResult<ApiResponse> ValidationErrorResponse(IEnumerable<string> validationErrors)
    {
        var errors = validationErrors.ToList();
        Logger.LogWarning("Returning validation error response with {ErrorCount} errors: {Errors}", 
            errors.Count, string.Join(", ", errors));
        return BadRequest(ApiResponse.ValidationError(errors));
    }

    /// <summary>
    /// Valida el ModelState y retorna errores de validación si es inválido
    /// </summary>
    protected ActionResult<ApiResponse<T>>? ValidateModelState<T>()
    {
        if (ModelState.IsValid) 
            return null;

        var errors = ModelState
            .SelectMany(x => x.Value?.Errors ?? new Microsoft.AspNetCore.Mvc.ModelBinding.ModelErrorCollection())
            .Select(x => x.ErrorMessage)
            .Where(x => !string.IsNullOrEmpty(x))
            .ToList();

        return ValidationErrorResponse<T>(errors);
    }

    /// <summary>
    /// Valida el ModelState y retorna errores de validación si es inválido (sin datos)
    /// </summary>
    protected ActionResult<ApiResponse>? ValidateModelState()
    {
        if (ModelState.IsValid) 
            return null;

        var errors = ModelState
            .SelectMany(x => x.Value?.Errors ?? new Microsoft.AspNetCore.Mvc.ModelBinding.ModelErrorCollection())
            .Select(x => x.ErrorMessage)
            .Where(x => !string.IsNullOrEmpty(x))
            .ToList();

        return ValidationErrorResponse(errors);
    }

    /// <summary>
    /// Valida que un parámetro string no sea nulo o vacío
    /// </summary>
    protected ActionResult<ApiResponse<T>>? ValidateStringParameter<T>(string? value, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return ValidationErrorResponse<T>(new[] { $"{parameterName} es requerido y no puede estar vacío" });
        }
        return null;
    }

    /// <summary>
    /// Valida que un parámetro string no sea nulo o vacío (sin datos)
    /// </summary>
    protected ActionResult<ApiResponse>? ValidateStringParameter(string? value, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return ValidationErrorResponse(new[] { $"{parameterName} es requerido y no puede estar vacío" });
        }
        return null;
    }

    /// <summary>
    /// Maneja la ejecución de una operación con manejo de errores estándar
    /// </summary>
    protected async Task<ActionResult<ApiResponse<T>>> ExecuteAsync<T>(Func<Task<T>> operation, string operationName)
    {
        try
        {
            Logger.LogDebug("Executing operation: {OperationName}", operationName);
            var result = await operation();
            Logger.LogDebug("Operation {OperationName} completed successfully", operationName);
            return SuccessResponse(result);
        }
        catch (InvalidOperationException ex)
        {
            Logger.LogWarning(ex, "Invalid operation in {OperationName}: {Message}", operationName, ex.Message);
            return ErrorResponse<T>(ex.Message, "INVALID_OPERATION");
        }
        catch (ArgumentException ex)
        {
            Logger.LogWarning(ex, "Invalid argument in {OperationName}: {Message}", operationName, ex.Message);
            return ErrorResponse<T>(ex.Message, "INVALID_ARGUMENT");
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Unexpected error in {OperationName}: {Message}", operationName, ex.Message);
            return ErrorResponse<T>("Error interno del servidor", "INTERNAL_ERROR");
        }
    }

    /// <summary>
    /// Maneja la ejecución de una operación sin datos de retorno con manejo de errores estándar
    /// </summary>
    protected async Task<ActionResult<ApiResponse>> ExecuteAsync(Func<Task> operation, string operationName)
    {
        try
        {
            Logger.LogDebug("Executing operation: {OperationName}", operationName);
            await operation();
            Logger.LogDebug("Operation {OperationName} completed successfully", operationName);
            return SuccessResponse();
        }
        catch (InvalidOperationException ex)
        {
            Logger.LogWarning(ex, "Invalid operation in {OperationName}: {Message}", operationName, ex.Message);
            return ErrorResponse(ex.Message, "INVALID_OPERATION");
        }
        catch (ArgumentException ex)
        {
            Logger.LogWarning(ex, "Invalid argument in {OperationName}: {Message}", operationName, ex.Message);
            return ErrorResponse(ex.Message, "INVALID_ARGUMENT");
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Unexpected error in {OperationName}: {Message}", operationName, ex.Message);
            return ErrorResponse("Error interno del servidor", "INTERNAL_ERROR");
        }
    }

    /// <summary>
    /// Convierte un IResult a ActionResult con ApiResponse
    /// </summary>
    protected ActionResult<ApiResponse<T>> ToActionResult<T>(PimFlow.Contracts.Common.IResult<T> result)
    {
        if (result.IsSuccess)
        {
            return SuccessResponse(result.Value);
        }

        Logger.LogWarning("Operation failed: {Error}", result.Error);
        return ErrorResponse<T>(result.Error);
    }

    /// <summary>
    /// Convierte un IResult a ActionResult con ApiResponse (sin datos)
    /// </summary>
    protected ActionResult<ApiResponse> ToActionResult(PimFlow.Contracts.Common.IResult result)
    {
        if (result.IsSuccess)
        {
            return SuccessResponse();
        }

        Logger.LogWarning("Operation failed: {Error}", result.Error);
        return ErrorResponse(result.Error);
    }

    /// <summary>
    /// Publica eventos de dominio de un aggregate root si el servicio está disponible
    /// </summary>
    protected async Task PublishDomainEventsAsync(AggregateRoot aggregateRoot)
    {
        if (DomainEventService != null && aggregateRoot.HasDomainEvents)
        {
            try
            {
                await DomainEventService.PublishEventsAsync(aggregateRoot);
                Logger.LogDebug("Published {EventCount} domain events from {AggregateType}", 
                    aggregateRoot.DomainEvents.Count, aggregateRoot.GetType().Name);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error publishing domain events from {AggregateType}", 
                    aggregateRoot.GetType().Name);
                // No re-throw para no afectar la operación principal
            }
        }
    }
}
