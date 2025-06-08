using Microsoft.EntityFrameworkCore;
using TaskFlow.Server.Data;
using TaskFlow.Server.Models;

namespace TaskFlow.Server.Repositories;

public class CustomAttributeRepository : ICustomAttributeRepository
{
    private readonly TaskFlowDbContext _context;

    public CustomAttributeRepository(TaskFlowDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<CustomAttribute>> GetAllAsync()
    {
        return await _context.CustomAttributes
            .OrderBy(ca => ca.SortOrder)
            .ThenBy(ca => ca.DisplayName)
            .ToListAsync();
    }

    public async Task<IEnumerable<CustomAttribute>> GetActiveAsync()
    {
        return await _context.CustomAttributes
            .Where(ca => ca.IsActive)
            .OrderBy(ca => ca.SortOrder)
            .ThenBy(ca => ca.DisplayName)
            .ToListAsync();
    }

    public async Task<CustomAttribute?> GetByIdAsync(int id)
    {
        return await _context.CustomAttributes
            .FirstOrDefaultAsync(ca => ca.Id == id);
    }

    public async Task<CustomAttribute?> GetByNameAsync(string name)
    {
        return await _context.CustomAttributes
            .FirstOrDefaultAsync(ca => ca.Name == name);
    }

    public async Task<CustomAttribute> CreateAsync(CustomAttribute attribute)
    {
        attribute.CreatedAt = DateTime.UtcNow;
        _context.CustomAttributes.Add(attribute);
        await _context.SaveChangesAsync();
        return attribute;
    }

    public async Task<CustomAttribute?> UpdateAsync(CustomAttribute attribute)
    {
        var existingAttribute = await _context.CustomAttributes.FindAsync(attribute.Id);
        if (existingAttribute == null)
            return null;

        attribute.UpdatedAt = DateTime.UtcNow;
        _context.Entry(existingAttribute).CurrentValues.SetValues(attribute);
        await _context.SaveChangesAsync();
        return existingAttribute;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var attribute = await _context.CustomAttributes.FindAsync(id);
        if (attribute == null)
            return false;

        _context.CustomAttributes.Remove(attribute);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsByNameAsync(string name)
    {
        return await _context.CustomAttributes.AnyAsync(ca => ca.Name == name);
    }
}
