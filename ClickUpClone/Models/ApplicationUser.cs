using Microsoft.AspNetCore.Identity;

namespace ClickUpClone.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? ProfilePicture { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? LastLoginAt { get; set; }
        public bool IsActive { get; set; } = true;

        // Navigation properties
        public ICollection<WorkspaceUser> WorkspaceUsers { get; set; } = new List<WorkspaceUser>();
        public ICollection<Task> AssignedTasks { get; set; } = new List<Task>();
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public ICollection<ActivityLog> ActivityLogs { get; set; } = new List<ActivityLog>();
        public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    }
}
