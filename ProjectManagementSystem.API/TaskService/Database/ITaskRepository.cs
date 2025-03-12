using ProjectManagementSystem.Infrastructure.Database;

namespace ProjectManagementSystem.API.TaskService.Database;

public interface ITaskRepository
{
    Task<TaskEntity> GetByIdAsync(Guid taskId);
    Task<IEnumerable<TaskEntity>> GetAllAsync(int pageNumber, int pageSize);
    Task<IEnumerable<TaskEntity>> GetByProjectIdAsync(Guid projectId, int pageNumber, int pageSize);
    Task AddAsync(TaskEntity task);
    Task UpdateAsync(TaskEntity task);
    Task DeleteAsync(Guid taskId);
}
