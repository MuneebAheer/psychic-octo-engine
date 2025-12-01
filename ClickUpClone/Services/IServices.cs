using ClickUpClone.Models;
using ClickUpClone.DTOs;

namespace ClickUpClone.Services
{
    public interface IAuthService
    {
        Task<(bool Success, string Message, ApplicationUser? User)> RegisterAsync(RegisterDto dto);
        Task<(bool Success, string Message)> LoginAsync(LoginDto dto);
        Task LogoutAsync();
        Task<ApplicationUser?> GetCurrentUserAsync(string userId);
    }

    public interface IWorkspaceService
    {
        Task<WorkspaceDto?> GetWorkspaceAsync(int id);
        Task<IEnumerable<WorkspaceDto>> GetUserWorkspacesAsync(string userId);
        Task<WorkspaceDto> CreateWorkspaceAsync(CreateWorkspaceDto dto, string ownerId);
        Task<WorkspaceDto> UpdateWorkspaceAsync(int id, UpdateWorkspaceDto dto, string userId);
        Task<bool> DeleteWorkspaceAsync(int id, string userId);
        Task<IEnumerable<WorkspaceUserDto>> GetWorkspaceUsersAsync(int workspaceId, string userId);
        Task<(bool Success, string Message)> InviteUserAsync(int workspaceId, InviteUserDto dto, string userId);
        Task<(bool Success, string Message)> RemoveUserAsync(int workspaceId, int workspaceUserId, string userId);
        Task<(bool Success, string Message)> UpdateUserRoleAsync(int workspaceUserId, WorkspaceRole role, string userId);
    }

    public interface IProjectService
    {
        Task<ProjectDto?> GetProjectAsync(int id);
        Task<IEnumerable<ProjectDto>> GetWorkspaceProjectsAsync(int workspaceId);
        Task<ProjectDto> CreateProjectAsync(CreateProjectDto dto, string userId);
        Task<ProjectDto> UpdateProjectAsync(int id, UpdateProjectDto dto, string userId);
        Task<bool> DeleteProjectAsync(int id, string userId);
    }

    public interface ITaskListService
    {
        Task<TaskListDto?> GetTaskListAsync(int id);
        Task<IEnumerable<TaskListDto>> GetProjectTaskListsAsync(int projectId);
        Task<TaskListDto> CreateTaskListAsync(CreateTaskListDto dto, string userId);
        Task<TaskListDto> UpdateTaskListAsync(int id, UpdateTaskListDto dto, string userId);
        Task<bool> DeleteTaskListAsync(int id, string userId);
    }

    public interface ITaskService
    {
        Task<TaskDto?> GetTaskAsync(int id);
        Task<IEnumerable<TaskDto>> GetListTasksAsync(int listId);
        Task<IEnumerable<TaskDto>> GetProjectTasksAsync(int projectId);
        Task<IEnumerable<TaskDto>> GetUserTasksAsync(string userId);
        Task<IEnumerable<TaskDto>> GetFilteredTasksAsync(int projectId, TaskStatus? status = null, TaskPriority? priority = null, string? assignedTo = null);
        Task<TaskDto> CreateTaskAsync(CreateTaskDto dto, string userId);
        Task<TaskDto> UpdateTaskAsync(int id, UpdateTaskDto dto, string userId);
        Task<TaskDto> UpdateTaskStatusAsync(int id, TaskStatus status, string userId);
        Task<TaskDto> UpdateTaskPriorityAsync(int id, TaskPriority priority, string userId);
        Task<TaskDto> AssignTaskAsync(int id, string? assignedToId, string userId);
        Task<bool> DeleteTaskAsync(int id, string userId);
    }

    public interface ISubtaskService
    {
        Task<SubtaskDto?> GetSubtaskAsync(int id);
        Task<IEnumerable<SubtaskDto>> GetTaskSubtasksAsync(int taskId);
        Task<SubtaskDto> CreateSubtaskAsync(CreateSubtaskDto dto, string userId);
        Task<SubtaskDto> UpdateSubtaskAsync(int id, bool isCompleted, string userId);
        Task<bool> DeleteSubtaskAsync(int id, string userId);
    }

    public interface ICommentService
    {
        Task<CommentDto?> GetCommentAsync(int id);
        Task<IEnumerable<CommentDto>> GetTaskCommentsAsync(int taskId);
        Task<CommentDto> CreateCommentAsync(CreateCommentDto dto, string userId);
        Task<CommentDto> UpdateCommentAsync(int id, string content, string userId);
        Task<bool> DeleteCommentAsync(int id, string userId);
    }

    public interface IAttachmentService
    {
        Task<AttachmentDto> UploadFileAsync(IFormFile file, int taskId, string userId);
        Task<bool> DeleteFileAsync(int id, string userId);
        Task<IEnumerable<AttachmentDto>> GetTaskAttachmentsAsync(int taskId);
    }

    public interface IActivityLogService
    {
        Task LogActivityAsync(ActivityLog activity);
        Task<IEnumerable<ActivityLog>> GetWorkspaceActivityAsync(int workspaceId);
        Task<IEnumerable<ActivityLog>> GetProjectActivityAsync(int projectId);
        Task<IEnumerable<ActivityLog>> GetTaskActivityAsync(int taskId);
    }

    public interface INotificationService
    {
        Task<IEnumerable<Notification>> GetUserNotificationsAsync(string userId);
        Task<int> GetUnreadNotificationCountAsync(string userId);
        Task<Notification> CreateNotificationAsync(Notification notification);
        Task<bool> MarkAsReadAsync(int notificationId);
        Task<bool> MarkAllAsReadAsync(string userId);
        Task<bool> DeleteNotificationAsync(int id);
    }
}
