using ClickUpClone.Models;

namespace ClickUpClone.Repositories
{
    public interface IWorkspaceRepository
    {
        Task<Workspace?> GetByIdAsync(int id);
        Task<IEnumerable<Workspace>> GetUserWorkspacesAsync(string userId);
        Task<IEnumerable<Workspace>> GetAllAsync();
        Task<Workspace> CreateAsync(Workspace workspace);
        Task<Workspace> UpdateAsync(Workspace workspace);
        Task<bool> DeleteAsync(int id);
    }

    public interface IWorkspaceUserRepository
    {
        Task<WorkspaceUser?> GetByIdAsync(int id);
        Task<WorkspaceUser?> GetByWorkspaceAndUserAsync(int workspaceId, string userId);
        Task<IEnumerable<WorkspaceUser>> GetWorkspaceUsersAsync(int workspaceId);
        Task<WorkspaceUser> AddUserAsync(WorkspaceUser workspaceUser);
        Task<WorkspaceUser> UpdateAsync(WorkspaceUser workspaceUser);
        Task<bool> RemoveUserAsync(int id);
    }

    public interface IProjectRepository
    {
        Task<Project?> GetByIdAsync(int id);
        Task<IEnumerable<Project>> GetWorkspaceProjectsAsync(int workspaceId);
        Task<Project> CreateAsync(Project project);
        Task<Project> UpdateAsync(Project project);
        Task<bool> DeleteAsync(int id);
    }

    public interface ITaskListRepository
    {
        Task<TaskList?> GetByIdAsync(int id);
        Task<IEnumerable<TaskList>> GetProjectTaskListsAsync(int projectId);
        Task<TaskList> CreateAsync(TaskList taskList);
        Task<TaskList> UpdateAsync(TaskList taskList);
        Task<bool> DeleteAsync(int id);
    }

    public interface ITaskRepository
    {
        Task<Models.Task?> GetByIdAsync(int id);
        Task<IEnumerable<Models.Task>> GetListTasksAsync(int listId);
        Task<IEnumerable<Models.Task>> GetProjectTasksAsync(int projectId);
        Task<IEnumerable<Models.Task>> GetUserTasksAsync(string userId);
        Task<IEnumerable<Models.Task>> GetTasksByStatusAsync(int projectId, TaskStatus status);
        Task<Models.Task> CreateAsync(Models.Task task);
        Task<Models.Task> UpdateAsync(Models.Task task);
        Task<bool> DeleteAsync(int id);
    }

    public interface ISubtaskRepository
    {
        Task<Subtask?> GetByIdAsync(int id);
        Task<IEnumerable<Subtask>> GetTaskSubtasksAsync(int taskId);
        Task<Subtask> CreateAsync(Subtask subtask);
        Task<Subtask> UpdateAsync(Subtask subtask);
        Task<bool> DeleteAsync(int id);
    }

    public interface ICommentRepository
    {
        Task<Comment?> GetByIdAsync(int id);
        Task<IEnumerable<Comment>> GetTaskCommentsAsync(int taskId);
        Task<Comment> CreateAsync(Comment comment);
        Task<Comment> UpdateAsync(Comment comment);
        Task<bool> DeleteAsync(int id);
    }

    public interface IAttachmentRepository
    {
        Task<Attachment?> GetByIdAsync(int id);
        Task<IEnumerable<Attachment>> GetTaskAttachmentsAsync(int taskId);
        Task<Attachment> CreateAsync(Attachment attachment);
        Task<bool> DeleteAsync(int id);
    }

    public interface IActivityLogRepository
    {
        Task<ActivityLog?> GetByIdAsync(int id);
        Task<IEnumerable<ActivityLog>> GetWorkspaceActivityAsync(int workspaceId, int count = 50);
        Task<IEnumerable<ActivityLog>> GetProjectActivityAsync(int projectId, int count = 50);
        Task<IEnumerable<ActivityLog>> GetTaskActivityAsync(int taskId, int count = 50);
        Task<ActivityLog> CreateAsync(ActivityLog log);
    }

    public interface INotificationRepository
    {
        Task<Notification?> GetByIdAsync(int id);
        Task<IEnumerable<Notification>> GetUserNotificationsAsync(string userId);
        Task<IEnumerable<Notification>> GetUnreadNotificationsAsync(string userId);
        Task<Notification> CreateAsync(Notification notification);
        Task<Notification> UpdateAsync(Notification notification);
        Task<bool> DeleteAsync(int id);
    }
}
