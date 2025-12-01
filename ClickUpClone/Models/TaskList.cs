namespace ClickUpClone.Models
{
    /// <summary>
    /// Renamed from 'List' to 'TaskList' to avoid conflict with C# List<T>
    /// Represents an organized container for tasks within a project
    /// </summary>
    public class TaskList
    {
        public int Id { get; set; }
        
        public string Name { get; set; } = string.Empty;
        
        public string? Description { get; set; }
        
        public string? Color { get; set; }
        
        public int ProjectId { get; set; }
        
        public Project? Project { get; set; }
        
        /// <summary>
        /// Order for custom sequencing without relying on creation order
        /// </summary>
        public int Order { get; set; } = 0;
        
        /// <summary>
        /// Soft delete flag - false means archived/deleted
        /// </summary>
        public bool IsActive { get; set; } = true;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public ICollection<Models.Task> Tasks { get; set; } = new List<Models.Task>();
    }
}
