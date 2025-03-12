using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using ProjectManagementSystem.API.TaskService.Database;
using ProjectManagementSystem.Common.Exceptions;
using ProjectManagementSystem.Infrastructure.Database;
using System.Linq.Expressions;

namespace ProjectManagementSystem.Test.TaskService.Unit
{
    [TestFixture]
    public class TaskRepositoryTests
    {
        private Mock<DbSet<TaskEntity>> _mockTasksDbSet;
        private Mock<ApplicationContext> _mockContext;
        private Mock<ILogger<TaskRepository>> _mockLogger;
        private TaskRepository _repository;

        [SetUp]
        public void SetUp()
        {
            _mockTasksDbSet = new Mock<DbSet<TaskEntity>>();
            _mockContext = new Mock<ApplicationContext>();
            _mockLogger = new Mock<ILogger<TaskRepository>>();
            _repository = new TaskRepository(_mockContext.Object, _mockLogger.Object);
        }

        [Test]
        public async Task GetByIdAsync_TaskExists_ReturnsTask()
        {
            // Arrange
            var taskId = Guid.NewGuid();
            var task = new TaskEntity { Id = taskId, Title = "Test Task" };

            _mockTasksDbSet.Setup(m => m.FindAsync(taskId))
                .ReturnsAsync(task);

            _mockContext.Setup(c => c.Tasks).Returns(_mockTasksDbSet.Object);

            // Act
            var result = await _repository.GetByIdAsync(taskId);

            // Assert
            Assert.AreEqual(taskId, result.Id);
            Assert.AreEqual("Test Task", result.Title);
        }

        [Test]
        public void GetByIdAsync_TaskNotFound_ThrowsException()
        {
            // Arrange
            var taskId = Guid.NewGuid();

            _mockTasksDbSet.Setup(m => m.FindAsync(taskId))
                .ReturnsAsync((TaskEntity)null);

            _mockContext.Setup(c => c.Tasks).Returns(_mockTasksDbSet.Object);

            // Act & Assert
            Assert.ThrowsAsync<EntityNotFoundException>(() => _repository.GetByIdAsync(taskId));
        }

        [Test]
        public async Task GetAllAsync_ReturnsPagedResults()
        {
            // Arrange
            var tasks = new List<TaskEntity>
            {
                new TaskEntity { Id = Guid.NewGuid(), Title = "Task 1" },
                new TaskEntity { Id = Guid.NewGuid(), Title = "Task 2" },
                new TaskEntity { Id = Guid.NewGuid(), Title = "Task 3" }
            }.AsQueryable();

            _mockTasksDbSet.As<IQueryable<TaskEntity>>().Setup(m => m.Provider).Returns(tasks.Provider);
            _mockTasksDbSet.As<IQueryable<TaskEntity>>().Setup(m => m.Expression).Returns(tasks.Expression);
            _mockTasksDbSet.As<IQueryable<TaskEntity>>().Setup(m => m.ElementType).Returns(tasks.ElementType);
            _mockTasksDbSet.As<IQueryable<TaskEntity>>().Setup(m => m.GetEnumerator()).Returns(tasks.GetEnumerator());

            _mockContext.Setup(c => c.Tasks).Returns(_mockTasksDbSet.Object);

            // Act
            var result = await _repository.GetAllAsync(1, 2);

            // Assert
            Assert.AreEqual(2, result.Count());
        }

        [Test]
        public async Task GetByProjectIdAsync_ProjectExists_ReturnsTasks()
        {
            // Arrange
            var projectId = Guid.NewGuid();
            var tasks = new List<TaskEntity>
            {
                new TaskEntity { Id = Guid.NewGuid(), ProjectId = projectId, Title = "Task 1" },
                new TaskEntity { Id = Guid.NewGuid(), ProjectId = projectId, Title = "Task 2" }
            }.AsQueryable();

            _mockTasksDbSet.As<IQueryable<TaskEntity>>().Setup(m => m.Provider).Returns(tasks.Provider);
            _mockTasksDbSet.As<IQueryable<TaskEntity>>().Setup(m => m.Expression).Returns(tasks.Expression);
            _mockTasksDbSet.As<IQueryable<TaskEntity>>().Setup(m => m.ElementType).Returns(tasks.ElementType);
            _mockTasksDbSet.As<IQueryable<TaskEntity>>().Setup(m => m.GetEnumerator()).Returns(tasks.GetEnumerator());

            _mockContext.Setup(c => c.Tasks).Returns(_mockTasksDbSet.Object);
            _mockTasksDbSet.Setup(m => m.AnyAsync(It.IsAny<Expression<Func<TaskEntity, bool>>>(), CancellationToken.None))
                .ReturnsAsync(true);

            // Act
            var result = await _repository.GetByProjectIdAsync(projectId, 1, 2);

            // Assert
            Assert.AreEqual(2, result.Count());
        }

