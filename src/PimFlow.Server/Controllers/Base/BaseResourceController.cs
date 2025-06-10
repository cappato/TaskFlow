using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PimFlow.Shared.Common;
using PimFlow.Server.Services;

namespace PimFlow.Server.Controllers.Base;

/// <summary>
/// Controlador base para recursos que implementan operaciones CRUD estándar
/// Proporciona endpoints comunes: GET, GET by ID, POST, PUT, DELETE
/// </summary>
/// <typeparam name="TDto">Tipo del DTO principal</typeparam>
/// <typeparam name="TCreateDto">Tipo del DTO para creación</typeparam>
/// <typeparam name="TUpdateDto">Tipo del DTO para actualización</typeparam>
/// <typeparam name="TService">Tipo del servicio que maneja las operaciones</typeparam>
[Route("api/[controller]")]
public abstract class BaseResourceController<TDto, TCreateDto, TUpdateDto, TService> : BaseApiController
    where TDto : class
    where TCreateDto : class
    where TUpdateDto : class
    where TService : class
{
    protected readonly TService Service;

    protected BaseResourceController(TService service, ILogger logger, IDomainEventService? domainEventService = null)
        : base(logger, domainEventService)
    {
        Service = service ?? throw new ArgumentNullException(nameof(service));
    }

    /// <summary>
    /// GET api/[controller]
    /// Obtiene todos los recursos
    /// </summary>
    [HttpGet]
    public virtual async Task<ActionResult<ApiResponse<IEnumerable<TDto>>>> GetAll()
    {
        return await ExecuteAsync(async () =>
        {
            var items = await GetAllItemsAsync();
            Logger.LogInformation("Retrieved {ItemCount} {ResourceType} items", 
                items?.Count() ?? 0, typeof(TDto).Name);
            return items;
        }, $"GetAll{typeof(TDto).Name}");
    }

    /// <summary>
    /// GET api/[controller]/{id}
    /// Obtiene un recurso por ID
    /// </summary>
    [HttpGet("{id}")]
    public virtual async Task<ActionResult<ApiResponse<TDto>>> GetById(int id)
    {
        return await ExecuteAsync(async () =>
        {
            var item = await GetItemByIdAsync(id);
            if (item == null)
            {
                Logger.LogInformation("{ResourceType} with ID {Id} not found", typeof(TDto).Name, id);
                throw new InvalidOperationException($"{typeof(TDto).Name} no encontrado");
            }

            Logger.LogDebug("Retrieved {ResourceType} with ID {Id}", typeof(TDto).Name, id);
            return item;
        }, $"GetById{typeof(TDto).Name}");
    }

    /// <summary>
    /// POST api/[controller]
    /// Crea un nuevo recurso
    /// </summary>
    [HttpPost]
    public virtual async Task<ActionResult<ApiResponse<TDto>>> Create([FromBody] TCreateDto createDto)
    {
        var validationResult = ValidateModelState<TDto>();
        if (validationResult != null)
            return validationResult;

        return await ExecuteAsync(async () =>
        {
            var item = await CreateItemAsync(createDto);
            Logger.LogInformation("Created new {ResourceType} with ID {Id}", 
                typeof(TDto).Name, GetItemId(item));
            return item;
        }, $"Create{typeof(TDto).Name}");
    }

    /// <summary>
    /// PUT api/[controller]/{id}
    /// Actualiza un recurso existente
    /// </summary>
    [HttpPut("{id}")]
    public virtual async Task<ActionResult<ApiResponse<TDto>>> Update(int id, [FromBody] TUpdateDto updateDto)
    {
        var validationResult = ValidateModelState<TDto>();
        if (validationResult != null)
            return validationResult;

        return await ExecuteAsync(async () =>
        {
            var item = await UpdateItemAsync(id, updateDto);
            if (item == null)
            {
                Logger.LogInformation("{ResourceType} with ID {Id} not found for update", typeof(TDto).Name, id);
                throw new InvalidOperationException($"{typeof(TDto).Name} no encontrado");
            }

            Logger.LogInformation("Updated {ResourceType} with ID {Id}", typeof(TDto).Name, id);
            return item;
        }, $"Update{typeof(TDto).Name}");
    }

    /// <summary>
    /// DELETE api/[controller]/{id}
    /// Elimina un recurso
    /// </summary>
    [HttpDelete("{id}")]
    public virtual async Task<ActionResult<ApiResponse>> Delete(int id)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await DeleteItemAsync(id);
            if (!result)
            {
                Logger.LogInformation("{ResourceType} with ID {Id} not found for deletion", typeof(TDto).Name, id);
                throw new InvalidOperationException($"{typeof(TDto).Name} no encontrado");
            }

            Logger.LogInformation("Deleted {ResourceType} with ID {Id}", typeof(TDto).Name, id);
        }, $"Delete{typeof(TDto).Name}");
    }

    /// <summary>
    /// Método abstracto para obtener todos los elementos
    /// Debe ser implementado por las clases derivadas
    /// </summary>
    protected abstract Task<IEnumerable<TDto>> GetAllItemsAsync();

    /// <summary>
    /// Método abstracto para obtener un elemento por ID
    /// Debe ser implementado por las clases derivadas
    /// </summary>
    protected abstract Task<TDto?> GetItemByIdAsync(int id);

    /// <summary>
    /// Método abstracto para crear un nuevo elemento
    /// Debe ser implementado por las clases derivadas
    /// </summary>
    protected abstract Task<TDto> CreateItemAsync(TCreateDto createDto);

    /// <summary>
    /// Método abstracto para actualizar un elemento
    /// Debe ser implementado por las clases derivadas
    /// </summary>
    protected abstract Task<TDto?> UpdateItemAsync(int id, TUpdateDto updateDto);

    /// <summary>
    /// Método abstracto para eliminar un elemento
    /// Debe ser implementado por las clases derivadas
    /// </summary>
    protected abstract Task<bool> DeleteItemAsync(int id);

    /// <summary>
    /// Método virtual para obtener el ID de un elemento
    /// Puede ser sobrescrito por las clases derivadas si es necesario
    /// </summary>
    protected virtual object? GetItemId(TDto item)
    {
        // Intenta obtener la propiedad Id usando reflexión
        var idProperty = typeof(TDto).GetProperty("Id");
        return idProperty?.GetValue(item);
    }

    /// <summary>
    /// Método virtual para validaciones adicionales en creación
    /// Puede ser sobrescrito por las clases derivadas
    /// </summary>
    protected virtual Task<ActionResult<ApiResponse<TDto>>?> ValidateCreateAsync(TCreateDto createDto)
    {
        return Task.FromResult<ActionResult<ApiResponse<TDto>>?>(null);
    }

    /// <summary>
    /// Método virtual para validaciones adicionales en actualización
    /// Puede ser sobrescrito por las clases derivadas
    /// </summary>
    protected virtual Task<ActionResult<ApiResponse<TDto>>?> ValidateUpdateAsync(int id, TUpdateDto updateDto)
    {
        return Task.FromResult<ActionResult<ApiResponse<TDto>>?>(null);
    }

    /// <summary>
    /// Método virtual para validaciones adicionales en eliminación
    /// Puede ser sobrescrito por las clases derivadas
    /// </summary>
    protected virtual Task<ActionResult<ApiResponse>?> ValidateDeleteAsync(int id)
    {
        return Task.FromResult<ActionResult<ApiResponse>?>(null);
    }

    /// <summary>
    /// Método virtual para acciones post-creación
    /// Puede ser sobrescrito por las clases derivadas
    /// </summary>
    protected virtual Task OnItemCreatedAsync(TDto item, TCreateDto createDto)
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// Método virtual para acciones post-actualización
    /// Puede ser sobrescrito por las clases derivadas
    /// </summary>
    protected virtual Task OnItemUpdatedAsync(TDto item, TUpdateDto updateDto)
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// Método virtual para acciones post-eliminación
    /// Puede ser sobrescrito por las clases derivadas
    /// </summary>
    protected virtual Task OnItemDeletedAsync(int id)
    {
        return Task.CompletedTask;
    }
}
