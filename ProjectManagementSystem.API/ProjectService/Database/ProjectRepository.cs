using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProjectManagementSystem.Common.Exceptions;
using ProjectManagementSystem.Infrastructure.Database;

namespace ProjectManagementSystem.API.ProjectService.Database;

public class ProjectRepository : IProjectRepository
{
    private readonly ApplicationContext _context;
    private readonly ILogger<ProjectRepository> _logger;

    public ProjectRepository(ApplicationContext context, ILogger<ProjectRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<ProjectEntity> GetByIdAsync(Guid projectId)
    {
        var project = await _context.Projects.FirstOrDefaultAsync(p => p.Id == projectId);
        if (project == null)
        {
            throw new EntityNotFoundException(projectId.ToString());
        }
        _logger.LogInformation($"Retrieved Project: ID={projectId}");
        return project;
    }

    public async Task<IEnumerable<ProjectEntity>> GetAllAsync(int pageNumber, int pageSize)
    {
        var projects = await _context.Projects
            .OrderBy(p => p.ProjectName)  
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        _logger.LogInformation($"Retrieved page {pageNumber} of projects with size {pageSize}. Count={projects.Count}");
        return projects;
    }
    
    public async Task AddAsync(ProjectEntity project)
    {
        await _context.Projects.AddAsync(project);
        await _context.SaveChangesAsync();
        _logger.LogInformation($"Created Project: ID={project.Id}, Name={project.ProjectName}");
    }

    public async Task UpdateAsync(ProjectEntity project)
    {
        var existingProject = await _context.Projects.FindAsync(project.Id);
        if (existingProject == null)
        {
            throw new EntityNotFoundException(project.Id.ToString());
        }
        _context.Projects.Update(project);
        await _context.SaveChangesAsync();
        _logger.LogInformation($"Updated Project: ID={project.Id}, Name={project.ProjectName}");
    }

    public async Task DeleteAsync(Guid projectId)
    {
        var project = await _context.Projects.FindAsync(projectId);
        if (project == null)
        {
            throw new EntityNotFoundException(projectId.ToString());
        }
        _context.Projects.Remove(project);
        await _context.SaveChangesAsync();
        _logger.LogInformation($"Deleted Project: ID={projectId}");
    }
}

