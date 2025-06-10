using PimFlow.Shared.DTOs;

namespace PimFlow.Server.Services;

/// <summary>
/// INTERFACE SEGREGATION PRINCIPLE (ISP) - Interfaces segregadas para CustomAttribute
/// </summary>

/// <summary>
/// Interface for CustomAttribute read operations only
/// Follows ISP - clients that only need to read attributes don't depend on write operations
/// </summary>
public interface ICustomAttributeReader
{
    Task<IEnumerable<CustomAttributeDto>> GetAllAttributesAsync();
    Task<IEnumerable<CustomAttributeDto>> GetActiveAttributesAsync();
    Task<CustomAttributeDto?> GetAttributeByIdAsync(int id);
}

/// <summary>
/// Interface for CustomAttribute write operations only
/// Follows ISP - clients that only need to write attributes don't depend on read operations
/// </summary>
public interface ICustomAttributeWriter
{
    Task<CustomAttributeDto> CreateAttributeAsync(CreateCustomAttributeDto createAttributeDto);
    Task<CustomAttributeDto?> UpdateAttributeAsync(int id, UpdateCustomAttributeDto updateAttributeDto);
    Task<bool> DeleteAttributeAsync(int id);
}

/// <summary>
/// Facade interface that combines all CustomAttribute operations for backward compatibility
/// Follows ISP by inheriting from segregated interfaces
/// </summary>
public interface ICustomAttributeService : ICustomAttributeReader, ICustomAttributeWriter
{
    // This interface is now composed of segregated interfaces
    // Clients can depend on specific interfaces (ICustomAttributeReader, ICustomAttributeWriter)
    // or on the full interface for backward compatibility
}
