using ClickUpClone.Models;

namespace ClickUpClone.DTOs
{
    public class TaskDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int ListId { get; set; }
        public int ProjectId { get; set; }
        public string? AssignedToId { get; set; }
        public string? AssignedToName { get; set; }
        public Models.TaskStatus Status { get; set; }
        public Models.TaskPriority Priority { get; set; }
        public DateTime? DueDate { get; set; }
        public int Order { get; set; }
        public int SubtaskCount { get; set; }
        public int CompletedSubtaskCount { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CreateTaskDto
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int ListId { get; set; }
        public string? AssignedToId { get; set; }
        public Models.TaskPriority Priority { get; set; } = Models.TaskPriority.Normal;
        public DateTime? DueDate { get; set; }
    }

    public class UpdateTaskDto
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? AssignedToId { get; set; }
        public Models.TaskStatus Status { get; set; }
        public Models.TaskPriority Priority { get; set; }
        public DateTime? DueDate { get; set; }
    }

    public class SubtaskDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public int TaskId { get; set; }
        public bool IsCompleted { get; set; }
        public int Order { get; set; }
    }

    public class CreateSubtaskDto
    {
        public string Title { get; set; } = string.Empty;
        public int TaskId { get; set; }
    }

    public class CommentDto
    {
        public int Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public bool IsEdited { get; set; }
    }

    public class CreateCommentDto
    {
        public string Content { get; set; } = string.Empty;
        public int TaskId { get; set; }
    }
}
