namespace ClickUpClone.Models
{
    public class Subtask
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public int TaskId { get; set; }
        public Task? Task { get; set; }
        public bool IsCompleted { get; set; } = false;
        public DateTime? CompletedAt { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public int Order { get; set; } = 0;
    }
}
