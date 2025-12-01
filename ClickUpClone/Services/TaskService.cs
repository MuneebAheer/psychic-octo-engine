using ClickUpClone.Models;
using ClickUpClone.DTOs;
using ClickUpClone.Repositories;

namespace ClickUpClone.Services
{
    public class TaskService : ITaskService
    {
        private readonly ITaskRepository _taskRepository;
        private readonly IListRepository _listRepository;
        private readonly IActivityLogRepository _activityLogRepository;

        public TaskService(
            ITaskRepository taskRepository,
            IListRepository listRepository,
            IActivityLogRepository activityLogRepository)
        {
            _taskRepository = taskRepository;
            _listRepository = listRepository;
            _activityLogRepository = activityLogRepository;
        }

        public async Task<TaskDto?> GetTaskAsync(int id)
        {
            var task = await _taskRepository.GetByIdAsync(id);
            if (task == null) return null;

            return MapToDto(task);
        }

        public async Task<IEnumerable<TaskDto>> GetListTasksAsync(int listId)
        {
            var tasks = await _taskRepository.GetListTasksAsync(listId);
            return tasks.Select(MapToDto);
        }

        public async Task<IEnumerable<TaskDto>> GetProjectTasksAsync(int projectId)
        {
            var tasks = await _taskRepository.GetProjectTasksAsync(projectId);
            return tasks.Select(MapToDto);
        }

        public async Task<IEnumerable<TaskDto>> GetUserTasksAsync(string userId)
        {
            var tasks = await _taskRepository.GetUserTasksAsync(userId);
            return tasks.Select(MapToDto);
        }

        public async Task<IEnumerable<TaskDto>> GetFilteredTasksAsync(int projectId, TaskStatus? status = null, TaskPriority? priority = null, string? assignedTo = null)
        {
            var tasks = await _taskRepository.GetProjectTasksAsync(projectId);

            if (status.HasValue)
                tasks = tasks.Where(t => t.Status == status.Value);

            if (priority.HasValue)
                tasks = tasks.Where(t => t.Priority == priority.Value);

            if (!string.IsNullOrEmpty(assignedTo))
                tasks = tasks.Where(t => t.AssignedToId == assignedTo);

            return tasks.Select(MapToDto);
        }

        public async Task<TaskDto> CreateTaskAsync(CreateTaskDto dto, string userId)
        {
            var list = await _listRepository.GetByIdAsync(dto.ListId);
            if (list == null)
                throw new InvalidOperationException("List not found");

            var maxOrder = (await _taskRepository.GetListTasksAsync(dto.ListId))
                .Max(t => (int?)t.Order) ?? 0;

            var task = new Models.Task
            {
                Title = dto.Title,
                Description = dto.Description,
                ListId = dto.ListId,
                ProjectId = list.ProjectId,
                AssignedToId = dto.AssignedToId,
                Priority = dto.Priority,
                DueDate = dto.DueDate,
                Order = maxOrder + 1
            };

            var created = await _taskRepository.CreateAsync(task);

            await _activityLogRepository.CreateAsync(new ActivityLog
            {
                Type = ActivityType.Created,
                Description = $"Created task '{created.Title}'",
                UserId = userId,
                ProjectId = created.ProjectId,
                TaskId = created.Id
            });

            return MapToDto(created);
        }

        public async Task<TaskDto> UpdateTaskAsync(int id, UpdateTaskDto dto, string userId)
        {
            var task = await _taskRepository.GetByIdAsync(id);
            if (task == null)
                throw new InvalidOperationException("Task not found");

            var oldStatus = task.Status;
            var oldAssignedTo = task.AssignedToId;

            task.Title = dto.Title;
            task.Description = dto.Description;
            task.Status = dto.Status;
            task.Priority = dto.Priority;
            task.AssignedToId = dto.AssignedToId;
            task.DueDate = dto.DueDate;

            var updated = await _taskRepository.UpdateAsync(task);

            var changes = new List<string>();
            if (oldStatus != dto.Status)
                changes.Add($"Status changed from {oldStatus} to {dto.Status}");
            if (oldAssignedTo != dto.AssignedToId)
                changes.Add("Assignment changed");

            if (changes.Any())
            {
                await _activityLogRepository.CreateAsync(new ActivityLog
                {
                    Type = ActivityType.Updated,
                    Description = string.Join(", ", changes),
                    UserId = userId,
                    ProjectId = task.ProjectId,
                    TaskId = id
                });
            }

            return MapToDto(updated);
        }

