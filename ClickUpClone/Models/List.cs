namespace ClickUpClone.Models
{
    public class List
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int ProjectId { get; set; }
        public Project? Project { get; set; }
        public int Order { get; set; } = 0;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public ICollection<Task> Tasks { get; set; } = new List<Task>();
    }
}
