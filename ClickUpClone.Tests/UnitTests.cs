using Xunit;
using Moq;
using ClickUpClone.Repositories;
using ClickUpClone.Services;
using ClickUpClone.Models;
using ClickUpClone.DTOs;

namespace ClickUpClone.Tests
{
    public class WorkspaceServiceTests
    {
        private readonly Mock<IWorkspaceRepository> _mockWorkspaceRepo;
        private readonly Mock<IWorkspaceUserRepository> _mockWorkspaceUserRepo;
        private readonly Mock<IActivityLogRepository> _mockActivityLogRepo;
        private readonly WorkspaceService _service;

        public WorkspaceServiceTests()
        {
            _mockWorkspaceRepo = new Mock<IWorkspaceRepository>();
            _mockWorkspaceUserRepo = new Mock<IWorkspaceUserRepository>();
            _mockActivityLogRepo = new Mock<IActivityLogRepository>();
            _service = new WorkspaceService(
                _mockWorkspaceRepo.Object,
                _mockWorkspaceUserRepo.Object,
                _mockActivityLogRepo.Object,
                null!);
        }

        [Fact]
        public async Task GetWorkspaceAsync_WithValidId_ReturnsWorkspaceDto()
        {
            // Arrange
            var workspaceId = 1;
            var workspace = new Workspace
            {
                Id = workspaceId,
                Name = "Test Workspace",
                Description = "Test Description",
                OwnerId = "user1",
                Owner = new ApplicationUser { Id = "user1", FirstName = "John", LastName = "Doe" },
                WorkspaceUsers = new List<WorkspaceUser> { new WorkspaceUser { IsActive = true } }
            };

            _mockWorkspaceRepo.Setup(r => r.GetByIdAsync(workspaceId))
                .ReturnsAsync(workspace);

            // Act
            var result = await _service.GetWorkspaceAsync(workspaceId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(workspaceId, result.Id);
            Assert.Equal("Test Workspace", result.Name);
        }

        [Fact]
        public async Task GetWorkspaceAsync_WithInvalidId_ReturnsNull()
        {
            // Arrange
            _mockWorkspaceRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((Workspace?)null);

            // Act
            var result = await _service.GetWorkspaceAsync(999);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task CreateWorkspaceAsync_WithValidData_CreatesWorkspace()
        {
            // Arrange
            var userId = "user1";
            var createDto = new CreateWorkspaceDto
            {
                Name = "New Workspace",
                Description = "New Description",
                Color = "#FF0000"
            };

            var workspace = new Workspace
            {
                Id = 1,
                Name = createDto.Name,
                Description = createDto.Description,
                Color = createDto.Color,
                OwnerId = userId,
                Owner = new ApplicationUser { Id = userId, FirstName = "John", LastName = "Doe" }
            };

            _mockWorkspaceRepo.Setup(r => r.CreateAsync(It.IsAny<Workspace>()))
                .ReturnsAsync(workspace);

            _mockWorkspaceUserRepo.Setup(r => r.AddUserAsync(It.IsAny<WorkspaceUser>()))
                .ReturnsAsync(new WorkspaceUser { Id = 1, UserId = userId });

            _mockActivityLogRepo.Setup(r => r.CreateAsync(It.IsAny<ActivityLog>()))
                .ReturnsAsync(new ActivityLog { Id = 1 });

            // Act
            var result = await _service.CreateWorkspaceAsync(createDto, userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("New Workspace", result.Name);
            _mockWorkspaceRepo.Verify(r => r.CreateAsync(It.IsAny<Workspace>()), Times.Once);
        }
    }

    public class TaskServiceTests
    {
        private readonly Mock<ITaskRepository> _mockTaskRepo;
        private readonly Mock<IListRepository> _mockListRepo;
        private readonly Mock<IActivityLogRepository> _mockActivityLogRepo;
        private readonly TaskService _service;

        public TaskServiceTests()
        {
            _mockTaskRepo = new Mock<ITaskRepository>();
            _mockListRepo = new Mock<IListRepository>();
            _mockActivityLogRepo = new Mock<IActivityLogRepository>();
            _service = new TaskService(
                _mockTaskRepo.Object,
                _mockListRepo.Object,
                _mockActivityLogRepo.Object);
        }

        [Fact]
        public async Task GetTaskAsync_WithValidId_ReturnsTaskDto()
        {
            // Arrange
            var taskId = 1;
            var task = new Models.Task
            {
                Id = taskId,
                Title = "Test Task",
                Description = "Test Description",
                Status = TaskStatus.ToDo,
                Priority = TaskPriority.Normal,
                Subtasks = new List<Subtask>()
            };

            _mockTaskRepo.Setup(r => r.GetByIdAsync(taskId))
                .ReturnsAsync(task);

            // Act
            var result = await _service.GetTaskAsync(taskId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(taskId, result.Id);
            Assert.Equal("Test Task", result.Title);
        }

        [Fact]
        public async Task CreateTaskAsync_WithValidData_CreatesTask()
        {
            // Arrange
            var userId = "user1";
            var createDto = new CreateTaskDto
            {
                Title = "New Task",
                Description = "New Description",
                ListId = 1,
                Priority = TaskPriority.High
            };

            var list = new List { Id = 1, ProjectId = 1 };
            var task = new Models.Task
            {
                Id = 1,
                Title = createDto.Title,
                Description = createDto.Description,
                ListId = createDto.ListId,
                ProjectId = list.ProjectId,
                Priority = createDto.Priority,
                Subtasks = new List<Subtask>(),
                Status = TaskStatus.ToDo
            };

            _mockListRepo.Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(list);

            _mockTaskRepo.Setup(r => r.GetListTasksAsync(1))
                .ReturnsAsync(new List<Models.Task>());

            _mockTaskRepo.Setup(r => r.CreateAsync(It.IsAny<Models.Task>()))
                .ReturnsAsync(task);

            _mockActivityLogRepo.Setup(r => r.CreateAsync(It.IsAny<ActivityLog>()))
                .ReturnsAsync(new ActivityLog { Id = 1 });

            // Act
            var result = await _service.CreateTaskAsync(createDto, userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("New Task", result.Title);
            _mockTaskRepo.Verify(r => r.CreateAsync(It.IsAny<Models.Task>()), Times.Once);
        }
    }

    public class RepositoryTests
    {
        [Fact]
        public void Workspace_Repository_Should_Have_Required_Methods()
        {
            // Arrange
            var repositoryType = typeof(IWorkspaceRepository);

            // Assert
            Assert.True(repositoryType.GetMethod("GetByIdAsync") != null);
            Assert.True(repositoryType.GetMethod("GetUserWorkspacesAsync") != null);
            Assert.True(repositoryType.GetMethod("CreateAsync") != null);
            Assert.True(repositoryType.GetMethod("UpdateAsync") != null);
            Assert.True(repositoryType.GetMethod("DeleteAsync") != null);
        }

        [Fact]
        public void Task_Repository_Should_Have_Required_Methods()
        {
            // Arrange
            var repositoryType = typeof(ITaskRepository);

            // Assert
            Assert.True(repositoryType.GetMethod("GetByIdAsync") != null);
            Assert.True(repositoryType.GetMethod("GetListTasksAsync") != null);
            Assert.True(repositoryType.GetMethod("GetProjectTasksAsync") != null);
            Assert.True(repositoryType.GetMethod("CreateAsync") != null);
            Assert.True(repositoryType.GetMethod("UpdateAsync") != null);
            Assert.True(repositoryType.GetMethod("DeleteAsync") != null);
        }
    }
}
