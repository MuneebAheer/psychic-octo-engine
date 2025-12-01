using ClickUpClone.DTOs;

namespace ClickUpClone.ViewModels.Tasks
{
    public class TaskBoardViewModel
    {
        public ProjectDto Project { get; set; }
        public IList<TaskListDto> TaskLists { get; set; } = new List<TaskListDto>();

        // Grouped tasks by list for Kanban board
        public Dictionary<int, IList<TaskDto>> TasksByList { get; set; } 
            = new Dictionary<int, IList<TaskDto>>();

        // Team members for assignment dropdown
        public IList<ApplicationUserDto> TeamMembers { get; set; } = new List<ApplicationUserDto>();

        // Filter options
        public string? FilterStatus { get; set; }
        public string? FilterPriority { get; set; }
        public string? FilterAssignedTo { get; set; }
    }

    /// <summary>
    /// DTO for ApplicationUser in ViewModels
    /// </summary>
    public class ApplicationUserDto
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string? ProfilePicture { get; set; }

        public string FullName => $"{FirstName} {LastName}";
    }
}
