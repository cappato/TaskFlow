using TaskFlow.Shared.DTOs;
using TaskFlow.Shared.Enums;

namespace TaskFlow.Client.Services;

public interface ITaskApiService
{
    Task<IEnumerable<TaskDto>> GetAllTasksAsync();
    Task<TaskDto?> GetTaskByIdAsync(int id);
    Task<IEnumerable<TaskDto>> GetTasksByProjectIdAsync(int projectId);
    Task<IEnumerable<TaskDto>> GetTasksByStatusAsync(TaskState status);
    Task<TaskDto> CreateTaskAsync(CreateTaskDto createTaskDto);
    Task<TaskDto?> UpdateTaskAsync(int id, UpdateTaskDto updateTaskDto);
    Task<bool> DeleteTaskAsync(int id);
}
