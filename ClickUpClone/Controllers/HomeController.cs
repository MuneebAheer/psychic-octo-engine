using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using ClickUpClone.Services;
using ClickUpClone.DTOs;
using ClickUpClone.Models;
using ClickUpClone.ViewModels.Dashboard;

namespace ClickUpClone.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly IWorkspaceService _workspaceService;
        private readonly ITaskService _taskService;
        private readonly INotificationService _notificationService;
        private readonly ILogger<HomeController> _logger;

        public HomeController(
            IWorkspaceService workspaceService,
            ITaskService taskService,
            INotificationService notificationService,
            ILogger<HomeController> logger)
        {
            _workspaceService = workspaceService;
            _taskService = taskService;
            _notificationService = notificationService;
            _logger = logger;
        }

        [AllowAnonymous]
        public IActionResult Index()
        {
            if (User.Identity?.IsAuthenticated == true)
                return RedirectToAction(nameof(Dashboard));

            return View();
        }

        [Authorize]
        public async Task<IActionResult> Dashboard()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;

            try
            {
                var workspaces = await _workspaceService.GetUserWorkspacesAsync(userId);
                var userTasks = await _taskService.GetUserTasksAsync(userId);
                var unreadNotifications = await _notificationService.GetUnreadNotificationsAsync(userId);

                // Group tasks by status
                var tasksByStatus = new Dictionary<string, int>();
                foreach (var status in Enum.GetNames(typeof(Models.TaskStatus)))
                {
                    tasksByStatus[status] = userTasks.Count(t => t.Status.ToString() == status);
                }

                // Group tasks by priority
                var tasksByPriority = new Dictionary<string, int>();
                foreach (var priority in Enum.GetNames(typeof(Models.TaskPriority)))
                {
                    tasksByPriority[priority] = userTasks.Count(t => t.Priority.ToString() == priority);
                }

                var vm = new DashboardViewModel
                {
                    WorkspaceCount = workspaces.Count(),
                    ProjectCount = workspaces.Sum(w => w.ProjectCount ?? 0),
                    TaskCount = userTasks.Count(),
                    OverdueTaskCount = userTasks.Count(t => t.DueDate.HasValue && t.DueDate < DateTime.Now && t.Status != Models.TaskStatus.Done),
                    UnreadNotificationCount = unreadNotifications.Count(),
                    CompletedTasksThisWeek = userTasks.Count(t => 
                        t.Status == Models.TaskStatus.Done && 
                        t.UpdatedAt.HasValue &&
                        t.UpdatedAt >= DateTime.Now.AddDays(-7)),
                    RecentWorkspaces = workspaces.Take(5).ToList(),
                    MyTasks = userTasks.Take(10).ToList(),
                    OverdueTasks = userTasks
                        .Where(t => t.DueDate.HasValue && t.DueDate < DateTime.Now && t.Status != Models.TaskStatus.Done)
                        .ToList(),
                    RecentNotifications = unreadNotifications.Take(5).ToList(),
                    TasksByStatus = tasksByStatus,
                    TasksByPriority = tasksByPriority
                };

                return View(vm);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error loading dashboard: {ex.Message}");
                TempData["ErrorMessage"] = "Error loading dashboard";
                return RedirectToAction(nameof(Index));
            }
        }

        [AllowAnonymous]
        public IActionResult Privacy()
        {
            return View();
        }

        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View();
        }
    }
}
