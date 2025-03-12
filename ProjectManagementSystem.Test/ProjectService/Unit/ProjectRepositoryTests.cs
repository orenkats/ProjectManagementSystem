using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using ProjectManagementSystem.API.ProjectService.Database;
using ProjectManagementSystem.Common.Exceptions;
using ProjectManagementSystem.Infrastructure.Database;

namespace ProjectManagementSystem.Test.ProjectService.Unit
{
    [TestFixture]
    public class ProjectRepositoryTests
    {
        private Mock<DbSet<ProjectEntity>> _mockProjectsDbSet;
        private Mock<ApplicationContext> _mockContext;
        private Mock<ILogger<ProjectRepository>> _mockLogger;
        private ProjectRepository _repository;

        [SetUp]
        public void SetUp()
        {
            _mockProjectsDbSet = new Mock<DbSet<ProjectEntity>>();
            _mockContext = new Mock<ApplicationContext>();
            _mockLogger = new Mock<ILogger<ProjectRepository>>();
            _repository = new ProjectRepository(_mockContext.Object, _mockLogger.Object);
        }

        [Test]
        public async Task GetByIdAsync_ProjectExists_ReturnsProject()
        {
            // Arrange
            var projectId = Guid.NewGuid();
            var project = new ProjectEntity { Id = projectId, ProjectName = "Test Project" };

            _mockProjectsDbSet.Setup(m => m.FindAsync(projectId))
                              .ReturnsAsync(project);

            _mockContext.Setup(c => c.Projects).Returns(_mockProjectsDbSet.Object);

            // Act
            var result = await _repository.GetByIdAsync(projectId);

            // Assert
            Assert.AreEqual(projectId, result.Id);
            Assert.AreEqual("Test Project", result.ProjectName);
        }

        [Test]
        public void GetByIdAsync_ProjectNotFound_ThrowsException()
        {
            // Arrange
            var projectId = Guid.NewGuid();

            _mockProjectsDbSet.Setup(m => m.FindAsync(projectId))
                              .ReturnsAsync((ProjectEntity)null);

            _mockContext.Setup(c => c.Projects).Returns(_mockProjectsDbSet.Object);

            // Act & Assert
            Assert.ThrowsAsync<EntityNotFoundException>(() => _repository.GetByIdAsync(projectId));
        }

        [Test]
        public async Task GetAllAsync_ReturnsPagedResults()
        {
            // Arrange
            var projects = new List<ProjectEntity>
            {
                new ProjectEntity { Id = Guid.NewGuid(), ProjectName = "Project 1" },
                new ProjectEntity { Id = Guid.NewGuid(), ProjectName = "Project 2" },
                new ProjectEntity { Id = Guid.NewGuid(), ProjectName = "Project 3" }
            }.AsQueryable();

            _mockProjectsDbSet.As<IQueryable<ProjectEntity>>().Setup(m => m.Provider).Returns(projects.Provider);
            _mockProjectsDbSet.As<IQueryable<ProjectEntity>>().Setup(m => m.Expression).Returns(projects.Expression);
            _mockProjectsDbSet.As<IQueryable<ProjectEntity>>().Setup(m => m.ElementType).Returns(projects.ElementType);
            _mockProjectsDbSet.As<IQueryable<ProjectEntity>>().Setup(m => m.GetEnumerator()).Returns(projects.GetEnumerator());

            _mockContext.Setup(c => c.Projects).Returns(_mockProjectsDbSet.Object);

            // Act
            var result = await _repository.GetAllAsync(1, 2);

            // Assert
            Assert.AreEqual(2, result.Count());
        }

        [Test]
        public async Task AddAsync_AddsProject()
        {
            // Arrange
            var project = new ProjectEntity { Id = Guid.NewGuid(), ProjectName = "New Project" };

            _mockProjectsDbSet.Setup(m => m.AddAsync(It.IsAny<ProjectEntity>(), CancellationToken.None))
                              .ReturnsAsync(new Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<ProjectEntity>(null));

            _mockContext.Setup(c => c.Projects).Returns(_mockProjectsDbSet.Object);
            _mockContext.Setup(c => c.SaveChangesAsync(CancellationToken.None)).ReturnsAsync(1);

            // Act
            await _repository.AddAsync(project);

            // Assert
            _mockProjectsDbSet.Verify(m => m.AddAsync(It.Is<ProjectEntity>(p => p.ProjectName == "New Project"), CancellationToken.None), Times.Once);
            _mockContext.Verify(c => c.SaveChangesAsync(CancellationToken.None), Times.Once);
        }

        [Test]
        public async Task UpdateAsync_UpdatesExistingProject()
        {
            // Arrange
            var existingProject = new ProjectEntity { Id = Guid.NewGuid(), ProjectName = "Old Project" };

            _mockProjectsDbSet.Setup(m => m.FindAsync(existingProject.Id))
                              .ReturnsAsync(existingProject);

            _mockContext.Setup(c => c.Projects).Returns(_mockProjectsDbSet.Object);
            _mockContext.Setup(c => c.SaveChangesAsync(CancellationToken.None)).ReturnsAsync(1);

            var updatedProject = new ProjectEntity { Id = existingProject.Id, ProjectName = "Updated Project" };

            // Act
            await _repository.UpdateAsync(updatedProject);

            // Assert
            _mockProjectsDbSet.Verify(m => m.Update(It.Is<ProjectEntity>(p => p.ProjectName == "Updated Project")), Times.Once);
            _mockContext.Verify(c => c.SaveChangesAsync(CancellationToken.None), Times.Once);
        }

        [Test]
        public async Task DeleteAsync_DeletesProjectIfExists()
        {
            // Arrange
            var existingProject = new ProjectEntity { Id = Guid.NewGuid(), ProjectName = "Project to Delete" };

            _mockProjectsDbSet.Setup(m => m.FindAsync(existingProject.Id))
                              .ReturnsAsync(existingProject);

            _mockContext.Setup(c => c.Projects).Returns(_mockProjectsDbSet.Object);
            _mockContext.Setup(c => c.SaveChangesAsync(CancellationToken.None)).ReturnsAsync(1);

            // Act
            await _repository.DeleteAsync(existingProject.Id);

            // Assert
            _mockProjectsDbSet.Verify(m => m.Remove(It.Is<ProjectEntity>(p => p.Id == existingProject.Id)), Times.Once);
            _mockContext.Verify(c => c.SaveChangesAsync(CancellationToken.None), Times.Once);
        }

        [Test]
        public void DeleteAsync_ThrowsIfProjectNotFound()
        {
            // Arrange
            var projectId = Guid.NewGuid();

            _mockProjectsDbSet.Setup(m => m.FindAsync(projectId))
                              .ReturnsAsync((ProjectEntity)null);

            _mockContext.Setup(c => c.Projects).Returns(_mockProjectsDbSet.Object);

            // Act & Assert
            Assert.ThrowsAsync<EntityNotFoundException>(() => _repository.DeleteAsync(projectId));
        }
    }
}
