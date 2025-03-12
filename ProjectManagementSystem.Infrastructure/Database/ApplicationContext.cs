using Microsoft.EntityFrameworkCore;

namespace ProjectManagementSystem.Infrastructure.Database
{
    public class ApplicationContext : DbContext
    {
        public DbSet<ProjectEntity> Projects { get; set; }
        public DbSet<TaskEntity> Tasks { get; set; }

        public ApplicationContext() { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var connectionString = "Server=sqlserver,1433;Database=ProjectManagementDB;User Id=sa;Password=YourStrongPassword!;TrustServerCertificate=True;";
            optionsBuilder.UseSqlServer(connectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure GUID keys and relationships
            modelBuilder.Entity<ProjectEntity>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.ToTable("Projects");
            });

            modelBuilder.Entity<TaskEntity>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasOne(t => t.Project)
                    .WithMany(p => p.Tasks)
                    .HasForeignKey(t => t.ProjectId);
                entity.ToTable("Tasks");
            });
        }
    }
}