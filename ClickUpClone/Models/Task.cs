namespace ClickUpClone.Models
{
    public enum TaskStatus
    {
        ToDo = 0,
        InProgress = 1,
        InReview = 2,
        Done = 3
    }

    public enum TaskPriority
    {
        Urgent = 0,
        High = 1,
        Normal = 2,
        Low = 3
    }

    public class Task
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int ListId { get; set; }
        public TaskList? TaskList { get; set; }
        public int ProjectId { get; set; }
        public Project? Project { get; set; }
        public string? AssignedToId { get; set; }
        public ApplicationUser? AssignedTo { get; set; }
        public TaskStatus Status { get; set; } = TaskStatus.ToDo;
        public TaskPriority Priority { get; set; } = TaskPriority.Normal;
        public DateTime? DueDate { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public int Order { get; set; } = 0;

        // Navigation properties
        public ICollection<Subtask> Subtasks { get; set; } = new List<Subtask>();
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public ICollection<Attachment> Attachments { get; set; } = new List<Attachment>();
        public ICollection<ActivityLog> ActivityLogs { get; set; } = new List<ActivityLog>();
    }
}
