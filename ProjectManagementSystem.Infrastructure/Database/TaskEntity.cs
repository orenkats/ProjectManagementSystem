namespace ProjectManagementSystem.Infrastructure.Database;

public class TaskEntity
{
    public Guid Id { get; set; }
    public Guid ProjectId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string Status { get; set; }  
    public ProjectEntity Project { get; set; } 
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedDate { get; set; }
}