using ProjectManagementSystem.Infrastructure.Database;

namespace ProjectManagementSystem.API.ProjectService.Database;

public interface IProjectRepository
{
    Task<ProjectEntity> GetByIdAsync(Guid projectId);
    Task<IEnumerable<ProjectEntity>> GetAllAsync(int pageNumber, int pageSize);
    Task AddAsync(ProjectEntity project);
    Task UpdateAsync(ProjectEntity project);
    Task DeleteAsync(Guid projectId);
}
