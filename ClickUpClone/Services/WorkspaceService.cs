using ClickUpClone.Models;
using ClickUpClone.DTOs;
using ClickUpClone.Repositories;
using Microsoft.AspNetCore.Identity;

namespace ClickUpClone.Services
{
    public class WorkspaceService : IWorkspaceService
    {
        private readonly IWorkspaceRepository _workspaceRepository;
        private readonly IWorkspaceUserRepository _workspaceUserRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IActivityLogRepository _activityLogRepository;

        public WorkspaceService(
            IWorkspaceRepository workspaceRepository,
            IWorkspaceUserRepository workspaceUserRepository,
            UserManager<ApplicationUser> userManager,
            IActivityLogRepository activityLogRepository)
        {
            _workspaceRepository = workspaceRepository;
            _workspaceUserRepository = workspaceUserRepository;
            _userManager = userManager;
            _activityLogRepository = activityLogRepository;
        }

        public async Task<WorkspaceDto?> GetWorkspaceAsync(int id)
        {
            var workspace = await _workspaceRepository.GetByIdAsync(id);
            if (workspace == null) return null;

            return MapToDto(workspace);
        }

        public async Task<IEnumerable<WorkspaceDto>> GetUserWorkspacesAsync(string userId)
        {
            var workspaces = await _workspaceRepository.GetUserWorkspacesAsync(userId);
            return workspaces.Select(MapToDto);
        }

        public async Task<WorkspaceDto> CreateWorkspaceAsync(CreateWorkspaceDto dto, string ownerId)
        {
            var workspace = new Workspace
            {
                Name = dto.Name,
                Description = dto.Description,
                Color = dto.Color,
                OwnerId = ownerId
            };

            var created = await _workspaceRepository.CreateAsync(workspace);

            // Add owner as Admin
            await _workspaceUserRepository.AddUserAsync(new WorkspaceUser
            {
                WorkspaceId = created.Id,
                UserId = ownerId,
                Role = WorkspaceRole.Owner
            });

            // Log activity
            await _activityLogRepository.CreateAsync(new ActivityLog
            {
                Type = ActivityType.Created,
                Description = $"Created workspace '{created.Name}'",
                UserId = ownerId,
                WorkspaceId = created.Id
            });

            return MapToDto(created);
        }

        public async Task<WorkspaceDto> UpdateWorkspaceAsync(int id, UpdateWorkspaceDto dto, string userId)
        {
            var workspace = await _workspaceRepository.GetByIdAsync(id);
            if (workspace == null)
                throw new InvalidOperationException("Workspace not found");

            if (workspace.OwnerId != userId)
                throw new UnauthorizedAccessException("Only owner can update workspace");

            workspace.Name = dto.Name;
            workspace.Description = dto.Description;
            workspace.Color = dto.Color;

            var updated = await _workspaceRepository.UpdateAsync(workspace);

            await _activityLogRepository.CreateAsync(new ActivityLog
            {
                Type = ActivityType.Updated,
                Description = $"Updated workspace",
                UserId = userId,
                WorkspaceId = id
            });

            return MapToDto(updated);
        }

        public async Task<bool> DeleteWorkspaceAsync(int id, string userId)
        {
            var workspace = await _workspaceRepository.GetByIdAsync(id);
            if (workspace == null)
                throw new InvalidOperationException("Workspace not found");

            if (workspace.OwnerId != userId)
                throw new UnauthorizedAccessException("Only owner can delete workspace");

            await _activityLogRepository.CreateAsync(new ActivityLog
            {
                Type = ActivityType.Deleted,
                Description = $"Deleted workspace",
                UserId = userId,
                WorkspaceId = id
            });

            return await _workspaceRepository.DeleteAsync(id);
        }

        public async Task<IEnumerable<WorkspaceUserDto>> GetWorkspaceUsersAsync(int workspaceId, string userId)
        {
            var workspace = await _workspaceRepository.GetByIdAsync(workspaceId);
            if (workspace == null)
                throw new InvalidOperationException("Workspace not found");

            var users = await _workspaceUserRepository.GetWorkspaceUsersAsync(workspaceId);
            return users.Select(wu => new WorkspaceUserDto
            {
                Id = wu.Id,
                WorkspaceId = wu.WorkspaceId,
                UserId = wu.UserId,
                UserEmail = wu.User?.Email ?? string.Empty,
                UserName = $"{wu.User?.FirstName} {wu.User?.LastName}",
                Role = wu.Role,
                JoinedAt = wu.JoinedAt
            });
        }

        public async Task<(bool Success, string Message)> InviteUserAsync(int workspaceId, InviteUserDto dto, string userId)
        {
            var workspace = await _workspaceRepository.GetByIdAsync(workspaceId);
            if (workspace == null)
                return (false, "Workspace not found");

            var requester = await _workspaceUserRepository.GetByWorkspaceAndUserAsync(workspaceId, userId);
            if (requester?.Role == WorkspaceRole.Member)
                return (false, "Only owners and admins can invite users");

            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
                return (false, "User not found");

            var existing = await _workspaceUserRepository.GetByWorkspaceAndUserAsync(workspaceId, user.Id);
            if (existing != null)
                return (false, "User is already a member");

            var workspaceUser = new WorkspaceUser
            {
                WorkspaceId = workspaceId,
                UserId = user.Id,
                Role = dto.Role,
                InvitedAt = DateTime.UtcNow
            };

            await _workspaceUserRepository.AddUserAsync(workspaceUser);

            // Create notification
            // In real app, send email

            return (true, "User invited successfully");
        }

        public async Task<(bool Success, string Message)> RemoveUserAsync(int workspaceId, int workspaceUserId, string userId)
        {
            var workspace = await _workspaceRepository.GetByIdAsync(workspaceId);
            if (workspace == null)
                return (false, "Workspace not found");

            if (workspace.OwnerId != userId)
                return (false, "Only owner can remove users");

            return await _workspaceUserRepository.RemoveUserAsync(workspaceUserId)
                ? (true, "User removed successfully")
                : (false, "User not found");
        }

        public async Task<(bool Success, string Message)> UpdateUserRoleAsync(int workspaceUserId, WorkspaceRole role, string userId)
        {
            var workspaceUser = await _workspaceUserRepository.GetByIdAsync(workspaceUserId);
            if (workspaceUser == null)
                return (false, "User not found");

            var requester = await _workspaceUserRepository.GetByWorkspaceAndUserAsync(workspaceUser.WorkspaceId, userId);
            if (requester?.Role != WorkspaceRole.Owner)
                return (false, "Only owner can change roles");

            workspaceUser.Role = role;
            await _workspaceUserRepository.UpdateAsync(workspaceUser);

            return (true, "Role updated successfully");
        }

        private WorkspaceDto MapToDto(Workspace workspace)
        {
            return new WorkspaceDto
            {
                Id = workspace.Id,
                Name = workspace.Name,
                Description = workspace.Description,
                Color = workspace.Color,
                OwnerId = workspace.OwnerId,
                OwnerName = $"{workspace.Owner?.FirstName} {workspace.Owner?.LastName}",
                MemberCount = workspace.WorkspaceUsers.Count,
                CreatedAt = workspace.CreatedAt
            };
        }
    }
}
