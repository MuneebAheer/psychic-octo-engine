using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using ClickUpClone.Services;

namespace ClickUpClone.Controllers
{
    [Authorize]
    [Route("[controller]")]
    public class ActivityLogsController : Controller
    {
        private readonly IActivityLogService _activityLogService;
        private readonly IWorkspaceService _workspaceService;
        private readonly IProjectService _projectService;
        private readonly ILogger<ActivityLogsController> _logger;

        public ActivityLogsController(
            IActivityLogService activityLogService,
            IWorkspaceService workspaceService,
            IProjectService projectService,
            ILogger<ActivityLogsController> logger)
        {
            _activityLogService = activityLogService;
            _workspaceService = workspaceService;
            _projectService = projectService;
            _logger = logger;
        }

        private string GetUserId() => User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;

        [HttpGet]
        [Route("Workspace/{workspaceId}")]
        public async Task<IActionResult> Workspace(int workspaceId)
        {
            try
            {
                var workspace = await _workspaceService.GetWorkspaceAsync(workspaceId);
                if (workspace == null)
                    return NotFound();

                var activities = await _activityLogService.GetWorkspaceActivityAsync(workspaceId);
                ViewBag.WorkspaceName = workspace.Name;
                ViewBag.WorkspaceId = workspaceId;

                return View("Index", activities);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error loading workspace activity: {ex.Message}");
                return NotFound();
            }
        }

        [HttpGet]
        [Route("Project/{projectId}")]
        public async Task<IActionResult> Project(int projectId)
        {
            try
            {
                var project = await _projectService.GetProjectAsync(projectId);
                if (project == null)
                    return NotFound();

                var activities = await _activityLogService.GetProjectActivityAsync(projectId);
                ViewBag.ProjectName = project.Name;
                ViewBag.ProjectId = projectId;

                return View("Index", activities);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error loading project activity: {ex.Message}");
                return NotFound();
            }
        }

        [HttpGet]
        [Route("Task/{taskId}")]
        public async Task<IActionResult> Task(int taskId)
        {
            try
            {
                var activities = await _activityLogService.GetTaskActivityAsync(taskId);
                ViewBag.TaskId = taskId;

                return View("Index", activities);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error loading task activity: {ex.Message}");
                return NotFound();
            }
        }
    }
}
