using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using ClickUpClone.Services;
using ClickUpClone.DTOs;

namespace ClickUpClone.Controllers
{
    [Authorize]
    [Route("[controller]")]
    public class ProjectsController : Controller
    {
        private readonly IProjectService _projectService;
        private readonly IListService _listService;
        private readonly IWorkspaceService _workspaceService;
        private readonly ILogger<ProjectsController> _logger;

        public ProjectsController(
            IProjectService projectService,
            IListService listService,
            IWorkspaceService workspaceService,
            ILogger<ProjectsController> logger)
        {
            _projectService = projectService;
            _listService = listService;
            _workspaceService = workspaceService;
            _logger = logger;
        }

        private string GetUserId() => User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;

        [HttpGet]
        [Route("")]
        public async Task<IActionResult> Index(int workspaceId)
        {
            try
            {
                var projects = await _projectService.GetWorkspaceProjectsAsync(workspaceId);
                ViewBag.WorkspaceId = workspaceId;
                return View(projects);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error loading projects: {ex.Message}");
                TempData["ErrorMessage"] = "Error loading projects";
                return RedirectToAction("Details", "Workspaces", new { id = workspaceId });
            }
        }

        [HttpGet]
        [Route("Create")]
        public IActionResult Create(int workspaceId)
        {
            ViewBag.WorkspaceId = workspaceId;
            return View();
        }

        [HttpPost]
        [Route("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateProjectDto model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.WorkspaceId = model.WorkspaceId;
                return View(model);
            }

            try
            {
                var project = await _projectService.CreateProjectAsync(model, GetUserId());
                TempData["SuccessMessage"] = "Project created successfully";
                return RedirectToAction(nameof(Details), new { id = project.Id });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error creating project: {ex.Message}");
                ModelState.AddModelError(string.Empty, "Error creating project");
                ViewBag.WorkspaceId = model.WorkspaceId;
                return View(model);
            }
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var project = await _projectService.GetProjectAsync(id);
                if (project == null)
                    return NotFound();

                var lists = await _listService.GetProjectListsAsync(id);
                ViewBag.Lists = lists;

                return View(project);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error loading project: {ex.Message}");
                return NotFound();
            }
        }

        [HttpGet]
        [Route("{id}/Edit")]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var project = await _projectService.GetProjectAsync(id);
                if (project == null)
                    return NotFound();

                var updateDto = new UpdateProjectDto
                {
                    Name = project.Name,
                    Description = project.Description,
                    Color = project.Color
                };

                return View(updateDto);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error loading project for edit: {ex.Message}");
                return NotFound();
            }
        }

        [HttpPost]
        [Route("{id}/Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UpdateProjectDto model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                await _projectService.UpdateProjectAsync(id, model, GetUserId());
                TempData["SuccessMessage"] = "Project updated successfully";
                return RedirectToAction(nameof(Details), new { id });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating project: {ex.Message}");
                ModelState.AddModelError(string.Empty, "Error updating project");
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
                var project = await _projectService.GetProjectAsync(id);
                if (project == null)
                    return NotFound();

                await _projectService.DeleteProjectAsync(id, GetUserId());
                TempData["SuccessMessage"] = "Project deleted successfully";
                return RedirectToAction(nameof(Index), new { workspaceId = project.WorkspaceId });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error deleting project: {ex.Message}");
                TempData["ErrorMessage"] = "Error deleting project";
                return RedirectToAction(nameof(Details), new { id });
            }
        }
    }
}
