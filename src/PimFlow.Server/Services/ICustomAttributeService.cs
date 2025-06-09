using PimFlow.Shared.DTOs;

namespace PimFlow.Server.Services;

public interface ICustomAttributeService
{
    Task<IEnumerable<CustomAttributeDto>> GetAllAttributesAsync();
    Task<IEnumerable<CustomAttributeDto>> GetActiveAttributesAsync();
    Task<CustomAttributeDto?> GetAttributeByIdAsync(int id);
    Task<CustomAttributeDto> CreateAttributeAsync(CreateCustomAttributeDto createAttributeDto);
    Task<CustomAttributeDto?> UpdateAttributeAsync(int id, UpdateCustomAttributeDto updateAttributeDto);
    Task<bool> DeleteAttributeAsync(int id);
}
