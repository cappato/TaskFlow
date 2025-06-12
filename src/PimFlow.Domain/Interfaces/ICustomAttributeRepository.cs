using CustomAttributeEntity = PimFlow.Domain.CustomAttribute.CustomAttribute;

namespace PimFlow.Domain.Interfaces;

public interface ICustomAttributeRepository
{
    Task<IEnumerable<CustomAttributeEntity>> GetAllAsync();
    Task<IEnumerable<CustomAttributeEntity>> GetActiveAsync();
    Task<CustomAttributeEntity?> GetByIdAsync(int id);
    Task<CustomAttributeEntity?> GetByNameAsync(string name);
    Task<CustomAttributeEntity> CreateAsync(CustomAttributeEntity attribute);
    Task<CustomAttributeEntity?> UpdateAsync(CustomAttributeEntity attribute);
    Task<bool> DeleteAsync(int id);
    Task<bool> ExistsByNameAsync(string name);
}
