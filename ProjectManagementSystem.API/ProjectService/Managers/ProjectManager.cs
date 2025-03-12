// File: ProjectManagementSystem.API\ProjectService\Managers\ProjectManager.cs

using Microsoft.Extensions.Logging;
using ProjectManagementSystem.API.ProjectService.Database;
using ProjectManagementSystem.Infrastructure.Database;

namespace ProjectManagementSystem.API.ProjectService.Managers;

public class ProjectManager : IProjectManager
{
    private readonly IProjectRepository _projectRepository;
    private readonly ILogger<ProjectManager> _logger;

    public ProjectManager(IProjectRepository projectRepository, ILogger<ProjectManager> logger)
    {
        _projectRepository = projectRepository;
        _logger = logger;
    }
    
    public async Task<IEnumerable<ProjectEntity>> GetAllProjectsAsync(int pageNumber, int pageSize)
    {
        _logger.LogInformation($"Retrieving all projects with pagination: PageNumber={pageNumber}, PageSize={pageSize}");
        return await _projectRepository.GetAllAsync(pageNumber, pageSize);
    }
    
    public async Task<ProjectEntity> GetProjectByIdAsync(Guid projectId)
    {
        _logger.LogInformation($"Attempting to retrieve project with ID: {projectId}");
        return await _projectRepository.GetByIdAsync(projectId);
    }

    public async Task CreateProjectAsync(ProjectEntity project)
    {
        _logger.LogInformation($"Creating a new project with Name: {project.ProjectName}");
        await _projectRepository.AddAsync(project);
    }

    public async Task UpdateProjectAsync(ProjectEntity project)
    {
        _logger.LogInformation($"Updating project with ID: {project.Id}");
        await _projectRepository.UpdateAsync(project);
    }

    public async Task DeleteProjectAsync(Guid projectId)
    {
        _logger.LogInformation($"Deleting project with ID: {projectId}");
        await _projectRepository.DeleteAsync(projectId);
    }
}
