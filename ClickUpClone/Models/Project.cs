namespace ClickUpClone.Models
{
    public class Project
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Color { get; set; }
        public int WorkspaceId { get; set; }
        public Workspace? Workspace { get; set; }
        public string CreatedById { get; set; } = string.Empty;
        public ApplicationUser? CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public bool IsArchived { get; set; } = false;

        // Navigation properties
        public ICollection<TaskList> TaskLists { get; set; } = new List<TaskList>();
        public ICollection<Models.Task> Tasks { get; set; } = new List<Models.Task>();
        public ICollection<ActivityLog> ActivityLogs { get; set; } = new List<ActivityLog>();
    }
}
