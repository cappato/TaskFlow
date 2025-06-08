using TaskFlow.Server.Models;
using TaskFlow.Server.Repositories;
using TaskFlow.Shared.DTOs;
using TaskFlow.Shared.Enums;

namespace TaskFlow.Server.Services;

public class TaskService : ITaskService
{
    private readonly ITaskRepository _taskRepository;

    public TaskService(ITaskRepository taskRepository)
    {
        _taskRepository = taskRepository;
    }

    public async Task<IEnumerable<TaskDto>> GetAllTasksAsync()
    {
        var tasks = await _taskRepository.GetAllAsync();
        return tasks.Select(MapToDto);
    }

    public async Task<TaskDto?> GetTaskByIdAsync(int id)
    {
        var task = await _taskRepository.GetByIdAsync(id);
        return task != null ? MapToDto(task) : null;
    }

    public async Task<IEnumerable<TaskDto>> GetTasksByProjectIdAsync(int projectId)
    {
        var tasks = await _taskRepository.GetByProjectIdAsync(projectId);
        return tasks.Select(MapToDto);
    }

    public async Task<IEnumerable<TaskDto>> GetTasksByStatusAsync(TaskState status)
    {
        var tasks = await _taskRepository.GetByStatusAsync(status);
        return tasks.Select(MapToDto);
    }

    public async Task<IEnumerable<TaskDto>> GetTasksByUserIdAsync(int userId)
    {
        var tasks = await _taskRepository.GetByAssignedUserIdAsync(userId);
        return tasks.Select(MapToDto);
    }

    public async Task<TaskDto> CreateTaskAsync(CreateTaskDto createTaskDto)
    {
        var task = new TaskItem
        {
            Title = createTaskDto.Title,
            Description = createTaskDto.Description,
            Priority = createTaskDto.Priority,
            DueDate = createTaskDto.DueDate,
            ProjectId = createTaskDto.ProjectId,
            AssignedToUserId = createTaskDto.AssignedToUserId,
            Status = TaskState.Pending
        };

        var createdTask = await _taskRepository.CreateAsync(task);
        return MapToDto(createdTask);
    }

    public async Task<TaskDto?> UpdateTaskAsync(int id, UpdateTaskDto updateTaskDto)
    {
        var existingTask = await _taskRepository.GetByIdAsync(id);
        if (existingTask == null)
            return null;

        // Update only provided fields
        if (!string.IsNullOrEmpty(updateTaskDto.Title))
            existingTask.Title = updateTaskDto.Title;

        if (!string.IsNullOrEmpty(updateTaskDto.Description))
            existingTask.Description = updateTaskDto.Description;

        if (updateTaskDto.Status.HasValue)
            existingTask.Status = updateTaskDto.Status.Value;

        if (updateTaskDto.Priority.HasValue)
            existingTask.Priority = updateTaskDto.Priority.Value;

        if (updateTaskDto.DueDate.HasValue)
            existingTask.DueDate = updateTaskDto.DueDate.Value;

        if (updateTaskDto.ProjectId.HasValue)
            existingTask.ProjectId = updateTaskDto.ProjectId.Value;

        if (updateTaskDto.AssignedToUserId.HasValue)
            existingTask.AssignedToUserId = updateTaskDto.AssignedToUserId.Value;

        var updatedTask = await _taskRepository.UpdateAsync(existingTask);
        return updatedTask != null ? MapToDto(updatedTask) : null;
    }

    public async Task<bool> DeleteTaskAsync(int id)
    {
        return await _taskRepository.DeleteAsync(id);
    }

    private static TaskDto MapToDto(TaskItem task)
    {
        return new TaskDto
        {
            Id = task.Id,
            Title = task.Title,
            Description = task.Description,
            Status = task.Status,
            Priority = task.Priority,
            CreatedAt = task.CreatedAt,
            DueDate = task.DueDate,
            CompletedAt = task.CompletedAt,
            ProjectId = task.ProjectId,
            ProjectName = task.Project?.Name,
            AssignedToUserId = task.AssignedToUserId,
            AssignedToUserName = task.AssignedToUser?.Name
        };
    }
}
