using Microsoft.EntityFrameworkCore;
using ClickUpClone.Data;
using ClickUpClone.Models;

namespace ClickUpClone.Repositories
{
    public class WorkspaceRepository : IWorkspaceRepository
    {
        private readonly ApplicationDbContext _context;

        public WorkspaceRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Workspace?> GetByIdAsync(int id)
        {
            return await _context.Workspaces
                .Include(w => w.Owner)
                .Include(w => w.WorkspaceUsers)
                .FirstOrDefaultAsync(w => w.Id == id && w.IsActive);
        }

        public async Task<IEnumerable<Workspace>> GetUserWorkspacesAsync(string userId)
        {
            return await _context.Workspaces
                .Where(w => w.IsActive && (w.OwnerId == userId || w.WorkspaceUsers.Any(wu => wu.UserId == userId && wu.IsActive)))
                .Include(w => w.Owner)
                .Include(w => w.WorkspaceUsers)
                .OrderByDescending(w => w.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Workspace>> GetAllAsync()
        {
            return await _context.Workspaces
                .Where(w => w.IsActive)
                .Include(w => w.Owner)
                .OrderByDescending(w => w.CreatedAt)
                .ToListAsync();
        }

        public async Task<Workspace> CreateAsync(Workspace workspace)
        {
            _context.Workspaces.Add(workspace);
            await _context.SaveChangesAsync();
            return workspace;
        }

        public async Task<Workspace> UpdateAsync(Workspace workspace)
        {
            workspace.UpdatedAt = DateTime.UtcNow;
            _context.Workspaces.Update(workspace);
            await _context.SaveChangesAsync();
            return workspace;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var workspace = await _context.Workspaces.FindAsync(id);
            if (workspace == null) return false;

            workspace.IsActive = false;
            await _context.SaveChangesAsync();
            return true;
        }
    }

    public class WorkspaceUserRepository : IWorkspaceUserRepository
    {
        private readonly ApplicationDbContext _context;

        public WorkspaceUserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<WorkspaceUser?> GetByIdAsync(int id)
        {
            return await _context.WorkspaceUsers
                .Include(wu => wu.Workspace)
                .Include(wu => wu.User)
                .FirstOrDefaultAsync(wu => wu.Id == id && wu.IsActive);
        }

        public async Task<WorkspaceUser?> GetByWorkspaceAndUserAsync(int workspaceId, string userId)
        {
            return await _context.WorkspaceUsers
                .Include(wu => wu.Workspace)
                .Include(wu => wu.User)
                .FirstOrDefaultAsync(wu => wu.WorkspaceId == workspaceId && wu.UserId == userId && wu.IsActive);
        }

        public async Task<IEnumerable<WorkspaceUser>> GetWorkspaceUsersAsync(int workspaceId)
        {
            return await _context.WorkspaceUsers
                .Where(wu => wu.WorkspaceId == workspaceId && wu.IsActive)
                .Include(wu => wu.User)
                .OrderBy(wu => wu.JoinedAt)
                .ToListAsync();
        }

        public async Task<WorkspaceUser> AddUserAsync(WorkspaceUser workspaceUser)
        {
            _context.WorkspaceUsers.Add(workspaceUser);
            await _context.SaveChangesAsync();
            return workspaceUser;
        }

        public async Task<WorkspaceUser> UpdateAsync(WorkspaceUser workspaceUser)
        {
            _context.WorkspaceUsers.Update(workspaceUser);
            await _context.SaveChangesAsync();
            return workspaceUser;
        }

        public async Task<bool> RemoveUserAsync(int id)
        {
            var workspaceUser = await _context.WorkspaceUsers.FindAsync(id);
            if (workspaceUser == null) return false;

            workspaceUser.IsActive = false;
            await _context.SaveChangesAsync();
            return true;
        }
    }

    public class ProjectRepository : IProjectRepository
    {
        private readonly ApplicationDbContext _context;

        public ProjectRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Project?> GetByIdAsync(int id)
        {
            return await _context.Projects
                .Include(p => p.Workspace)
                .Include(p => p.CreatedBy)
                .FirstOrDefaultAsync(p => p.Id == id && !p.IsArchived);
        }

        public async Task<IEnumerable<Project>> GetWorkspaceProjectsAsync(int workspaceId)
        {
            return await _context.Projects
                .Where(p => p.WorkspaceId == workspaceId && !p.IsArchived)
                .Include(p => p.CreatedBy)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        public async Task<Project> CreateAsync(Project project)
        {
            _context.Projects.Add(project);
            await _context.SaveChangesAsync();
            return project;
        }

        public async Task<Project> UpdateAsync(Project project)
        {
            project.UpdatedAt = DateTime.UtcNow;
            _context.Projects.Update(project);
            await _context.SaveChangesAsync();
            return project;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var project = await _context.Projects.FindAsync(id);
            if (project == null) return false;

            project.IsArchived = true;
            await _context.SaveChangesAsync();
            return true;
        }
    }

    public class TaskListRepository : ITaskListRepository
    {
        private readonly ApplicationDbContext _context;

        public TaskListRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<TaskList?> GetByIdAsync(int id)
        {
            return await _context.TaskLists
                .Include(l => l.Project)
                .Include(l => l.Tasks)
                .FirstOrDefaultAsync(l => l.Id == id && l.IsActive);
        }

        public async Task<IEnumerable<TaskList>> GetProjectTaskListsAsync(int projectId)
        {
            return await _context.TaskLists
                .Where(l => l.ProjectId == projectId && l.IsActive)
                .OrderBy(l => l.Order)
                .ToListAsync();
        }

        public async Task<TaskList> CreateAsync(TaskList taskList)
        {
            _context.TaskLists.Add(taskList);
            await _context.SaveChangesAsync();
            return taskList;
        }

        public async Task<TaskList> UpdateAsync(TaskList taskList)
        {
            taskList.UpdatedAt = DateTime.UtcNow;
            _context.TaskLists.Update(taskList);
            await _context.SaveChangesAsync();
            return taskList;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var taskList = await _context.TaskLists.FindAsync(id);
            if (taskList == null) return false;

            taskList.IsActive = false;
            await _context.SaveChangesAsync();
            return true;
        }
    }

    public class TaskRepository : ITaskRepository
    {
        private readonly ApplicationDbContext _context;

        public TaskRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Models.Task?> GetByIdAsync(int id)
        {
            return await _context.Tasks
                .Include(t => t.TaskList)
                .Include(t => t.Project)
                .Include(t => t.AssignedTo)
                .Include(t => t.Subtasks)
                .Include(t => t.Comments)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<IEnumerable<Models.Task>> GetListTasksAsync(int listId)
        {
            return await _context.Tasks
                .Where(t => t.ListId == listId)
                .Include(t => t.AssignedTo)
                .Include(t => t.Subtasks)
                .OrderBy(t => t.Order)
                .ToListAsync();
        }

        public async Task<IEnumerable<Models.Task>> GetProjectTasksAsync(int projectId)
        {
            return await _context.Tasks
                .Where(t => t.ProjectId == projectId)
                .Include(t => t.TaskList)
                .Include(t => t.AssignedTo)
                .Include(t => t.Subtasks)
                .OrderBy(t => t.Order)
                .ToListAsync();
        }

        public async Task<IEnumerable<Models.Task>> GetUserTasksAsync(string userId)
        {
            return await _context.Tasks
                .Where(t => t.AssignedToId == userId)
                .Include(t => t.Project)
                .Include(t => t.List)
                .Include(t => t.Subtasks)
                .OrderBy(t => t.DueDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Models.Task>> GetTasksByStatusAsync(int projectId, TaskStatus status)
        {
            return await _context.Tasks
                .Where(t => t.ProjectId == projectId && t.Status == status)
                .Include(t => t.AssignedTo)
                .Include(t => t.Subtasks)
                .ToListAsync();
        }

        public async Task<Models.Task> CreateAsync(Models.Task task)
        {
            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();
            return task;
        }

        public async Task<Models.Task> UpdateAsync(Models.Task task)
        {
            task.UpdatedAt = DateTime.UtcNow;
            _context.Tasks.Update(task);
            await _context.SaveChangesAsync();
            return task;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var task = await _context.Tasks.FindAsync(id);
            if (task == null) return false;

            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();
            return true;
        }
    }

    public class SubtaskRepository : ISubtaskRepository
    {
        private readonly ApplicationDbContext _context;

        public SubtaskRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Subtask?> GetByIdAsync(int id)
        {
            return await _context.Subtasks
                .Include(s => s.Task)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<IEnumerable<Subtask>> GetTaskSubtasksAsync(int taskId)
        {
            return await _context.Subtasks
                .Where(s => s.TaskId == taskId)
                .OrderBy(s => s.Order)
                .ToListAsync();
        }

        public async Task<Subtask> CreateAsync(Subtask subtask)
        {
            _context.Subtasks.Add(subtask);
            await _context.SaveChangesAsync();
            return subtask;
        }

        public async Task<Subtask> UpdateAsync(Subtask subtask)
        {
            _context.Subtasks.Update(subtask);
            await _context.SaveChangesAsync();
            return subtask;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var subtask = await _context.Subtasks.FindAsync(id);
            if (subtask == null) return false;

            _context.Subtasks.Remove(subtask);
            await _context.SaveChangesAsync();
            return true;
        }
    }

    public class CommentRepository : ICommentRepository
    {
        private readonly ApplicationDbContext _context;

        public CommentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Comment?> GetByIdAsync(int id)
        {
            return await _context.Comments
                .Include(c => c.Task)
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<IEnumerable<Comment>> GetTaskCommentsAsync(int taskId)
        {
            return await _context.Comments
                .Where(c => c.TaskId == taskId)
                .Include(c => c.User)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
        }

        public async Task<Comment> CreateAsync(Comment comment)
        {
            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();
            return comment;
        }

        public async Task<Comment> UpdateAsync(Comment comment)
        {
            comment.UpdatedAt = DateTime.UtcNow;
            comment.IsEdited = true;
            _context.Comments.Update(comment);
            await _context.SaveChangesAsync();
            return comment;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var comment = await _context.Comments.FindAsync(id);
            if (comment == null) return false;

            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();
            return true;
        }
    }

    public class AttachmentRepository : IAttachmentRepository
    {
        private readonly ApplicationDbContext _context;

        public AttachmentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Attachment?> GetByIdAsync(int id)
        {
            return await _context.Attachments
                .Include(a => a.Task)
                .Include(a => a.UploadedBy)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<IEnumerable<Attachment>> GetTaskAttachmentsAsync(int taskId)
        {
            return await _context.Attachments
                .Where(a => a.TaskId == taskId)
                .Include(a => a.UploadedBy)
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();
        }

        public async Task<Attachment> CreateAsync(Attachment attachment)
        {
            _context.Attachments.Add(attachment);
            await _context.SaveChangesAsync();
            return attachment;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var attachment = await _context.Attachments.FindAsync(id);
            if (attachment == null) return false;

            _context.Attachments.Remove(attachment);
            await _context.SaveChangesAsync();
            return true;
        }
    }

    public class ActivityLogRepository : IActivityLogRepository
    {
        private readonly ApplicationDbContext _context;

        public ActivityLogRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ActivityLog?> GetByIdAsync(int id)
        {
            return await _context.ActivityLogs
                .Include(a => a.User)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<IEnumerable<ActivityLog>> GetWorkspaceActivityAsync(int workspaceId, int count = 50)
        {
            return await _context.ActivityLogs
                .Where(a => a.WorkspaceId == workspaceId)
                .Include(a => a.User)
                .OrderByDescending(a => a.CreatedAt)
                .Take(count)
                .ToListAsync();
        }

        public async Task<IEnumerable<ActivityLog>> GetProjectActivityAsync(int projectId, int count = 50)
        {
            return await _context.ActivityLogs
                .Where(a => a.ProjectId == projectId)
                .Include(a => a.User)
                .OrderByDescending(a => a.CreatedAt)
                .Take(count)
                .ToListAsync();
        }

        public async Task<IEnumerable<ActivityLog>> GetTaskActivityAsync(int taskId, int count = 50)
        {
            return await _context.ActivityLogs
                .Where(a => a.TaskId == taskId)
                .Include(a => a.User)
                .OrderByDescending(a => a.CreatedAt)
                .Take(count)
                .ToListAsync();
        }

        public async Task<ActivityLog> CreateAsync(ActivityLog log)
        {
            _context.ActivityLogs.Add(log);
            await _context.SaveChangesAsync();
            return log;
        }
    }

    public class NotificationRepository : INotificationRepository
    {
        private readonly ApplicationDbContext _context;

        public NotificationRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Notification?> GetByIdAsync(int id)
        {
            return await _context.Notifications
                .Include(n => n.User)
                .FirstOrDefaultAsync(n => n.Id == id);
        }

        public async Task<IEnumerable<Notification>> GetUserNotificationsAsync(string userId)
        {
            return await _context.Notifications
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Notification>> GetUnreadNotificationsAsync(string userId)
        {
            return await _context.Notifications
                .Where(n => n.UserId == userId && !n.IsRead)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
        }

        public async Task<Notification> CreateAsync(Notification notification)
        {
            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();
            return notification;
        }

        public async Task<Notification> UpdateAsync(Notification notification)
        {
            _context.Notifications.Update(notification);
            await _context.SaveChangesAsync();
            return notification;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var notification = await _context.Notifications.FindAsync(id);
            if (notification == null) return false;

            _context.Notifications.Remove(notification);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
