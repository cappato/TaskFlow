using TaskFlow.Server.Models;
using TaskFlow.Shared.Enums;

namespace TaskFlow.Server.Repositories;

public interface ITaskRepository
{
    Task<IEnumerable<TaskItem>> GetAllAsync();
    Task<TaskItem?> GetByIdAsync(int id);
    Task<IEnumerable<TaskItem>> GetByProjectIdAsync(int projectId);
    Task<IEnumerable<TaskItem>> GetByStatusAsync(TaskState status);
    Task<IEnumerable<TaskItem>> GetByAssignedUserIdAsync(int userId);
    Task<TaskItem> CreateAsync(TaskItem task);
    Task<TaskItem?> UpdateAsync(TaskItem task);
    Task<bool> DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
}
