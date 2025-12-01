using ClickUpClone.Models;

namespace ClickUpClone.DTOs
{
    public class WorkspaceDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Color { get; set; }
        public string OwnerId { get; set; } = string.Empty;
        public string OwnerName { get; set; } = string.Empty;
        public int MemberCount { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class WorkspaceUserDto
    {
        public int Id { get; set; }
        public int WorkspaceId { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string UserEmail { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public WorkspaceRole Role { get; set; }
        public DateTime JoinedAt { get; set; }
    }

    public class CreateWorkspaceDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Color { get; set; }
    }

    public class UpdateWorkspaceDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Color { get; set; }
    }

    public class InviteUserDto
    {
        public string Email { get; set; } = string.Empty;
        public WorkspaceRole Role { get; set; } = WorkspaceRole.Member;
    }
}
