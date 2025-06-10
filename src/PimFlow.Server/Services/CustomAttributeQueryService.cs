using PimFlow.Domain.Entities;
using PimFlow.Domain.Interfaces;
using PimFlow.Shared.DTOs;
using PimFlow.Server.Mappers;

namespace PimFlow.Server.Services;

/// <summary>
/// Implementation of CustomAttribute queries (CQRS - Query side)
/// Follows Single Responsibility Principle - only handles read operations
/// Follows Dependency Inversion Principle - depends on abstractions
/// </summary>
public class CustomAttributeQueryService : ICustomAttributeQueryService
{
    private readonly ICustomAttributeRepository _customAttributeRepository;

    public CustomAttributeQueryService(ICustomAttributeRepository customAttributeRepository)
    {
        _customAttributeRepository = customAttributeRepository;
    }

    public async Task<IEnumerable<CustomAttributeDto>> GetAllAttributesAsync()
    {
        var attributes = await _customAttributeRepository.GetAllAsync();
        return attributes.Select(MapToDto);
    }

    public async Task<IEnumerable<CustomAttributeDto>> GetActiveAttributesAsync()
    {
        var attributes = await _customAttributeRepository.GetActiveAsync();
        return attributes.Select(MapToDto);
    }

    public async Task<CustomAttributeDto?> GetAttributeByIdAsync(int id)
    {
        var attribute = await _customAttributeRepository.GetByIdAsync(id);
        return attribute != null ? MapToDto(attribute) : null;
    }

    private static CustomAttributeDto MapToDto(CustomAttribute attribute)
    {
        return new CustomAttributeDto
        {
            Id = attribute.Id,
            Name = attribute.Name,
            DisplayName = attribute.DisplayName,
            Type = DomainEnumMapper.ToShared(attribute.Type),
            IsRequired = attribute.IsRequired,
            DefaultValue = attribute.DefaultValue,
            ValidationRules = attribute.ValidationRules,
            SortOrder = attribute.SortOrder,
            IsActive = attribute.IsActive,
            CreatedAt = attribute.CreatedAt,
            UpdatedAt = attribute.UpdatedAt
        };
    }
}
