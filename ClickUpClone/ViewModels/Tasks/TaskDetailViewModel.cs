using ClickUpClone.DTOs;
using ClickUpClone.Models;

namespace ClickUpClone.ViewModels.Tasks
{
    public class TaskDetailViewModel
    {
        public TaskDto Task { get; set; }
        public IList<SubtaskDto> Subtasks { get; set; } = new List<SubtaskDto>();
        public IList<CommentDto> Comments { get; set; } = new List<CommentDto>();
        public IList<AttachmentDto> Attachments { get; set; } = new List<AttachmentDto>();
        public IList<ActivityLogDto> ActivityLogs { get; set; } = new List<ActivityLogDto>();

        // For dropdowns
        public IList<ApplicationUserDto> AssigneeOptions { get; set; } = new List<ApplicationUserDto>();
        
        // Status and Priority options - populated from enums
        public IList<string> StatusOptions { get; set; } = new List<string>();
        public IList<string> PriorityOptions { get; set; } = new List<string>();

        public TaskDetailViewModel()
        {
            // Populate enum values
            StatusOptions = Enum.GetNames(typeof(TaskStatus)).ToList();
            PriorityOptions = Enum.GetNames(typeof(TaskPriority)).ToList();
        }
    }

    /// <summary>
    /// DTO for Comments in ViewModels
    /// </summary>
    public class CommentDto
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public string UserName { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsEdited => UpdatedAt.HasValue && UpdatedAt > CreatedAt;
    }

    /// <summary>
    /// DTO for Subtasks in ViewModels
    /// </summary>
    public class SubtaskDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    /// <summary>
    /// DTO for Activity Logs in ViewModels
    /// </summary>
    public class ActivityLogDto
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Activity { get; set; }
        public string ActivityType { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