        [Test]
        public void GetByProjectIdAsync_ProjectNotFound_ThrowsException()
        {
            // Arrange
            var projectId = Guid.NewGuid();

            _mockTasksDbSet.Setup(m => m.AnyAsync(It.IsAny<Expression<Func<TaskEntity, bool>>>(), CancellationToken.None))
                .ReturnsAsync(false);

            _mockContext.Setup(c => c.Tasks).Returns(_mockTasksDbSet.Object);

            // Act & Assert
            Assert.ThrowsAsync<EntityNotFoundException>(() => _repository.GetByProjectIdAsync(projectId, 1, 2));
        }

        [Test]
        public async Task AddAsync_AddsTask()
        {
            // Arrange
            var task = new TaskEntity { Id = Guid.NewGuid(), Title = "New Task" };

            _mockTasksDbSet.Setup(m => m.AddAsync(It.IsAny<TaskEntity>(), CancellationToken.None))
                .ReturnsAsync(new Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<TaskEntity>(null));

            _mockContext.Setup(c => c.Tasks).Returns(_mockTasksDbSet.Object);
            _mockContext.Setup(c => c.SaveChangesAsync(CancellationToken.None)).ReturnsAsync(1);

            // Act
            await _repository.AddAsync(task);

            // Assert
            _mockTasksDbSet.Verify(m => m.AddAsync(It.Is<TaskEntity>(t => t.Title == "New Task"), CancellationToken.None), Times.Once);
            _mockContext.Verify(c => c.SaveChangesAsync(CancellationToken.None), Times.Once);
        }

        [Test]
        public async Task UpdateAsync_UpdatesExistingTask()
        {
            // Arrange
            var existingTask = new TaskEntity { Id = Guid.NewGuid(), Title = "Old Task" };

            _mockTasksDbSet.Setup(m => m.FindAsync(existingTask.Id))
                .ReturnsAsync(existingTask);

            _mockContext.Setup(c => c.Tasks).Returns(_mockTasksDbSet.Object);
            _mockContext.Setup(c => c.SaveChangesAsync(CancellationToken.None)).ReturnsAsync(1);

            var updatedTask = new TaskEntity { Id = existingTask.Id, Title = "Updated Task" };

            // Act
            await _repository.UpdateAsync(updatedTask);

            // Assert
            _mockTasksDbSet.Verify(m => m.Update(It.Is<TaskEntity>(t => t.Title == "Updated Task")), Times.Once);
            _mockContext.Verify(c => c.SaveChangesAsync(CancellationToken.None), Times.Once);
        }

        [Test]
        public async Task DeleteAsync_DeletesTaskIfExists()
        {
            // Arrange
            var existingTask = new TaskEntity { Id = Guid.NewGuid(), Title = "Task to Delete" };

            _mockTasksDbSet.Setup(m => m.FindAsync(existingTask.Id))
                .ReturnsAsync(existingTask);

            _mockContext.Setup(c => c.Tasks).Returns(_mockTasksDbSet.Object);
            _mockContext.Setup(c => c.SaveChangesAsync(CancellationToken.None)).ReturnsAsync(1);

            // Act
            await _repository.DeleteAsync(existingTask.Id);

            // Assert
            _mockTasksDbSet.Verify(m => m.Remove(It.Is<TaskEntity>(t => t.Id == existingTask.Id)), Times.Once);
            _mockContext.Verify(c => c.SaveChangesAsync(CancellationToken.None), Times.Once);
        }

        [Test]
        public void DeleteAsync_ThrowsIfTaskNotFound()
        {
            // Arrange
            var taskId = Guid.NewGuid();

            _mockTasksDbSet.Setup(m => m.FindAsync(taskId))
                .ReturnsAsync((TaskEntity)null);

            _mockContext.Setup(c => c.Tasks).Returns(_mockTasksDbSet.Object);

            // Act & Assert
            Assert.ThrowsAsync<EntityNotFoundException>(() => _repository.DeleteAsync(taskId));
        }
    }
}
