using PimFlow.Domain.Article;
using PimFlow.Domain.Category;
using PimFlow.Domain.User;
using PimFlow.Domain.CustomAttribute;
using PimFlow.Domain.Interfaces;
using PimFlow.Shared.DTOs;
using PimFlow.Server.Mappers;


namespace PimFlow.Server.Services;

/// <summary>
/// Implementation of CustomAttribute commands (CQRS - Command side)
/// Follows Single Responsibility Principle - only handles write operations
/// Follows Dependency Inversion Principle - depends on abstractions
/// </summary>
public class CustomAttributeCommandService : ICustomAttributeCommandService
{
    private readonly ICustomAttributeRepository _customAttributeRepository;

    public CustomAttributeCommandService(ICustomAttributeRepository customAttributeRepository)
    {
        _customAttributeRepository = customAttributeRepository;
    }

    public async Task<CustomAttributeDto> CreateAttributeAsync(CreateCustomAttributeDto createAttributeDto)
    {
        // Validate name uniqueness
        if (await _customAttributeRepository.ExistsByNameAsync(createAttributeDto.Name))
        {
            throw new InvalidOperationException($"Ya existe un atributo con nombre: {createAttributeDto.Name}");
        }

        var attribute = new CustomAttribute
        {
            Name = createAttributeDto.Name,
            DisplayName = createAttributeDto.DisplayName,
            Type = DomainEnumMapper.ToDomain(createAttributeDto.Type),
            IsRequired = createAttributeDto.IsRequired,
            DefaultValue = createAttributeDto.DefaultValue,
            ValidationRules = createAttributeDto.ValidationRules,
            SortOrder = createAttributeDto.SortOrder,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        var createdAttribute = await _customAttributeRepository.CreateAsync(attribute);
        return MapToDto(createdAttribute);
    }

    public async Task<CustomAttributeDto?> UpdateAttributeAsync(int id, UpdateCustomAttributeDto updateAttributeDto)
    {
        var existingAttribute = await _customAttributeRepository.GetByIdAsync(id);
        if (existingAttribute == null)
            return null;

        // Update properties
        if (!string.IsNullOrEmpty(updateAttributeDto.Name))
        {
            if (updateAttributeDto.Name != existingAttribute.Name &&
                await _customAttributeRepository.ExistsByNameAsync(updateAttributeDto.Name))
            {
                throw new InvalidOperationException($"Ya existe un atributo con nombre: {updateAttributeDto.Name}");
            }
            existingAttribute.Name = updateAttributeDto.Name;
        }

        if (!string.IsNullOrEmpty(updateAttributeDto.DisplayName))
            existingAttribute.DisplayName = updateAttributeDto.DisplayName;

        if (updateAttributeDto.Type.HasValue)
            existingAttribute.Type = DomainEnumMapper.ToDomain(updateAttributeDto.Type.Value);

        if (updateAttributeDto.IsRequired.HasValue)
            existingAttribute.IsRequired = updateAttributeDto.IsRequired.Value;

        if (updateAttributeDto.DefaultValue != null)
            existingAttribute.DefaultValue = updateAttributeDto.DefaultValue;

        if (updateAttributeDto.ValidationRules != null)
            existingAttribute.ValidationRules = updateAttributeDto.ValidationRules;

        if (updateAttributeDto.SortOrder.HasValue)
            existingAttribute.SortOrder = updateAttributeDto.SortOrder.Value;

        if (updateAttributeDto.IsActive.HasValue)
            existingAttribute.IsActive = updateAttributeDto.IsActive.Value;

        existingAttribute.UpdatedAt = DateTime.UtcNow;

        var updatedAttribute = await _customAttributeRepository.UpdateAsync(existingAttribute);
        return updatedAttribute != null ? MapToDto(updatedAttribute) : null;
    }

    public async Task<bool> DeleteAttributeAsync(int id)
    {
        return await _customAttributeRepository.DeleteAsync(id);
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
