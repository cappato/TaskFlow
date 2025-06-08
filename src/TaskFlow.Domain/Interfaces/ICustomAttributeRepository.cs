using TaskFlow.Domain.Entities;

namespace TaskFlow.Domain.Interfaces;

public interface ICustomAttributeRepository
{
    Task<IEnumerable<CustomAttribute>> GetAllAsync();
    Task<IEnumerable<CustomAttribute>> GetActiveAsync();
    Task<CustomAttribute?> GetByIdAsync(int id);
    Task<CustomAttribute?> GetByNameAsync(string name);
    Task<CustomAttribute> CreateAsync(CustomAttribute attribute);
    Task<CustomAttribute?> UpdateAsync(CustomAttribute attribute);
    Task<bool> DeleteAsync(int id);
    Task<bool> ExistsByNameAsync(string name);
}
