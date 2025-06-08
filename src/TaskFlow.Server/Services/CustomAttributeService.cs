using TaskFlow.Server.Models;
using TaskFlow.Server.Repositories;
using TaskFlow.Shared.DTOs;

namespace TaskFlow.Server.Services;

public class CustomAttributeService : ICustomAttributeService
{
    private readonly ICustomAttributeRepository _customAttributeRepository;

    public CustomAttributeService(ICustomAttributeRepository customAttributeRepository)
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
            Type = createAttributeDto.Type,
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
            existingAttribute.Type = updateAttributeDto.Type.Value;

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
            Type = attribute.Type,
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
