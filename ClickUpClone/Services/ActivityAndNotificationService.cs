using ClickUpClone.Models;
using ClickUpClone.Repositories;

namespace ClickUpClone.Services
{
    public class ActivityLogService : IActivityLogService
    {
        private readonly IActivityLogRepository _activityLogRepository;

        public ActivityLogService(IActivityLogRepository activityLogRepository)
        {
            _activityLogRepository = activityLogRepository;
        }

        public async Task LogActivityAsync(ActivityLog activity)
        {
            await _activityLogRepository.CreateAsync(activity);
        }

        public async Task<IEnumerable<ActivityLog>> GetWorkspaceActivityAsync(int workspaceId)
        {
            return await _activityLogRepository.GetWorkspaceActivityAsync(workspaceId, 100);
        }

        public async Task<IEnumerable<ActivityLog>> GetProjectActivityAsync(int projectId)
        {
            return await _activityLogRepository.GetProjectActivityAsync(projectId, 100);
        }

        public async Task<IEnumerable<ActivityLog>> GetTaskActivityAsync(int taskId)
        {
            return await _activityLogRepository.GetTaskActivityAsync(taskId, 50);
        }
    }

    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _notificationRepository;

        public NotificationService(INotificationRepository notificationRepository)
        {
            _notificationRepository = notificationRepository;
        }

        public async Task<IEnumerable<Notification>> GetUserNotificationsAsync(string userId)
        {
            return await _notificationRepository.GetUserNotificationsAsync(userId);
        }

        public async Task<int> GetUnreadNotificationCountAsync(string userId)
        {
            var unread = await _notificationRepository.GetUnreadNotificationsAsync(userId);
            return unread.Count();
        }

        public async Task<Notification> CreateNotificationAsync(Notification notification)
        {
            return await _notificationRepository.CreateAsync(notification);
        }

        public async Task<bool> MarkAsReadAsync(int notificationId)
        {
            var notification = await _notificationRepository.GetByIdAsync(notificationId);
            if (notification == null) return false;

            notification.IsRead = true;
            notification.ReadAt = DateTime.UtcNow;
            await _notificationRepository.UpdateAsync(notification);
            return true;
        }

        public async Task<bool> MarkAllAsReadAsync(string userId)
        {
            var notifications = await _notificationRepository.GetUnreadNotificationsAsync(userId);
            foreach (var notification in notifications)
            {
                notification.IsRead = true;
                notification.ReadAt = DateTime.UtcNow;
                await _notificationRepository.UpdateAsync(notification);
            }
            return true;
        }

        public async Task<bool> DeleteNotificationAsync(int id)
        {
            return await _notificationRepository.DeleteAsync(id);
        }
    }
}
