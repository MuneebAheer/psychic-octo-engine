using ClickUpClone.DTOs;

namespace ClickUpClone.ViewModels.Projects
{
    public class ProjectDetailViewModel
    {
        public ProjectDto Project { get; set; }
        public int WorkspaceId { get; set; }
        public string WorkspaceName { get; set; }

        public IList<TaskListDto> TaskLists { get; set; } = new List<TaskListDto>();
        
        // Statistics
        public int TotalTasks { get; set; }
        public int CompletedTasks { get; set; }
        public int OverdueTasks { get; set; }
        public decimal CompletionPercentage => TotalTasks > 0 ? (CompletedTasks * 100m) / TotalTasks : 0;

        // Team members
        public IList<ApplicationUserDto> TeamMembers { get; set; } = new List<ApplicationUserDto>();
    }

    public class ProjectListViewModel
    {
        public int WorkspaceId { get; set; }
        public string WorkspaceName { get; set; }

        public IList<ProjectDto> Projects { get; set; } = new List<ProjectDto>();
        
        // Summary statistics
        public int TotalProjects { get; set; }
        public int ActiveProjects { get; set; }
        public int CompletedProjects { get; set; }
    }
}
