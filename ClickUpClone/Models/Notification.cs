namespace ClickUpClone.Models
{
    public class Notification
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Message { get; set; }
        public string UserId { get; set; } = string.Empty;
        public ApplicationUser? User { get; set; }
        public int? TaskId { get; set; }
        public Task? Task { get; set; }
        public int? ProjectId { get; set; }
        public Project? Project { get; set; }
        public bool IsRead { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ReadAt { get; set; }
    }
}
