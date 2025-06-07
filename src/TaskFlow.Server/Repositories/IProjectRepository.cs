using TaskFlow.Server.Models;

namespace TaskFlow.Server.Repositories;

public interface IProjectRepository
{
    Task<IEnumerable<Project>> GetAllAsync();
    Task<Project?> GetByIdAsync(int id);
    Task<IEnumerable<Project>> GetActiveProjectsAsync();
    Task<Project> CreateAsync(Project project);
    Task<Project?> UpdateAsync(Project project);
    Task<bool> DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
}
