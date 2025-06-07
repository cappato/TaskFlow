using System.Net.Http.Json;
using TaskFlow.Shared.DTOs;

namespace TaskFlow.Client.Services;

public class ProjectApiService : IProjectApiService
{
    private readonly HttpClient _httpClient;
    private const string ApiEndpoint = "api/projects";

    public ProjectApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IEnumerable<ProjectDto>> GetAllProjectsAsync()
    {
        try
        {
            var projects = await _httpClient.GetFromJsonAsync<IEnumerable<ProjectDto>>(ApiEndpoint);
            return projects ?? Enumerable.Empty<ProjectDto>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting projects: {ex.Message}");
            return Enumerable.Empty<ProjectDto>();
        }
    }

    public async Task<IEnumerable<ProjectDto>> GetActiveProjectsAsync()
    {
        try
        {
            var projects = await _httpClient.GetFromJsonAsync<IEnumerable<ProjectDto>>($"{ApiEndpoint}/active");
            return projects ?? Enumerable.Empty<ProjectDto>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting active projects: {ex.Message}");
            return Enumerable.Empty<ProjectDto>();
        }
    }

    public async Task<ProjectDto?> GetProjectByIdAsync(int id)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<ProjectDto>($"{ApiEndpoint}/{id}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting project {id}: {ex.Message}");
            return null;
        }
    }

    public async Task<ProjectDto> CreateProjectAsync(CreateProjectDto createProjectDto)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync(ApiEndpoint, createProjectDto);
            response.EnsureSuccessStatusCode();
            
            var project = await response.Content.ReadFromJsonAsync<ProjectDto>();
            return project ?? throw new InvalidOperationException("Failed to create project");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating project: {ex.Message}");
            throw;
        }
    }

    public async Task<ProjectDto?> UpdateProjectAsync(int id, CreateProjectDto updateProjectDto)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync($"{ApiEndpoint}/{id}", updateProjectDto);
            response.EnsureSuccessStatusCode();
            
            return await response.Content.ReadFromJsonAsync<ProjectDto>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating project {id}: {ex.Message}");
            return null;
        }
    }

    public async Task<bool> DeleteProjectAsync(int id)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"{ApiEndpoint}/{id}");
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting project {id}: {ex.Message}");
            return false;
        }
    }
}
