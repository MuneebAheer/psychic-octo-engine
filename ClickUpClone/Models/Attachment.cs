namespace ClickUpClone.Models
{
    public class Attachment
    {
        public int Id { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public string FileType { get; set; } = string.Empty;
        public long FileSize { get; set; }
        public int TaskId { get; set; }
        public Task? Task { get; set; }
        public string UploadedById { get; set; } = string.Empty;
        public ApplicationUser? UploadedBy { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
