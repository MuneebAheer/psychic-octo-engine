using ClickUpClone.DTOs;
using ClickUpClone.Models;

namespace ClickUpClone.ViewModels.Workspaces
{
    public class WorkspaceDetailViewModel
    {
        public WorkspaceDto Workspace { get; set; }
        public IList<ProjectDto> Projects { get; set; } = new List<ProjectDto>();
        public IList<WorkspaceUserDto> Members { get; set; } = new List<WorkspaceUserDto>();

        public int ProjectCount => Projects.Count;
        public int MemberCount => Members.Count;
        public int TaskCount { get; set; }

        // Team member options for dropdowns
        public IList<ApplicationUserDto> AvailableUsers { get; set; } = new List<ApplicationUserDto>();
    }

    public class WorkspaceUserDto
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string UserEmail { get; set; }
        public string? UserProfilePicture { get; set; }
        public WorkspaceRole Role { get; set; }
        public DateTime JoinedAt { get; set; }
        public DateTime? InvitedAt { get; set; }
        public bool IsActive { get; set; }

        public string RoleDisplay => Role.ToString();
    }

    public class WorkspaceIndexViewModel
    {
        public IList<WorkspaceDto> OwnedWorkspaces { get; set; } = new List<WorkspaceDto>();
        public IList<WorkspaceDto> MemberWorkspaces { get; set; } = new List<WorkspaceDto>();
        
        public int TotalOwnedCount => OwnedWorkspaces.Count;
        public int TotalMemberCount => MemberWorkspaces.Count;
    }
}
