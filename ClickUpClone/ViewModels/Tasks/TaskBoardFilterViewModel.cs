using ClickUpClone.DTOs;
using ClickUpClone.Models;

namespace ClickUpClone.ViewModels.Tasks
{
    public class TaskBoardFilterViewModel
    {
        /// <summary>
        /// The project this board belongs to
        /// </summary>
        public ProjectDto Project { get; set; }

        /// <summary>
        /// All task lists in the project
        /// </summary>
        public IList<TaskListDto> TaskLists { get; set; } = new List<TaskListDto>();

        /// <summary>
        /// Tasks grouped by their list
        /// </summary>
        public Dictionary<int, IList<TaskDto>> TasksByList { get; set; } = new Dictionary<int, IList<TaskDto>>();

        /// <summary>
        /// All team members for filtering and assignment
        /// </summary>
        public IList<ApplicationUserDto> TeamMembers { get; set; } = new List<ApplicationUserDto>();

        /// <summary>
        /// Available priority levels for filtering
        /// </summary>
        public IList<TaskPriority> PriorityOptions { get; set; } = new List<TaskPriority>
        {
            TaskPriority.Low,
            TaskPriority.Normal,
            TaskPriority.High,
            TaskPriority.Urgent
        };

        /// <summary>
        /// Available status values for filtering
        /// </summary>
        public IList<TaskStatus> StatusOptions { get; set; } = new List<TaskStatus>
        {
            TaskStatus.ToDo,
            TaskStatus.InProgress,
            TaskStatus.InReview,
            TaskStatus.Done
        };

        /// <summary>
        /// Current filter: Search term
        /// </summary>
        public string SearchTerm { get; set; } = string.Empty;

        /// <summary>
        /// Current filter: Status
        /// </summary>
        public TaskStatus? FilterStatus { get; set; }

        /// <summary>
        /// Current filter: Priority
        /// </summary>
        public TaskPriority? FilterPriority { get; set; }

        /// <summary>
        /// Current filter: Assigned user ID
        /// </summary>
        public string? FilterAssignedTo { get; set; }

        /// <summary>
        /// Current filter: Due date from
        /// </summary>
        public DateTime? FilterDueDateFrom { get; set; }

        /// <summary>
        /// Current filter: Due date to
        /// </summary>
        public DateTime? FilterDueDateTo { get; set; }

        /// <summary>
        /// Bulk actions: Selected task IDs
        /// </summary>
        public IList<int> SelectedTaskIds { get; set; } = new List<int>();

        /// <summary>
        /// Total tasks matching current filters
        /// </summary>
        public int TotalTasksCount { get; set; }

        /// <summary>
        /// Tasks completed count
        /// </summary>
        public int CompletedTasksCount { get; set; }

        /// <summary>
        /// Tasks in progress count
        /// </summary>
        public int InProgressTasksCount { get; set; }

        /// <summary>
        /// Overdue tasks count
        /// </summary>
        public int OverdueTasksCount { get; set; }
    }
}
