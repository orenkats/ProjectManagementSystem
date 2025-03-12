using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProjectManagementSystem.API.ProjectService.Database;
using ProjectManagementSystem.Common.Exceptions;
using ProjectManagementSystem.Infrastructure.Database;

namespace ProjectManagementSystem.API.TaskService.Database;

public class TaskRepository : ITaskRepository
{
    private readonly ApplicationContext _context;
    private readonly ILogger<TaskRepository> _logger;

    public TaskRepository(ApplicationContext context, ILogger<TaskRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<TaskEntity> GetByIdAsync(Guid taskId)
    {
        var task = await _context.Tasks.FirstOrDefaultAsync(t => t.Id == taskId);
        if (task == null)
        {
            throw new EntityNotFoundException(taskId.ToString());
        }
        _logger.LogInformation($"Retrieved Task: ID={taskId}");
        return task;
    }

    public async Task<IEnumerable<TaskEntity>> GetAllAsync(int pageNumber, int pageSize)
    {
        var tasks = await _context.Tasks
            .OrderBy(t=>t.Title)
            .Skip((pageNumber - 1)*pageSize)
            .Take(pageSize)
            .ToListAsync();
        _logger.LogInformation($"Retrieved all tasks. Count={tasks.Count}");
        return tasks;
    }

    public async Task<IEnumerable<TaskEntity>> GetByProjectIdAsync(Guid projectId, int pageNumber, int pageSize)
    {
        if (!await _context.Tasks.AnyAsync(t => t.ProjectId == projectId))
        {
            throw new EntityNotFoundException(projectId.ToString());
        }

        var tasks = await _context.Tasks
            .Where(t => t.ProjectId == projectId)
            .OrderBy(t => t.Title)  
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        _logger.LogInformation($"Retrieved tasks for Project ID={projectId}, PageNumber={pageNumber}, PageSize={pageSize}, Count={tasks.Count}");
        return tasks;
    }
    
    public async Task AddAsync(TaskEntity task)
    {
        await _context.Tasks.AddAsync(task);
        await _context.SaveChangesAsync();
        _logger.LogInformation($"Created Task: ID={task.Id}, Title={task.Title}");
    }

    public async Task UpdateAsync(TaskEntity task)
    {
        var existingTask = await _context.Tasks.FindAsync(task.Id);
        if (existingTask == null)
        {
            throw new EntityNotFoundException(task.Id.ToString());
        }
        _context.Tasks.Update(task);
        await _context.SaveChangesAsync();
        _logger.LogInformation($"Updated Task: ID={task.Id}, Title={task.Title}");
    }

    public async Task DeleteAsync(Guid taskId)
    {
        var task = await _context.Tasks.FindAsync(taskId);
        if (task == null)
        {
            throw new EntityNotFoundException(taskId.ToString());
        }
        _context.Tasks.Remove(task);
        await _context.SaveChangesAsync();
        _logger.LogInformation($"Deleted Task: ID={taskId}");
    }
}

