using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using ClickUpClone.Services;
using ClickUpClone.DTOs;
using ClickUpClone.Models;

namespace ClickUpClone.Controllers
{
    /// <summary>
    /// API controller for AJAX requests and real-time updates
    /// Handles task operations, comments, subtasks, and file uploads via API
    /// </summary>
    [ApiController]
    [Route("api")]
    [Authorize]
    public class ApiController : ControllerBase
    {
        private readonly ITaskService _taskService;
        private readonly ISubtaskService _subtaskService;
        private readonly ICommentService _commentService;
        private readonly IAttachmentService _attachmentService;
        private readonly ILogger<ApiController> _logger;

        public ApiController(
            ITaskService taskService,
            ISubtaskService subtaskService,
            ICommentService commentService,
            IAttachmentService attachmentService,
            ILogger<ApiController> logger)
        {
            _taskService = taskService;
            _subtaskService = subtaskService;
            _commentService = commentService;
            _attachmentService = attachmentService;
            _logger = logger;
        }

        private string GetCurrentUserId()
        {
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                ?? throw new UnauthorizedAccessException("User not found");
        }

        #region Task Operations

        /// <summary>
        /// Update task status via AJAX
        /// </summary>
        [HttpPost("tasks/{id}/status")]
        public async Task<IActionResult> UpdateTaskStatus(int id, [FromBody] UpdateTaskStatusRequest request)
        {
            try
            {
                if (!Enum.TryParse<TaskStatus>(request.Status, out var status))
                    return BadRequest(new { success = false, message = "Invalid status" });

                var task = await _taskService.GetTaskAsync(id);
                if (task == null)
                    return NotFound(new { success = false, message = "Task not found" });

                var updated = await _taskService.UpdateTaskStatusAsync(id, status, GetCurrentUserId());
                return Ok(new { success = true, data = updated });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating task status: {ex.Message}");
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Update task priority via AJAX
        /// </summary>
        [HttpPost("tasks/{id}/priority")]
        public async Task<IActionResult> UpdateTaskPriority(int id, [FromBody] UpdateTaskPriorityRequest request)
        {
            try
            {
                if (!Enum.TryParse<TaskPriority>(request.Priority, out var priority))
                    return BadRequest(new { success = false, message = "Invalid priority" });

                var updated = await _taskService.UpdateTaskPriorityAsync(id, priority, GetCurrentUserId());
                return Ok(new { success = true, data = updated });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating task priority: {ex.Message}");
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Assign task to user via AJAX
        /// </summary>
        [HttpPost("tasks/{id}/assign")]
        public async Task<IActionResult> AssignTask(int id, [FromBody] AssignTaskRequest request)
        {
            try
            {
                var updated = await _taskService.AssignTaskAsync(id, request.AssignedToId, GetCurrentUserId());
                return Ok(new { success = true, data = updated });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error assigning task: {ex.Message}");
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        #endregion

        #region Subtask Operations

        /// <summary>
        /// Toggle subtask completion via AJAX
        /// </summary>
        [HttpPost("subtasks/{id}/toggle")]
        public async Task<IActionResult> ToggleSubtask(int id)
        {
            try
            {
                var subtask = await _subtaskService.GetSubtaskAsync(id);
                if (subtask == null)
                    return NotFound(new { success = false, message = "Subtask not found" });

                var isCompleted = !subtask.IsCompleted;
                var updated = await _subtaskService.UpdateSubtaskAsync(
                    id, 
                    isCompleted, 
                    GetCurrentUserId()
                );
                return Ok(new { success = true, data = updated, isCompleted = isCompleted });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error toggling subtask: {ex.Message}");
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        #endregion

        #region Comment Operations

        /// <summary>
        /// Add comment via AJAX
        /// </summary>
        [HttpPost("tasks/{taskId}/comments")]
        public async Task<IActionResult> AddComment(int taskId, [FromBody] CreateCommentRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Content))
                    return BadRequest(new { success = false, message = "Comment cannot be empty" });

                var dto = new CreateCommentDto
                {
                    Content = request.Content,
                    TaskId = taskId
                };

                var comment = await _commentService.CreateCommentAsync(dto, GetCurrentUserId());
                return Ok(new { success = true, data = comment });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error adding comment: {ex.Message}");
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Update comment via AJAX
        /// </summary>
        [HttpPut("comments/{id}")]
        public async Task<IActionResult> UpdateComment(int id, [FromBody] UpdateCommentRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Content))
                    return BadRequest(new { success = false, message = "Comment cannot be empty" });

                var updated = await _commentService.UpdateCommentAsync(id, request.Content, GetCurrentUserId());
                return Ok(new { success = true, data = updated });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating comment: {ex.Message}");
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Delete comment via AJAX
        /// </summary>
        [HttpDelete("comments/{id}")]
        public async Task<IActionResult> DeleteComment(int id)
        {
            try
            {
                var result = await _commentService.DeleteCommentAsync(id, GetCurrentUserId());
                if (!result)
                    return NotFound(new { success = false, message = "Comment not found" });

                return Ok(new { success = true });
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error deleting comment: {ex.Message}");
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        #endregion

        #region File Operations

        /// <summary>
        /// Upload file attachment via AJAX/multipart
        /// </summary>
        [HttpPost("attachments")]
        public async Task<IActionResult> UploadAttachment([FromForm] int taskId, [FromForm] IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                    return BadRequest(new { success = false, message = "No file provided" });

                var attachment = await _attachmentService.UploadFileAsync(file, taskId, GetCurrentUserId());
                return Ok(new { success = true, data = attachment });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error uploading file: {ex.Message}");
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Delete attachment via AJAX
        /// </summary>
        [HttpDelete("attachments/{id}")]
        public async Task<IActionResult> DeleteAttachment(int id)
        {
            try
            {
                var result = await _attachmentService.DeleteFileAsync(id, GetCurrentUserId());
                if (!result)
                    return NotFound(new { success = false, message = "Attachment not found" });

                return Ok(new { success = true });
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error deleting attachment: {ex.Message}");
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        #endregion

        #region Request/Response Models

        public class UpdateTaskStatusRequest
        {
            public string Status { get; set; }
        }

        public class UpdateTaskPriorityRequest
        {
            public string Priority { get; set; }
        }

        public class AssignTaskRequest
        {
            public string? AssignedToId { get; set; }
        }

        public class CreateCommentRequest
        {
            public string Content { get; set; }
        }

        public class UpdateCommentRequest
        {
            public string Content { get; set; }
        }

        #endregion

        #region Advanced Filtering & Bulk Operations

        /// <summary>
        /// Get filtered tasks for advanced board filtering
        /// </summary>
        [HttpPost("tasks/filter")]
        public async Task<IActionResult> FilterTasks([FromBody] TaskFilterRequest request)
        {
            try
            {
                var userId = GetCurrentUserId();
                var filteredTasks = await _taskService.GetFilteredTasksAsync(
                    request.ProjectId,
                    request.Status,
                    request.Priority,
                    request.AssignedTo
                );

                // Additional client-side filtering
                var results = filteredTasks
                    .Where(t => string.IsNullOrEmpty(request.SearchTerm) || 
                                t.Title.Contains(request.SearchTerm, StringComparison.OrdinalIgnoreCase))
                    .Where(t => !request.DueDateFrom.HasValue || t.DueDate >= request.DueDateFrom)
                    .Where(t => !request.DueDateTo.HasValue || t.DueDate <= request.DueDateTo)
                    .ToList();

                return Ok(new { success = true, tasks = results, count = results.Count });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error filtering tasks: {ex.Message}");
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Bulk update task status
        /// </summary>
        [HttpPost("tasks/bulk/status")]
        public async Task<IActionResult> BulkUpdateStatus([FromBody] BulkActionRequest request)
        {
            try
            {
                var userId = GetCurrentUserId();
                var results = new List<dynamic>();

                foreach (var taskId in request.TaskIds)
                {
                    var result = await _taskService.UpdateTaskStatusAsync(taskId, request.NewStatus, userId);
                    results.Add(new { taskId = result.Id, status = result.Status });
                }

                return Ok(new { success = true, updated = results.Count, results });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in bulk update: {ex.Message}");
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Bulk update task priority
        /// </summary>
        [HttpPost("tasks/bulk/priority")]
        public async Task<IActionResult> BulkUpdatePriority([FromBody] BulkPriorityRequest request)
        {
            try
            {
                var userId = GetCurrentUserId();
                var results = new List<dynamic>();

                foreach (var taskId in request.TaskIds)
                {
                    var result = await _taskService.UpdateTaskPriorityAsync(taskId, request.NewPriority, userId);
                    results.Add(new { taskId = result.Id, priority = result.Priority });
                }

                return Ok(new { success = true, updated = results.Count, results });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in bulk update: {ex.Message}");
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Bulk assign tasks to user
        /// </summary>
        [HttpPost("tasks/bulk/assign")]
        public async Task<IActionResult> BulkAssignTasks([FromBody] BulkAssignRequest request)
        {
            try
            {
                var userId = GetCurrentUserId();
                var results = new List<dynamic>();

                foreach (var taskId in request.TaskIds)
                {
                    var result = await _taskService.AssignTaskAsync(taskId, request.AssignedToId, userId);
                    results.Add(new { taskId = result.Id, assignedTo = result.AssignedTo });
                }

                return Ok(new { success = true, updated = results.Count, results });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in bulk assign: {ex.Message}");
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Get quick statistics for tasks
        /// </summary>
        [HttpGet("tasks/stats/{projectId}")]
        public async Task<IActionResult> GetTaskStats(int projectId)
        {
            try
            {
                var allTasks = await _taskService.GetProjectTasksAsync(projectId);
                var today = DateTime.Today;

                var stats = new
                {
                    total = allTasks.Count(),
                    completed = allTasks.Count(t => t.Status == TaskStatus.Done),
                    inProgress = allTasks.Count(t => t.Status == TaskStatus.InProgress),
                    inReview = allTasks.Count(t => t.Status == TaskStatus.InReview),
                    overdue = allTasks.Count(t => t.DueDate < today && t.Status != TaskStatus.Done),
                    dueToday = allTasks.Count(t => t.DueDate == today && t.Status != TaskStatus.Done),
                    highPriority = allTasks.Count(t => t.Priority == TaskPriority.Urgent || t.Priority == TaskPriority.High)
                };

                return Ok(new { success = true, stats });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting task stats: {ex.Message}");
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        #endregion

        #region Request Models

        public class TaskFilterRequest
        {
            public int ProjectId { get; set; }
            public string SearchTerm { get; set; }
            public TaskStatus? Status { get; set; }
            public TaskPriority? Priority { get; set; }
            public string? AssignedTo { get; set; }
            public DateTime? DueDateFrom { get; set; }
            public DateTime? DueDateTo { get; set; }
        }

        public class BulkActionRequest
        {
            public List<int> TaskIds { get; set; }
            public TaskStatus NewStatus { get; set; }
        }

        public class BulkPriorityRequest
        {
            public List<int> TaskIds { get; set; }
            public TaskPriority NewPriority { get; set; }
        }

        public class BulkAssignRequest
        {
            public List<int> TaskIds { get; set; }
            public string? AssignedToId { get; set; }
        }

        #endregion
    }
}
