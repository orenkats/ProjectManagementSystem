namespace ProjectManagementSystem.Common.Contracts
{
    public class UpdateProjectRequest
    {
        public Guid Id { get; set; }
        public string ProjectName { get; set; }
        public string ProjectDescription { get; set; }
    }
}