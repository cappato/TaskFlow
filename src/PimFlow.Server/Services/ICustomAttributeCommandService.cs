using PimFlow.Shared.DTOs;

namespace PimFlow.Server.Services;

/// <summary>
/// Service interface for CustomAttribute commands (CQRS - Command side)
/// Follows Single Responsibility Principle by handling only write operations
/// </summary>
public interface ICustomAttributeCommandService
{
    /// <summary>
    /// Create a new custom attribute
    /// </summary>
    Task<CustomAttributeDto> CreateAttributeAsync(CreateCustomAttributeDto createAttributeDto);

    /// <summary>
    /// Update an existing custom attribute
    /// </summary>
    Task<CustomAttributeDto?> UpdateAttributeAsync(int id, UpdateCustomAttributeDto updateAttributeDto);

    /// <summary>
    /// Delete a custom attribute
    /// </summary>
    Task<bool> DeleteAttributeAsync(int id);
}
