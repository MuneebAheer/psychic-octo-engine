using ClickUpClone.Models;
using ClickUpClone.DTOs;
using ClickUpClone.Repositories;

namespace ClickUpClone.Services
{
    /// <summary>
    /// Handles file attachment operations including upload, download, and deletion
    /// </summary>
    public class AttachmentService : IAttachmentService
    {
        private readonly IAttachmentRepository _attachmentRepository;
        private readonly ITaskRepository _taskRepository;
        private readonly IActivityLogRepository _activityLogRepository;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly ILogger<AttachmentService> _logger;

        // Allowed file extensions
        private readonly string[] _allowedExtensions = { ".pdf", ".doc", ".docx", ".txt", ".jpg", ".jpeg", ".png", ".gif", ".zip", ".xlsx", ".xls" };
        private const long MaxFileSizeBytes = 10 * 1024 * 1024; // 10MB

        public AttachmentService(
            IAttachmentRepository attachmentRepository,
            ITaskRepository taskRepository,
            IActivityLogRepository activityLogRepository,
            IWebHostEnvironment hostingEnvironment,
            ILogger<AttachmentService> logger)
        {
            _attachmentRepository = attachmentRepository;
            _taskRepository = taskRepository;
            _activityLogRepository = activityLogRepository;
            _hostingEnvironment = hostingEnvironment;
            _logger = logger;
        }

        /// <summary>
        /// Upload a file attachment to a task
        /// </summary>
        public async Task<AttachmentDto> UploadFileAsync(IFormFile file, int taskId, string userId)
        {
            // Validate file
            if (file == null || file.Length == 0)
                throw new ArgumentException("File is empty");

            if (file.Length > MaxFileSizeBytes)
                throw new ArgumentException($"File size exceeds {MaxFileSizeBytes / (1024 * 1024)}MB limit");

            var fileExtension = Path.GetExtension(file.FileName).ToLower();
            if (!_allowedExtensions.Contains(fileExtension))
                throw new ArgumentException($"File type {fileExtension} is not allowed");

            // Verify task exists
            var task = await _taskRepository.GetByIdAsync(taskId);
            if (task == null)
                throw new InvalidOperationException("Task not found");

            try
            {
                // Create upload folder if it doesn't exist
                var uploadFolder = Path.Combine(_hostingEnvironment.WebRootPath, "uploads", "attachments");
                Directory.CreateDirectory(uploadFolder);

                // Generate unique filename to prevent collisions
                var uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
                var filePath = Path.Combine("uploads", "attachments", uniqueFileName);
                var fullPath = Path.Combine(uploadFolder, uniqueFileName);

                // Save file to disk
                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                _logger.LogInformation($"File uploaded: {uniqueFileName} by user {userId}");

                // Create attachment record in database
                var attachment = new Attachment
                {
                    FileName = file.FileName,
                    FilePath = filePath,
                    FileType = fileExtension,
                    FileSize = file.Length,
                    TaskId = taskId,
                    UploadedById = userId,
                    CreatedAt = DateTime.UtcNow
                };

                var created = await _attachmentRepository.CreateAsync(attachment);

                // Log activity
                await _activityLogRepository.CreateAsync(new ActivityLog
                {
                    Type = ActivityType.AttachmentAdded,
                    Description = $"Uploaded file: {file.FileName}",
                    UserId = userId,
                    TaskId = taskId
                });

                return MapToDto(created);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error uploading file: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Delete a file attachment
        /// </summary>
        public async Task<bool> DeleteFileAsync(int id, string userId)
        {
            var attachment = await _attachmentRepository.GetByIdAsync(id);
            if (attachment == null)
                return false;

            // Verify user is the uploader
            if (attachment.UploadedById != userId)
                throw new UnauthorizedAccessException("Cannot delete attachment uploaded by another user");

            try
            {
                // Delete file from disk
                var fullPath = Path.Combine(_hostingEnvironment.WebRootPath, attachment.FilePath);
                if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                    _logger.LogInformation($"File deleted: {attachment.FileName}");
                }

                // Delete from database
                var result = await _attachmentRepository.DeleteAsync(id);

                if (result)
                {
                    // Log activity
                    await _activityLogRepository.CreateAsync(new ActivityLog
                    {
                        Type = ActivityType.Updated,
                        Description = $"Deleted attachment: {attachment.FileName}",
                        UserId = userId,
                        TaskId = attachment.TaskId
                    });
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error deleting file: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Get all attachments for a task
        /// </summary>
        public async Task<IEnumerable<AttachmentDto>> GetTaskAttachmentsAsync(int taskId)
        {
            var attachments = await _attachmentRepository.GetTaskAttachmentsAsync(taskId);
            return attachments.Select(MapToDto);
        }

        /// <summary>
        /// Map Attachment entity to DTO
        /// </summary>
        private AttachmentDto MapToDto(Attachment attachment)
        {
            return new AttachmentDto
            {
                Id = attachment.Id,
                FileName = attachment.FileName,
                FilePath = attachment.FilePath,
                FileType = attachment.FileType,
                FileSize = attachment.FileSize,
                TaskId = attachment.TaskId,
                UploadedByName = $"{attachment.UploadedBy?.FirstName} {attachment.UploadedBy?.LastName}",
                CreatedAt = attachment.CreatedAt
            };
        }
    }
}
