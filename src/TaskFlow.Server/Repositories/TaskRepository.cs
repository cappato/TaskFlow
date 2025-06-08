using Microsoft.EntityFrameworkCore;
using TaskFlow.Server.Data;
using TaskFlow.Server.Models;
using TaskFlow.Shared.Enums;

namespace TaskFlow.Server.Repositories;

public class TaskRepository : ITaskRepository
{
    private readonly TaskFlowDbContext _context;

    public TaskRepository(TaskFlowDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<TaskItem>> GetAllAsync()
    {
        return await _context.Tasks
            .Include(t => t.Project)
            .Include(t => t.AssignedToUser)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
    }

    public async Task<TaskItem?> GetByIdAsync(int id)
    {
        return await _context.Tasks
            .Include(t => t.Project)
            .Include(t => t.AssignedToUser)
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<IEnumerable<TaskItem>> GetByProjectIdAsync(int projectId)
    {
        return await _context.Tasks
            .Include(t => t.Project)
            .Include(t => t.AssignedToUser)
            .Where(t => t.ProjectId == projectId)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<TaskItem>> GetByStatusAsync(TaskState status)
    {
        return await _context.Tasks
            .Include(t => t.Project)
            .Include(t => t.AssignedToUser)
            .Where(t => t.Status == status)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<TaskItem>> GetByAssignedUserIdAsync(int userId)
    {
        return await _context.Tasks
            .Include(t => t.Project)
            .Include(t => t.AssignedToUser)
            .Where(t => t.AssignedToUserId == userId)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
    }

    public async Task<TaskItem> CreateAsync(TaskItem task)
    {
        task.CreatedAt = DateTime.UtcNow;
        _context.Tasks.Add(task);
        await _context.SaveChangesAsync();

        return await GetByIdAsync(task.Id) ?? task;
    }

    public async Task<TaskItem?> UpdateAsync(TaskItem task)
    {
        var existingTask = await _context.Tasks.FindAsync(task.Id);
        if (existingTask == null)
            return null;

        _context.Entry(existingTask).CurrentValues.SetValues(task);

        if (task.Status == TaskState.Completed && existingTask.CompletedAt == null)
        {
            existingTask.CompletedAt = DateTime.UtcNow;
        }
        else if (task.Status != TaskState.Completed)
        {
            existingTask.CompletedAt = null;
        }

        await _context.SaveChangesAsync();
        return await GetByIdAsync(task.Id);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var task = await _context.Tasks.FindAsync(id);
        if (task == null)
            return false;

        _context.Tasks.Remove(task);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.Tasks.AnyAsync(t => t.Id == id);
    }
}
