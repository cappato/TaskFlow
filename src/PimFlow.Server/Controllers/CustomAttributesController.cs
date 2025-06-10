using Microsoft.AspNetCore.Mvc;
using PimFlow.Server.Services;
using PimFlow.Shared.DTOs;
using PimFlow.Server.Controllers.Base;
using PimFlow.Shared.Common;

namespace PimFlow.Server.Controllers;

public class CustomAttributesController : BaseResourceController<CustomAttributeDto, CreateCustomAttributeDto, UpdateCustomAttributeDto, ICustomAttributeService>
{
    public CustomAttributesController(ICustomAttributeService customAttributeService, ILogger<CustomAttributesController> logger, IDomainEventService? domainEventService = null)
        : base(customAttributeService, logger, domainEventService)
    {
    }

    // Implementación de métodos abstractos del BaseResourceController
    protected override async Task<IEnumerable<CustomAttributeDto>> GetAllItemsAsync()
    {
        return await Service.GetAllAttributesAsync();
    }

    protected override async Task<CustomAttributeDto?> GetItemByIdAsync(int id)
    {
        return await Service.GetAttributeByIdAsync(id);
    }

    protected override async Task<CustomAttributeDto> CreateItemAsync(CreateCustomAttributeDto createDto)
    {
        return await Service.CreateAttributeAsync(createDto);
    }

    protected override async Task<CustomAttributeDto?> UpdateItemAsync(int id, UpdateCustomAttributeDto updateDto)
    {
        return await Service.UpdateAttributeAsync(id, updateDto);
    }

    protected override async Task<bool> DeleteItemAsync(int id)
    {
        return await Service.DeleteAttributeAsync(id);
    }

    // Endpoints específicos de CustomAttributes (no cubiertos por BaseResourceController)

    [HttpGet("active")]
    public async Task<ActionResult<ApiResponse<IEnumerable<CustomAttributeDto>>>> GetActiveAttributes()
    {
        return await ExecuteAsync(async () =>
        {
            var attributes = await Service.GetActiveAttributesAsync();
            Logger.LogInformation("Retrieved {AttributeCount} active custom attributes", attributes?.Count() ?? 0);
            return attributes;
        }, "GetActiveCustomAttributes");
    }

}
