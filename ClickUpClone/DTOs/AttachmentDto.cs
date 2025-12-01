namespace ClickUpClone.DTOs
{
    public class AttachmentDto
    {
        public int Id { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public string FileType { get; set; } = string.Empty;
        public long FileSize { get; set; }
        public int TaskId { get; set; }
        public string UploadedByName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    public class CreateAttachmentDto
    {
        public int TaskId { get; set; }
        public IFormFile File { get; set; }
    }
}
