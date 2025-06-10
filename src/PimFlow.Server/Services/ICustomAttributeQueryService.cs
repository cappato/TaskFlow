using PimFlow.Shared.DTOs;

namespace PimFlow.Server.Services;

/// <summary>
/// Service interface for CustomAttribute queries (CQRS - Query side)
/// Follows Single Responsibility Principle by handling only read operations
/// </summary>
public interface ICustomAttributeQueryService
{
    /// <summary>
    /// Get all custom attributes
    /// </summary>
    Task<IEnumerable<CustomAttributeDto>> GetAllAttributesAsync();

    /// <summary>
    /// Get only active custom attributes
    /// </summary>
    Task<IEnumerable<CustomAttributeDto>> GetActiveAttributesAsync();

    /// <summary>
    /// Get custom attribute by unique identifier
    /// </summary>
    Task<CustomAttributeDto?> GetAttributeByIdAsync(int id);
}