        public async Task<bool> DeleteTaskAsync(int id, string userId)
        {
            var task = await _taskRepository.GetByIdAsync(id);
            if (task == null)
                throw new InvalidOperationException("Task not found");

            await _activityLogRepository.CreateAsync(new ActivityLog
            {
                Type = ActivityType.Deleted,
                Description = $"Deleted task",
                UserId = userId,
                ProjectId = task.ProjectId,
                TaskId = id
            });

            return await _taskRepository.DeleteAsync(id);
        }

        /// <summary>
        /// Update only task status (for AJAX calls)
        /// </summary>
        public async Task<TaskDto> UpdateTaskStatusAsync(int id, Models.TaskStatus status, string userId)
        {
            var task = await _taskRepository.GetByIdAsync(id);
            if (task == null)
                throw new InvalidOperationException("Task not found");

            if (task.Status == status)
                return MapToDto(task);

            var oldStatus = task.Status;
            task.Status = status;

            var updated = await _taskRepository.UpdateAsync(task);

            await _activityLogRepository.CreateAsync(new ActivityLog
            {
                Type = ActivityType.StatusChanged,
                Description = $"Status changed from {oldStatus} to {status}",
                UserId = userId,
                ProjectId = task.ProjectId,
                TaskId = id
            });

            return MapToDto(updated);
        }

        /// <summary>
        /// Update only task priority (for AJAX calls)
        /// </summary>
        public async Task<TaskDto> UpdateTaskPriorityAsync(int id, Models.TaskPriority priority, string userId)
        {
            var task = await _taskRepository.GetByIdAsync(id);
            if (task == null)
                throw new InvalidOperationException("Task not found");

            if (task.Priority == priority)
                return MapToDto(task);

            var oldPriority = task.Priority;
            task.Priority = priority;

            var updated = await _taskRepository.UpdateAsync(task);

            await _activityLogRepository.CreateAsync(new ActivityLog
            {
                Type = ActivityType.Updated,
                Description = $"Priority changed from {oldPriority} to {priority}",
                UserId = userId,
                ProjectId = task.ProjectId,
                TaskId = id
            });

            return MapToDto(updated);
        }

        /// <summary>
        /// Assign task to user (for AJAX calls)
        /// </summary>
        public async Task<TaskDto> AssignTaskAsync(int id, string? assignedToId, string userId)
        {
            var task = await _taskRepository.GetByIdAsync(id);
            if (task == null)
                throw new InvalidOperationException("Task not found");

            var oldAssignedTo = task.AssignedToId;
            task.AssignedToId = assignedToId;

            var updated = await _taskRepository.UpdateAsync(task);

            var description = assignedToId == null 
                ? "Unassigned task" 
                : $"Assigned task to user";

            await _activityLogRepository.CreateAsync(new ActivityLog
            {
                Type = ActivityType.Assigned,
                Description = description,
                UserId = userId,
                ProjectId = task.ProjectId,
                TaskId = id
            });

            return MapToDto(updated);
        }

