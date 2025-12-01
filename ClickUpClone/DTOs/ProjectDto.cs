using ClickUpClone.Models;

namespace ClickUpClone.DTOs
{
    public class ProjectDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Color { get; set; }
        public int WorkspaceId { get; set; }
        public string CreatedByName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    public class CreateProjectDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Color { get; set; }
        public int WorkspaceId { get; set; }
    }

    public class UpdateProjectDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Color { get; set; }
    }

    public class TaskListDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Color { get; set; }
        public int ProjectId { get; set; }
        public int Order { get; set; }
        public bool IsActive { get; set; }
    }

    public class CreateTaskListDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Color { get; set; }
        public int ProjectId { get; set; }
    }

    public class UpdateTaskListDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Color { get; set; }
        public int Order { get; set; }
    }
}
