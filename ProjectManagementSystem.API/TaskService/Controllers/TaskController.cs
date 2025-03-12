using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectManagementSystem.Common.Contracts;
using ProjectManagementSystem.Infrastructure.Database;
using ProjectManagementSystem.API.TaskService.Managers;

namespace ProjectManagementSystem.API.TaskService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TaskController : ControllerBase
    {
        private readonly ITaskManager _taskManager;

        public TaskController(ITaskManager taskManager)
        {
            _taskManager = taskManager;
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Manager,User")]  // Allow all authenticated users to view all tasks
        public async Task<IActionResult> GetAllTasks(int pageNumber = 1, int pageSize = 10)
        {
            var tasks = await _taskManager.GetAllTasksAsync(pageNumber, pageSize);
            return Ok(tasks);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Manager,User")]  
        public async Task<IActionResult> GetTaskById(Guid id)
        {
            var task = await _taskManager.GetTaskByIdAsync(id);
            return Ok(task);
        }

        [HttpGet("by-project/{projectId}")]
        [Authorize(Roles = "Admin,Manager,User")]  
        public async Task<IActionResult> GetTasksByProjectId(Guid projectId, int pageNumber, int pageSize)
        {
            var tasks = await _taskManager.GetTasksByProjectIdAsync(projectId, pageNumber, pageSize);
            return Ok(tasks);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]  
        public async Task<CreateTaskResponse> CreateTask([FromBody] CreateTaskRequest request)
        {
            var taskEntity = new TaskEntity
            {
                Id = Guid.NewGuid(),
                ProjectId = request.ProjectId,
                Title = request.Title,
                Description = request.Description,
                Status = request.Status
            };
            await _taskManager.CreateTaskAsync(taskEntity);
            return new CreateTaskResponse{ TaskId = taskEntity.Id.ToString()};
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Manager")]  
        public async Task<UpdateTaskResponse> UpdateTask(Guid id, [FromBody] UpdateTaskRequest request)
        {
            var taskEntity = new TaskEntity
            {
                Id = id,
                ProjectId = request.ProjectId,
                Title = request.Title,
                Description = request.Description,
                Status = request.Status
            };
            await _taskManager.UpdateTaskAsync(taskEntity);
            return new UpdateTaskResponse();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]  
        public async Task<IActionResult> DeleteTask(Guid id)
        {
            await _taskManager.DeleteTaskAsync(id);
            return NoContent();
        }
    }
}
