using TaskFlow.Shared.DTOs;

namespace TaskFlow.Client.Services;

public interface IProjectApiService
{
    Task<IEnumerable<ProjectDto>> GetAllProjectsAsync();
    Task<IEnumerable<ProjectDto>> GetActiveProjectsAsync();
    Task<ProjectDto?> GetProjectByIdAsync(int id);
    Task<ProjectDto> CreateProjectAsync(CreateProjectDto createProjectDto);
    Task<ProjectDto?> UpdateProjectAsync(int id, CreateProjectDto updateProjectDto);
    Task<bool> DeleteProjectAsync(int id);
}
