namespace ClickUpClone.Models
{
    public class Workspace
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Color { get; set; }
        public string OwnerId { get; set; } = string.Empty;
        public ApplicationUser? Owner { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;

        // Navigation properties
        public ICollection<WorkspaceUser> WorkspaceUsers { get; set; } = new List<WorkspaceUser>();
        public ICollection<Project> Projects { get; set; } = new List<Project>();
        public ICollection<ActivityLog> ActivityLogs { get; set; } = new List<ActivityLog>();
    }
}
