using PimFlow.Shared.DTOs;

namespace PimFlow.Client.Services;

public interface ICustomAttributeApiService
{
    Task<IEnumerable<CustomAttributeDto>> GetAllAttributesAsync();
    Task<IEnumerable<CustomAttributeDto>> GetActiveAttributesAsync();
    Task<CustomAttributeDto?> GetAttributeByIdAsync(int id);
    Task<CustomAttributeDto> CreateAttributeAsync(CreateCustomAttributeDto createAttributeDto);
    Task<CustomAttributeDto?> UpdateAttributeAsync(int id, UpdateCustomAttributeDto updateAttributeDto);
    Task<bool> DeleteAttributeAsync(int id);
}
