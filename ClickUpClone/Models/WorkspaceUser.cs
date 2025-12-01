namespace ClickUpClone.Models
{
    public enum WorkspaceRole
    {
        Owner = 0,
        Admin = 1,
        Member = 2,
        Guest = 3
    }

    public class WorkspaceUser
    {
        public int Id { get; set; }
        public int WorkspaceId { get; set; }
        public Workspace? Workspace { get; set; }
        public string UserId { get; set; } = string.Empty;
        public ApplicationUser? User { get; set; }
        public WorkspaceRole Role { get; set; } = WorkspaceRole.Member;
        public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
        public DateTime? InvitedAt { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
