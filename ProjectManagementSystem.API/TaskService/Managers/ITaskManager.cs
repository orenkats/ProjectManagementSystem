using ProjectManagementSystem.Infrastructure.Database;

namespace ProjectManagementSystem.API.TaskService.Managers
{
    public interface ITaskManager
    {
        Task<IEnumerable<TaskEntity>> GetAllTasksAsync(int pageNumber , int pageSize);
        Task<TaskEntity> GetTaskByIdAsync(Guid taskId);
        Task<IEnumerable<TaskEntity>> GetTasksByProjectIdAsync(Guid projectId, int pageNumber, int pageSize);
        Task CreateTaskAsync(TaskEntity task);
        Task UpdateTaskAsync(TaskEntity task);
        Task DeleteTaskAsync(Guid taskId);
    }
}