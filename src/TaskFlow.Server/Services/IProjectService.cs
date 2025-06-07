using TaskFlow.Shared.DTOs;

namespace TaskFlow.Server.Services;

public interface IProjectService
{
    Task<IEnumerable<ProjectDto>> GetAllProjectsAsync();
    Task<ProjectDto?> GetProjectByIdAsync(int id);
    Task<IEnumerable<ProjectDto>> GetActiveProjectsAsync();
    Task<ProjectDto> CreateProjectAsync(CreateProjectDto createProjectDto);
    Task<ProjectDto?> UpdateProjectAsync(int id, CreateProjectDto updateProjectDto);
    Task<bool> DeleteProjectAsync(int id);
}
