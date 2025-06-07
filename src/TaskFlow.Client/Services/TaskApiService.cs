using System.Net.Http.Json;
using TaskFlow.Shared.DTOs;
using TaskFlow.Shared.Enums;

namespace TaskFlow.Client.Services;

public class TaskApiService : ITaskApiService
{
    private readonly HttpClient _httpClient;
    private const string ApiEndpoint = "api/tasks";

    public TaskApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IEnumerable<TaskDto>> GetAllTasksAsync()
    {
        try
        {
            var tasks = await _httpClient.GetFromJsonAsync<IEnumerable<TaskDto>>(ApiEndpoint);
            return tasks ?? Enumerable.Empty<TaskDto>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting tasks: {ex.Message}");
            return Enumerable.Empty<TaskDto>();
        }
    }

    public async Task<TaskDto?> GetTaskByIdAsync(int id)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<TaskDto>($"{ApiEndpoint}/{id}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting task {id}: {ex.Message}");
            return null;
        }
    }

    public async Task<IEnumerable<TaskDto>> GetTasksByProjectIdAsync(int projectId)
    {
        try
        {
            var tasks = await _httpClient.GetFromJsonAsync<IEnumerable<TaskDto>>($"{ApiEndpoint}/project/{projectId}");
            return tasks ?? Enumerable.Empty<TaskDto>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting tasks for project {projectId}: {ex.Message}");
            return Enumerable.Empty<TaskDto>();
        }
    }

    public async Task<IEnumerable<TaskDto>> GetTasksByStatusAsync(TaskStatus status)
    {
        try
        {
            var tasks = await _httpClient.GetFromJsonAsync<IEnumerable<TaskDto>>($"{ApiEndpoint}/status/{status}");
            return tasks ?? Enumerable.Empty<TaskDto>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting tasks with status {status}: {ex.Message}");
            return Enumerable.Empty<TaskDto>();
        }
    }

    public async Task<TaskDto> CreateTaskAsync(CreateTaskDto createTaskDto)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync(ApiEndpoint, createTaskDto);
            response.EnsureSuccessStatusCode();
            
            var task = await response.Content.ReadFromJsonAsync<TaskDto>();
            return task ?? throw new InvalidOperationException("Failed to create task");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating task: {ex.Message}");
            throw;
        }
    }

    public async Task<TaskDto?> UpdateTaskAsync(int id, UpdateTaskDto updateTaskDto)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync($"{ApiEndpoint}/{id}", updateTaskDto);
            response.EnsureSuccessStatusCode();
            
            return await response.Content.ReadFromJsonAsync<TaskDto>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating task {id}: {ex.Message}");
            return null;
        }
    }

    public async Task<bool> DeleteTaskAsync(int id)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"{ApiEndpoint}/{id}");
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting task {id}: {ex.Message}");
            return false;
        }
    }
}
