namespace ProjectManagementSystem.Common.Contracts
{
    public class CreateTaskRequest
    {
        public Guid ProjectId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
    }
}