using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using ClickUpClone.Services;
using ClickUpClone.DTOs;
using ClickUpClone.Models;

namespace ClickUpClone.Controllers
{
    [Authorize]
    [Route("[controller]")]
    public class WorkspacesController : Controller
    {
        private readonly IWorkspaceService _workspaceService;
        private readonly IProjectService _projectService;
        private readonly ILogger<WorkspacesController> _logger;

        public WorkspacesController(
            IWorkspaceService workspaceService,
            IProjectService projectService,
            ILogger<WorkspacesController> logger)
        {
            _workspaceService = workspaceService;
            _projectService = projectService;
            _logger = logger;
        }

        private string GetUserId() => User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;

        [HttpGet]
        [Route("")]
        public async Task<IActionResult> Index()
        {
            try
            {
                var workspaces = await _workspaceService.GetUserWorkspacesAsync(GetUserId());
                return View(workspaces);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error loading workspaces: {ex.Message}");
                TempData["ErrorMessage"] = "Error loading workspaces";
                return RedirectToAction("Dashboard", "Home");
            }
        }

        [HttpGet]
        [Route("Create")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Route("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateWorkspaceDto model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var workspace = await _workspaceService.CreateWorkspaceAsync(model, GetUserId());
                TempData["SuccessMessage"] = "Workspace created successfully";
                return RedirectToAction(nameof(Details), new { id = workspace.Id });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error creating workspace: {ex.Message}");
                ModelState.AddModelError(string.Empty, "Error creating workspace");
                return View(model);
            }
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var workspace = await _workspaceService.GetWorkspaceAsync(id);
                if (workspace == null)
                    return NotFound();

                var projects = await _projectService.GetWorkspaceProjectsAsync(id);
                ViewBag.Projects = projects;

                return View(workspace);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error loading workspace: {ex.Message}");
                return NotFound();
            }
        }

        [HttpGet]
        [Route("{id}/Edit")]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var workspace = await _workspaceService.GetWorkspaceAsync(id);
                if (workspace == null)
                    return NotFound();

                var updateDto = new UpdateWorkspaceDto
                {
                    Name = workspace.Name,
                    Description = workspace.Description,
                    Color = workspace.Color
                };

                return View(updateDto);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error loading workspace for edit: {ex.Message}");
                return NotFound();
            }
        }

        [HttpPost]
        [Route("{id}/Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UpdateWorkspaceDto model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                await _workspaceService.UpdateWorkspaceAsync(id, model, GetUserId());
                TempData["SuccessMessage"] = "Workspace updated successfully";
                return RedirectToAction(nameof(Details), new { id });
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating workspace: {ex.Message}");
                ModelState.AddModelError(string.Empty, "Error updating workspace");
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
                await _workspaceService.DeleteWorkspaceAsync(id, GetUserId());
                TempData["SuccessMessage"] = "Workspace deleted successfully";
                return RedirectToAction(nameof(Index));
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error deleting workspace: {ex.Message}");
                TempData["ErrorMessage"] = "Error deleting workspace";
                return RedirectToAction(nameof(Details), new { id });
            }
        }

        [HttpGet]
        [Route("{id}/Members")]
        public async Task<IActionResult> Members(int id)
        {
            try
            {
                var users = await _workspaceService.GetWorkspaceUsersAsync(id, GetUserId());
                ViewBag.WorkspaceId = id;
                return View(users);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error loading members: {ex.Message}");
                return NotFound();
            }
        }

        [HttpPost]
        [Route("{id}/InviteUser")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> InviteUser(int id, InviteUserDto model)
        {
            try
            {
                var (success, message) = await _workspaceService.InviteUserAsync(id, model, GetUserId());
                if (success)
                    TempData["SuccessMessage"] = "User invited successfully";
                else
                    TempData["ErrorMessage"] = message;

                return RedirectToAction(nameof(Members), new { id });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error inviting user: {ex.Message}");
                TempData["ErrorMessage"] = "Error inviting user";
                return RedirectToAction(nameof(Members), new { id });
            }
        }

        [HttpPost]
        [Route("RemoveUser")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveUser(int workspaceId, int workspaceUserId)
        {
            try
            {
                var (success, message) = await _workspaceService.RemoveUserAsync(workspaceId, workspaceUserId, GetUserId());
                if (success)
                    TempData["SuccessMessage"] = "User removed successfully";
                else
                    TempData["ErrorMessage"] = message;

                return RedirectToAction(nameof(Members), new { id = workspaceId });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error removing user: {ex.Message}");
                TempData["ErrorMessage"] = "Error removing user";
                return RedirectToAction(nameof(Members), new { id = workspaceId });
            }
        }

        [HttpPost]
        [Route("UpdateUserRole")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateUserRole(int workspaceUserId, WorkspaceRole role)
        {
            try
            {
                var (success, message) = await _workspaceService.UpdateUserRoleAsync(workspaceUserId, role, GetUserId());
                if (success)
                    TempData["SuccessMessage"] = "Role updated successfully";
                else
                    TempData["ErrorMessage"] = message;

                return Ok(new { success });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating role: {ex.Message}");
                return BadRequest(new { success = false, message = "Error updating role" });
            }
        }
    }
}
