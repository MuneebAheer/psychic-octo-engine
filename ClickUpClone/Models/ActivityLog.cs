namespace ClickUpClone.Models
{
    public enum ActivityType
    {
        Created = 0,
        Updated = 1,
        Deleted = 2,
        Assigned = 3,
        Unassigned = 4,
        StatusChanged = 5,
        CommentAdded = 6,
        AttachmentAdded = 7
    }

    public class ActivityLog
    {
        public int Id { get; set; }
        public ActivityType Type { get; set; }
        public string? Description { get; set; }
        public string UserId { get; set; } = string.Empty;
        public ApplicationUser? User { get; set; }
        public int? WorkspaceId { get; set; }
        public Workspace? Workspace { get; set; }
        public int? ProjectId { get; set; }
        public Project? Project { get; set; }
        public int? TaskId { get; set; }
        public Task? Task { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
