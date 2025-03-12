namespace ProjectManagementSystem.Infrastructure.Database;

public class ProjectEntity 
{
    public Guid Id { get; set; } 
    public string ProjectName { get; set; }
    public string ProjectDescription { get; set; }
    public List<TaskEntity> Tasks { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;  
    public DateTime? UpdatedDate { get; set; }  
}