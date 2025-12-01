using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using ClickUpClone.Services;
using ClickUpClone.DTOs;
using ClickUpClone.Models;
using ClickUpClone.ViewModels.Tasks;

namespace ClickUpClone.Controllers
{
    [Authorize]
    [Route("[controller]")]
    public class TasksController : Controller
    {
        private readonly ITaskService _taskService;
        private readonly ISubtaskService _subtaskService;
        private readonly ICommentService _commentService;
        private readonly ITaskListService _taskListService;
        private readonly IActivityLogService _activityLogService;
        private readonly INotificationService _notificationService;
        private readonly ILogger<TasksController> _logger;

        public TasksController(
            ITaskService taskService,
            ISubtaskService subtaskService,
            ICommentService commentService,
            ITaskListService taskListService,
            IActivityLogService activityLogService,
            INotificationService notificationService,
            ILogger<TasksController> logger)
        {
            _taskService = taskService;
            _subtaskService = subtaskService;
            _commentService = commentService;
            _taskListService = taskListService;
            _activityLogService = activityLogService;
            _notificationService = notificationService;
            _logger = logger;
        }

        private string GetUserId() => User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;

        [HttpGet]
        [Route("")]
        public async Task<IActionResult> Index(int listId)
        {
            try
            {
                var tasks = await _taskService.GetListTasksAsync(listId);
                ViewBag.ListId = listId;
                return View(tasks);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error loading tasks: {ex.Message}");
                return NotFound();
            }
        }

        [HttpGet]
        [Route("Board/{projectId}")]
        public async Task<IActionResult> Board(int projectId, string? searchTerm = null, int? status = null, int? priority = null, string? assignedTo = null)
        {
            try
            {
                var taskLists = await _taskListService.GetProjectTaskListsAsync(projectId);
                
                if (!taskLists.Any())
                    return NotFound();

                // Parse filter parameters
                TaskStatus? filterStatus = status.HasValue ? (TaskStatus)status.Value : null;
                TaskPriority? filterPriority = priority.HasValue ? (TaskPriority)priority.Value : null;

                var vm = new TaskBoardFilterViewModel
                {
                    Project = new ProjectDto { Id = projectId },
                    TaskLists = taskLists.ToList(),
                    SearchTerm = searchTerm ?? string.Empty,
                    FilterStatus = filterStatus,
                    FilterPriority = filterPriority,
                    FilterAssignedTo = assignedTo
                };

                // Group tasks by list for Kanban board with filtering
                foreach (var list in taskLists)
                {
                    var tasks = await _taskService.GetListTasksAsync(list.Id);
                    
                    // Apply filters
                    var filtered = tasks
                        .Where(t => string.IsNullOrEmpty(searchTerm) || 
                                    t.Title.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                        .Where(t => !filterStatus.HasValue || t.Status == filterStatus)
                        .Where(t => !filterPriority.HasValue || t.Priority == filterPriority)
                        .Where(t => string.IsNullOrEmpty(assignedTo) || t.AssignedTo == assignedTo)
                        .ToList();

                    vm.TasksByList[list.Id] = filtered;
                }

                // Calculate statistics
                var allTasks = vm.TasksByList.SelectMany(kvp => kvp.Value).ToList();
                var today = DateTime.Today;
                
                vm.TotalTasksCount = allTasks.Count;
                vm.CompletedTasksCount = allTasks.Count(t => t.Status == TaskStatus.Done);
                vm.InProgressTasksCount = allTasks.Count(t => t.Status == TaskStatus.InProgress);
                vm.OverdueTasksCount = allTasks.Count(t => t.DueDate < today && t.Status != TaskStatus.Done);

                return View(vm);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error loading board: {ex.Message}");
                return NotFound();
            }
        }

        [HttpGet]
        [Route("Create")]
        public IActionResult Create(int listId, int projectId)
        {
            ViewBag.ListId = listId;
            ViewBag.ProjectId = projectId;
            return View();
        }

        [HttpPost]
        [Route("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateTaskDto model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.ListId = model.ListId;
                return View(model);
            }

            try
            {
                var task = await _taskService.CreateTaskAsync(model, GetUserId());
                TempData["SuccessMessage"] = "Task created successfully";
                return RedirectToAction(nameof(Details), new { id = task.Id });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error creating task: {ex.Message}");
                ModelState.AddModelError(string.Empty, "Error creating task");
                ViewBag.ListId = model.ListId;
                return View(model);
            }
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var task = await _taskService.GetTaskAsync(id);
                if (task == null)
                    return NotFound();

                var subtasks = await _subtaskService.GetTaskSubtasksAsync(id);
                var comments = await _commentService.GetTaskCommentsAsync(id);

                ViewBag.Subtasks = subtasks;
                ViewBag.Comments = comments;

                return View(task);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error loading task: {ex.Message}");
                return NotFound();
            }
        }