        private TaskDto MapToDto(Models.Task task)
        {
            var completedSubtasks = task.Subtasks.Count(s => s.IsCompleted);
            return new TaskDto
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                ListId = task.ListId,
                ProjectId = task.ProjectId,
                AssignedToId = task.AssignedToId,
                AssignedToName = task.AssignedTo != null ? $"{task.AssignedTo.FirstName} {task.AssignedTo.LastName}" : null,
                Status = task.Status,
                Priority = task.Priority,
                DueDate = task.DueDate,
                Order = task.Order,
                SubtaskCount = task.Subtasks.Count,
                CompletedSubtaskCount = completedSubtasks,
                CreatedAt = task.CreatedAt
            };
        }
    }

    public class SubtaskService : ISubtaskService
    {
        private readonly ISubtaskRepository _subtaskRepository;
        private readonly ITaskRepository _taskRepository;
        private readonly IActivityLogRepository _activityLogRepository;

        public SubtaskService(
            ISubtaskRepository subtaskRepository,
            ITaskRepository taskRepository,
            IActivityLogRepository activityLogRepository)
        {
            _subtaskRepository = subtaskRepository;
            _taskRepository = taskRepository;
            _activityLogRepository = activityLogRepository;
        }

        public async Task<SubtaskDto?> GetSubtaskAsync(int id)
        {
            var subtask = await _subtaskRepository.GetByIdAsync(id);
            if (subtask == null) return null;

            return MapToDto(subtask);
        }

        public async Task<IEnumerable<SubtaskDto>> GetTaskSubtasksAsync(int taskId)
        {
            var subtasks = await _subtaskRepository.GetTaskSubtasksAsync(taskId);
            return subtasks.Select(MapToDto);
        }

        public async Task<SubtaskDto> CreateSubtaskAsync(CreateSubtaskDto dto, string userId)
        {
            var task = await _taskRepository.GetByIdAsync(dto.TaskId);
            if (task == null)
                throw new InvalidOperationException("Task not found");

            var maxOrder = (await _subtaskRepository.GetTaskSubtasksAsync(dto.TaskId))
                .Max(s => (int?)s.Order) ?? 0;

            var subtask = new Subtask
            {
                Title = dto.Title,
                TaskId = dto.TaskId,
                Order = maxOrder + 1
            };

            var created = await _subtaskRepository.CreateAsync(subtask);
            return MapToDto(created);
        }

        public async Task<SubtaskDto> UpdateSubtaskAsync(int id, bool isCompleted, string userId)
        {
            var subtask = await _subtaskRepository.GetByIdAsync(id);
            if (subtask == null)
                throw new InvalidOperationException("Subtask not found");

            subtask.IsCompleted = isCompleted;
            subtask.CompletedAt = isCompleted ? DateTime.UtcNow : null;

            var updated = await _subtaskRepository.UpdateAsync(subtask);
            return MapToDto(updated);
        }

        public async Task<bool> DeleteSubtaskAsync(int id, string userId)
        {
            return await _subtaskRepository.DeleteAsync(id);
        }

        private SubtaskDto MapToDto(Subtask subtask)
        {
            return new SubtaskDto
            {
                Id = subtask.Id,
                Title = subtask.Title,
                TaskId = subtask.TaskId,
                IsCompleted = subtask.IsCompleted,
                Order = subtask.Order
            };
        }
    }

    public class CommentService : ICommentService
    {
        private readonly ICommentRepository _commentRepository;
        private readonly ITaskRepository _taskRepository;
        private readonly IActivityLogRepository _activityLogRepository;

        public CommentService(
            ICommentRepository commentRepository,
            ITaskRepository taskRepository,
            IActivityLogRepository activityLogRepository)
        {
            _commentRepository = commentRepository;
            _taskRepository = taskRepository;
            _activityLogRepository = activityLogRepository;
        }

        public async Task<CommentDto?> GetCommentAsync(int id)
        {
            var comment = await _commentRepository.GetByIdAsync(id);
            if (comment == null) return null;

            return MapToDto(comment);
        }

        public async Task<IEnumerable<CommentDto>> GetTaskCommentsAsync(int taskId)
        {
            var comments = await _commentRepository.GetTaskCommentsAsync(taskId);
            return comments.Select(MapToDto);
        }

        public async Task<CommentDto> CreateCommentAsync(CreateCommentDto dto, string userId)
        {
            var task = await _taskRepository.GetByIdAsync(dto.TaskId);
            if (task == null)
                throw new InvalidOperationException("Task not found");

            var comment = new Comment
            {
                Content = dto.Content,
                TaskId = dto.TaskId,
                UserId = userId
            };

            var created = await _commentRepository.CreateAsync(comment);

            await _activityLogRepository.CreateAsync(new ActivityLog
            {
                Type = ActivityType.CommentAdded,
                Description = "Added a comment",
                UserId = userId,
                TaskId = dto.TaskId,
                ProjectId = task.ProjectId
            });

            return MapToDto(created);
        }

        public async Task<CommentDto> UpdateCommentAsync(int id, string content, string userId)
        {
            var comment = await _commentRepository.GetByIdAsync(id);
            if (comment == null)
                throw new InvalidOperationException("Comment not found");

            if (comment.UserId != userId)
                throw new UnauthorizedAccessException("Can only edit your own comments");

            comment.Content = content;
            var updated = await _commentRepository.UpdateAsync(comment);

            return MapToDto(updated);
        }

        public async Task<bool> DeleteCommentAsync(int id, string userId)
        {
            var comment = await _commentRepository.GetByIdAsync(id);
            if (comment == null)
                throw new InvalidOperationException("Comment not found");

            if (comment.UserId != userId)
                throw new UnauthorizedAccessException("Can only delete your own comments");

            return await _commentRepository.DeleteAsync(id);
        }

        private CommentDto MapToDto(Comment comment)
        {
            return new CommentDto
            {
                Id = comment.Id,
                Content = comment.Content,
                UserName = $"{comment.User?.FirstName} {comment.User?.LastName}",
                CreatedAt = comment.CreatedAt,
                IsEdited = comment.IsEdited
            };
        }
    }
}

