namespace ClickUpClone.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public int TaskId { get; set; }
        public Task? Task { get; set; }
        public string UserId { get; set; } = string.Empty;
        public ApplicationUser? User { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public bool IsEdited { get; set; } = false;
    }
}
