using ClickUpClone.DTOs;

namespace ClickUpClone.ViewModels.Dashboard
{
    public class DashboardViewModel
    {
        public int WorkspaceCount { get; set; }
        public int ProjectCount { get; set; }
        public int TaskCount { get; set; }
        public int OverdueTaskCount { get; set; }
        public int UnreadNotificationCount { get; set; }
        public int CompletedTasksThisWeek { get; set; }

        public IList<WorkspaceDto> RecentWorkspaces { get; set; } = new List<WorkspaceDto>();
        public IList<TaskDto> MyTasks { get; set; } = new List<TaskDto>();
        public IList<TaskDto> OverdueTasks { get; set; } = new List<TaskDto>();
        public IList<NotificationDto> RecentNotifications { get; set; } = new List<NotificationDto>();

        public Dictionary<string, int> TasksByStatus { get; set; } = new Dictionary<string, int>();
        public Dictionary<string, int> TasksByPriority { get; set; } = new Dictionary<string, int>();
    }
}
