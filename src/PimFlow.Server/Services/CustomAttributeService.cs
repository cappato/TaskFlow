using PimFlow.Shared.DTOs;

namespace PimFlow.Server.Services;

/// <summary>
/// Facade service that coordinates CustomAttribute operations using CQRS pattern
/// Follows Single Responsibility Principle by delegating to specialized services
/// Follows Interface Segregation Principle by implementing segregated interfaces
/// Maintains backward compatibility while improving architecture
/// </summary>
public class CustomAttributeService : ICustomAttributeService, ICustomAttributeReader, ICustomAttributeWriter
{
    private readonly ICustomAttributeQueryService _queryService;
    private readonly ICustomAttributeCommandService _commandService;

    public CustomAttributeService(
        ICustomAttributeQueryService queryService,
        ICustomAttributeCommandService commandService)
    {
        _queryService = queryService;
        _commandService = commandService;
    }

    // Query operations - delegated to specialized query service
    public async Task<IEnumerable<CustomAttributeDto>> GetAllAttributesAsync()
        => await _queryService.GetAllAttributesAsync();

    public async Task<IEnumerable<CustomAttributeDto>> GetActiveAttributesAsync()
        => await _queryService.GetActiveAttributesAsync();

    public async Task<CustomAttributeDto?> GetAttributeByIdAsync(int id)
        => await _queryService.GetAttributeByIdAsync(id);

    // Command operations - delegated to specialized command service
    public async Task<CustomAttributeDto> CreateAttributeAsync(CreateCustomAttributeDto createAttributeDto)
        => await _commandService.CreateAttributeAsync(createAttributeDto);

    public async Task<CustomAttributeDto?> UpdateAttributeAsync(int id, UpdateCustomAttributeDto updateAttributeDto)
        => await _commandService.UpdateAttributeAsync(id, updateAttributeDto);

    public async Task<bool> DeleteAttributeAsync(int id)
        => await _commandService.DeleteAttributeAsync(id);
}
