using ClickUpClone.DTOs;

namespace ClickUpClone.ViewModels.Tasks
{
    public class TaskIndexViewModel
    {
        public int ProjectId { get; set; }
        public int TaskListId { get; set; }
        public string ProjectName { get; set; }
        public string TaskListName { get; set; }

        public IList<TaskDto> Tasks { get; set; } = new List<TaskDto>();

        // Pagination
        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalCount { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);

        // Filtering
        public string? FilterStatus { get; set; }
        public string? FilterPriority { get; set; }
        public string? FilterAssignedTo { get; set; }
        public string? SearchTerm { get; set; }
    }

    public class MyTasksViewModel
    {
        public IList<TaskDto> AssignedToMe { get; set; } = new List<TaskDto>();
        public IList<TaskDto> CreatedByMe { get; set; } = new List<TaskDto>();
        public IList<TaskDto> CompletedByMe { get; set; } = new List<TaskDto>();

        // Statistics
        public int TotalAssigned { get; set; }
        public int TotalCreated { get; set; }
        public int TotalCompleted { get; set; }
        public int OverdueCount { get; set; }

        // Grouping
        public Dictionary<string, IList<TaskDto>> TasksByStatus { get; set; } 
            = new Dictionary<string, IList<TaskDto>>();
        public Dictionary<string, IList<TaskDto>> TasksByPriority { get; set; } 
            = new Dictionary<string, IList<TaskDto>>();
    }
}