        [HttpGet]
        [Route("{id}/Edit")]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var task = await _taskService.GetTaskAsync(id);
                if (task == null)
                    return NotFound();

                var updateDto = new UpdateTaskDto
                {
                    Title = task.Title,
                    Description = task.Description,
                    AssignedToId = task.AssignedToId,
                    Status = task.Status,
                    Priority = task.Priority,
                    DueDate = task.DueDate
                };

                return View(updateDto);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error loading task for edit: {ex.Message}");
                return NotFound();
            }
        }

        [HttpPost]
        [Route("{id}/Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UpdateTaskDto model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                await _taskService.UpdateTaskAsync(id, model, GetUserId());
                TempData["SuccessMessage"] = "Task updated successfully";
                return RedirectToAction(nameof(Details), new { id });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating task: {ex.Message}");
                ModelState.AddModelError(string.Empty, "Error updating task");
                return View(model);
            }
        }

        [HttpPost]
        [Route("{id}/Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var task = await _taskService.GetTaskAsync(id);
                if (task == null)
                    return NotFound();

                await _taskService.DeleteTaskAsync(id, GetUserId());
                TempData["SuccessMessage"] = "Task deleted successfully";
                return RedirectToAction(nameof(Index), new { listId = task.ListId });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error deleting task: {ex.Message}");
                TempData["ErrorMessage"] = "Error deleting task";
                return RedirectToAction(nameof(Details), new { id });
            }
        }

        [HttpPost]
        [Route("{id}/AddSubtask")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddSubtask(int id, CreateSubtaskDto model)
        {
            try
            {
                await _subtaskService.CreateSubtaskAsync(model, GetUserId());
                TempData["SuccessMessage"] = "Subtask created successfully";
                return RedirectToAction(nameof(Details), new { id });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error creating subtask: {ex.Message}");
                TempData["ErrorMessage"] = "Error creating subtask";
                return RedirectToAction(nameof(Details), new { id });
            }
        }

        [HttpPost]
        [Route("UpdateSubtask")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateSubtask(int subtaskId, bool isCompleted)
        {
            try
            {
                var subtask = await _subtaskService.UpdateSubtaskAsync(subtaskId, isCompleted, GetUserId());
                return Ok(new { success = true, data = subtask });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating subtask: {ex.Message}");
                return BadRequest(new { success = false, message = "Error updating subtask" });
            }
        }

        [HttpPost]
        [Route("{id}/AddComment")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddComment(int id, CreateCommentDto model)
        {
            try
            {
                await _commentService.CreateCommentAsync(model, GetUserId());
                TempData["SuccessMessage"] = "Comment added successfully";
                return RedirectToAction(nameof(Details), new { id });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error adding comment: {ex.Message}");
                TempData["ErrorMessage"] = "Error adding comment";
                return RedirectToAction(nameof(Details), new { id });
            }
        }

        [HttpGet]
        [Route("MyTasks")]
        public async Task<IActionResult> MyTasks()
        {
            try
            {
                var tasks = await _taskService.GetUserTasksAsync(GetUserId());
                return View(tasks);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error loading my tasks: {ex.Message}");
                return RedirectToAction("Dashboard", "Home");
            }
        }

        [HttpGet]
        [Route("Filtered")]
        public async Task<IActionResult> Filtered(int projectId, TaskStatus? status, TaskPriority? priority, string? assignedTo)
        {
            try
            {
                var tasks = await _taskService.GetFilteredTasksAsync(projectId, status, priority, assignedTo);
                ViewBag.ProjectId = projectId;
                ViewBag.FilterStatus = status;
                ViewBag.FilterPriority = priority;
                ViewBag.FilterAssignedTo = assignedTo;
                return View("Index", tasks);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error filtering tasks: {ex.Message}");
                return NotFound();
            }
        }
    }
}
