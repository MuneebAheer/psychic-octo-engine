using ClickUpClone.Models;
using ClickUpClone.DTOs;
using ClickUpClone.Repositories;

namespace ClickUpClone.Services
{
    public class ProjectService : IProjectService
    {
        private readonly IProjectRepository _projectRepository;
        private readonly IWorkspaceRepository _workspaceRepository;
        private readonly IActivityLogRepository _activityLogRepository;

        public ProjectService(
            IProjectRepository projectRepository,
            IWorkspaceRepository workspaceRepository,
            IActivityLogRepository activityLogRepository)
        {
            _projectRepository = projectRepository;
            _workspaceRepository = workspaceRepository;
            _activityLogRepository = activityLogRepository;
        }

        public async Task<ProjectDto?> GetProjectAsync(int id)
        {
            var project = await _projectRepository.GetByIdAsync(id);
            if (project == null) return null;

            return MapToDto(project);
        }

        public async Task<IEnumerable<ProjectDto>> GetWorkspaceProjectsAsync(int workspaceId)
        {
            var projects = await _projectRepository.GetWorkspaceProjectsAsync(workspaceId);
            return projects.Select(MapToDto);
        }

        public async Task<ProjectDto> CreateProjectAsync(CreateProjectDto dto, string userId)
        {
            var project = new Project
            {
                Name = dto.Name,
                Description = dto.Description,
                Color = dto.Color,
                WorkspaceId = dto.WorkspaceId,
                CreatedById = userId
            };

            var created = await _projectRepository.CreateAsync(project);

            await _activityLogRepository.CreateAsync(new ActivityLog
            {
                Type = ActivityType.Created,
                Description = $"Created project '{created.Name}'",
                UserId = userId,
                WorkspaceId = dto.WorkspaceId,
                ProjectId = created.Id
            });

            return MapToDto(created);
        }

        public async Task<ProjectDto> UpdateProjectAsync(int id, UpdateProjectDto dto, string userId)
        {
            var project = await _projectRepository.GetByIdAsync(id);
            if (project == null)
                throw new InvalidOperationException("Project not found");

            project.Name = dto.Name;
            project.Description = dto.Description;
            project.Color = dto.Color;

            var updated = await _projectRepository.UpdateAsync(project);

            await _activityLogRepository.CreateAsync(new ActivityLog
            {
                Type = ActivityType.Updated,
                Description = $"Updated project",
                UserId = userId,
                ProjectId = id
            });

            return MapToDto(updated);
        }

        public async Task<bool> DeleteProjectAsync(int id, string userId)
        {
            var project = await _projectRepository.GetByIdAsync(id);
            if (project == null)
                throw new InvalidOperationException("Project not found");

            await _activityLogRepository.CreateAsync(new ActivityLog
            {
                Type = ActivityType.Deleted,
                Description = $"Deleted project",
                UserId = userId,
                ProjectId = id
            });

            return await _projectRepository.DeleteAsync(id);
        }

        private ProjectDto MapToDto(Project project)
        {
            return new ProjectDto
            {
                Id = project.Id,
                Name = project.Name,
                Description = project.Description,
                Color = project.Color,
                WorkspaceId = project.WorkspaceId,
                CreatedByName = $"{project.CreatedBy?.FirstName} {project.CreatedBy?.LastName}",
                CreatedAt = project.CreatedAt
            };
        }
    }

    public class TaskListService : ITaskListService
    {
        private readonly ITaskListRepository _taskListRepository;
        private readonly IProjectRepository _projectRepository;
        private readonly IActivityLogRepository _activityLogRepository;

        public TaskListService(
            ITaskListRepository taskListRepository,
            IProjectRepository projectRepository,
            IActivityLogRepository activityLogRepository)
        {
            _taskListRepository = taskListRepository;
            _projectRepository = projectRepository;
            _activityLogRepository = activityLogRepository;
        }

        public async Task<TaskListDto?> GetTaskListAsync(int id)
        {
            var taskList = await _taskListRepository.GetByIdAsync(id);
            if (taskList == null) return null;

            return MapToDto(taskList);
        }

        public async Task<IEnumerable<TaskListDto>> GetProjectTaskListsAsync(int projectId)
        {
            var taskLists = await _taskListRepository.GetProjectTaskListsAsync(projectId);
            return taskLists.Select(MapToDto);
        }

        public async Task<TaskListDto> CreateTaskListAsync(CreateTaskListDto dto, string userId)
        {
            var project = await _projectRepository.GetByIdAsync(dto.ProjectId);
            if (project == null)
                throw new InvalidOperationException("Project not found");

            var existingLists = await _taskListRepository.GetProjectTaskListsAsync(dto.ProjectId);
            var maxOrder = existingLists.Max(l => (int?)l.Order) ?? 0;

            var taskList = new TaskList
            {
                Name = dto.Name,
                Description = dto.Description,
                Color = dto.Color,
                ProjectId = dto.ProjectId,
                Order = maxOrder + 1
            };

            var created = await _taskListRepository.CreateAsync(taskList);

            await _activityLogRepository.CreateAsync(new ActivityLog
            {
                Type = ActivityType.Created,
                Description = $"Created task list '{created.Name}'",
                UserId = userId,
                ProjectId = dto.ProjectId
            });

            return MapToDto(created);
        }

        public async Task<TaskListDto> UpdateTaskListAsync(int id, UpdateTaskListDto dto, string userId)
        {
            var taskList = await _taskListRepository.GetByIdAsync(id);
            if (taskList == null)
                throw new InvalidOperationException("Task list not found");

            taskList.Name = dto.Name;
            taskList.Description = dto.Description;
            taskList.Color = dto.Color;
            taskList.Order = dto.Order;

            var updated = await _taskListRepository.UpdateAsync(taskList);

            await _activityLogRepository.CreateAsync(new ActivityLog
            {
                Type = ActivityType.Updated,
                Description = $"Updated task list",
                UserId = userId,
                ProjectId = taskList.ProjectId
            });

            return MapToDto(updated);
        }

        public async Task<bool> DeleteTaskListAsync(int id, string userId)
        {
            var taskList = await _taskListRepository.GetByIdAsync(id);
            if (taskList == null)
                throw new InvalidOperationException("Task list not found");

            await _activityLogRepository.CreateAsync(new ActivityLog
            {
                Type = ActivityType.Deleted,
                Description = $"Deleted task list",
                UserId = userId,
                ProjectId = taskList.ProjectId
            });

            return await _taskListRepository.DeleteAsync(id);
        }

        private TaskListDto MapToDto(TaskList taskList)
        {
            return new TaskListDto
            {
                Id = taskList.Id,
                Name = taskList.Name,
                Description = taskList.Description,
                Color = taskList.Color,
                ProjectId = taskList.ProjectId,
                Order = taskList.Order,
                IsActive = taskList.IsActive
            };
        }
    }
}
