using ProjectManagementSystem.Infrastructure.Database;

namespace ProjectManagementSystem.API.ProjectService.Managers
{
    public interface IProjectManager
    {
        Task<IEnumerable<ProjectEntity>> GetAllProjectsAsync(int pageNumber , int pageSize );
        Task<ProjectEntity> GetProjectByIdAsync(Guid projectId);
        Task CreateProjectAsync(ProjectEntity project);
        Task UpdateProjectAsync(ProjectEntity project);
        Task DeleteProjectAsync(Guid projectId);
    }
}