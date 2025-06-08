using TaskFlow.Server.Models;
using TaskFlow.Server.Repositories;
using TaskFlow.Shared.DTOs;
using TaskFlow.Shared.Enums;

namespace TaskFlow.Server.Services;

public class ProjectService : IProjectService
{
    private readonly IProjectRepository _projectRepository;

    public ProjectService(IProjectRepository projectRepository)
    {
        _projectRepository = projectRepository;
    }

    public async Task<IEnumerable<ProjectDto>> GetAllProjectsAsync()
    {
        var projects = await _projectRepository.GetAllAsync();
        return projects.Select(MapToDto);
    }

    public async Task<ProjectDto?> GetProjectByIdAsync(int id)
    {
        var project = await _projectRepository.GetByIdAsync(id);
        return project != null ? MapToDto(project) : null;
    }

    public async Task<IEnumerable<ProjectDto>> GetActiveProjectsAsync()
    {
        var projects = await _projectRepository.GetActiveProjectsAsync();
        return projects.Select(MapToDto);
    }

    public async Task<ProjectDto> CreateProjectAsync(CreateProjectDto createProjectDto)
    {
        var project = new Project
        {
            Name = createProjectDto.Name,
            Description = createProjectDto.Description,
            StartDate = createProjectDto.StartDate,
            EndDate = createProjectDto.EndDate,
            IsActive = true
        };

        var createdProject = await _projectRepository.CreateAsync(project);
        return MapToDto(createdProject);
    }

    public async Task<ProjectDto?> UpdateProjectAsync(int id, CreateProjectDto updateProjectDto)
    {
        var existingProject = await _projectRepository.GetByIdAsync(id);
        if (existingProject == null)
            return null;

        existingProject.Name = updateProjectDto.Name;
        existingProject.Description = updateProjectDto.Description;
        existingProject.StartDate = updateProjectDto.StartDate;
        existingProject.EndDate = updateProjectDto.EndDate;

        var updatedProject = await _projectRepository.UpdateAsync(existingProject);
        return updatedProject != null ? MapToDto(updatedProject) : null;
    }

    public async Task<bool> DeleteProjectAsync(int id)
    {
        return await _projectRepository.DeleteAsync(id);
    }

    private static ProjectDto MapToDto(Project project)
    {
        return new ProjectDto
        {
            Id = project.Id,
            Name = project.Name,
            Description = project.Description,
            CreatedAt = project.CreatedAt,
            StartDate = project.StartDate,
            EndDate = project.EndDate,
            IsActive = project.IsActive,
            TaskCount = project.Tasks.Count,
            CompletedTaskCount = project.Tasks.Count(t => t.Status == TaskState.Completed)
        };
    }
}
