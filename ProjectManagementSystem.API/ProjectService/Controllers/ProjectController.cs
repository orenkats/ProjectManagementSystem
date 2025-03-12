using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectManagementSystem.API.ProjectService.Managers;
using ProjectManagementSystem.Common.Contracts;
using ProjectManagementSystem.Infrastructure.Database;

namespace ProjectManagementSystem.API.ProjectService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProjectController : ControllerBase
    {
        private readonly IProjectManager _projectManager;

        public ProjectController(IProjectManager projectManager)
        {
            _projectManager = projectManager;
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Manager,User")]
        public async Task<IActionResult> GetAllProjects(int pageNumber = 1, int pageSize = 10)
        {
            var projects = await _projectManager.GetAllProjectsAsync(pageNumber, pageSize);
            return Ok(projects);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Manager,User")]
        public async Task<IActionResult> GetProjectById(Guid id)
        {
            var project = await _projectManager.GetProjectByIdAsync(id);
            return Ok(project);
        }
        
        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<CreateProjectResponse> CreateProject([FromBody] CreateProjectRequest request)
        {
            var projectEntity = new ProjectEntity
            {
                Id = Guid.NewGuid(),
                ProjectName = request.ProjectName,
                ProjectDescription = request.ProjectDescription
            };

            await _projectManager.CreateProjectAsync(projectEntity);

            return new CreateProjectResponse { ProjectId = projectEntity.Id.ToString() };
        }
        
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<UpdateProjectResponse> UpdateProject(Guid id, [FromBody] UpdateProjectRequest request)
        {
            var projectEntity = new ProjectEntity
            {
                Id = id,
                ProjectName = request.ProjectName,
                ProjectDescription = request.ProjectDescription
            };
            await _projectManager.UpdateProjectAsync(projectEntity);
            return new UpdateProjectResponse();
        }
    
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteProject(Guid id)
        {
            await _projectManager.DeleteProjectAsync(id);
            return NoContent();
        }
    }
}
