using Microsoft.Extensions.Logging;
using ProjectManagementSystem.API.TaskService.Database;
using ProjectManagementSystem.Infrastructure.Database;

namespace ProjectManagementSystem.API.TaskService.Managers;

public class TaskManager : ITaskManager
{
    private readonly ITaskRepository _taskRepository;
    private readonly ILogger<TaskManager> _logger;

    public TaskManager(ITaskRepository taskRepository, ILogger<TaskManager> logger)
    {
        _taskRepository = taskRepository;
        _logger = logger;
    }
    
    public async Task<IEnumerable<TaskEntity>> GetAllTasksAsync(int pageNumber , int pageSize)
    {
        _logger.LogInformation("Retrieving all tasks");
        return await _taskRepository.GetAllAsync(pageNumber, pageSize);
    }

    public async Task<TaskEntity> GetTaskByIdAsync(Guid taskId)
    {
        _logger.LogInformation($"Attempting to retrieve task with ID: {taskId}");
        return await _taskRepository.GetByIdAsync(taskId);
    }

    public async Task<IEnumerable<TaskEntity>> GetTasksByProjectIdAsync(Guid projectId,int pageNumber, int pageSize)
    {
        _logger.LogInformation($"Retrieving all tasks for project ID: {projectId}");
        return await _taskRepository.GetByProjectIdAsync(projectId, pageNumber, pageSize);
    }

    public async Task CreateTaskAsync(TaskEntity task)
    {
        _logger.LogInformation($"Creating a new task with Title: {task.Title}");
        await _taskRepository.AddAsync(task);
    }

    public async Task UpdateTaskAsync(TaskEntity task)
    {
        _logger.LogInformation($"Updating task with ID: {task.Id}");
        await _taskRepository.UpdateAsync(task);
    }

    public async Task DeleteTaskAsync(Guid taskId)
    {
        _logger.LogInformation($"Deleting task with ID: {taskId}");
        await _taskRepository.DeleteAsync(taskId);
    }
}

