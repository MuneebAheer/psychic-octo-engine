using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using ClickUpClone.Services;

namespace ClickUpClone.Controllers
{
    [Authorize]
    [Route("[controller]")]
    public class NotificationsController : Controller
    {
        private readonly INotificationService _notificationService;
        private readonly ILogger<NotificationsController> _logger;

        public NotificationsController(
            INotificationService notificationService,
            ILogger<NotificationsController> logger)
        {
            _notificationService = notificationService;
            _logger = logger;
        }

        private string GetUserId() => User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;

        [HttpGet]
        [Route("")]
        public async Task<IActionResult> Index()
        {
            try
            {
                var notifications = await _notificationService.GetUserNotificationsAsync(GetUserId());
                return View(notifications);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error loading notifications: {ex.Message}");
                return RedirectToAction("Dashboard", "Home");
            }
        }

        [HttpPost]
        [Route("MarkAsRead")]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            try
            {
                await _notificationService.MarkAsReadAsync(id);
                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error marking notification as read: {ex.Message}");
                return BadRequest(new { success = false });
            }
        }

        [HttpPost]
        [Route("MarkAllAsRead")]
        public async Task<IActionResult> MarkAllAsRead()
        {
            try
            {
                await _notificationService.MarkAllAsReadAsync(GetUserId());
                TempData["SuccessMessage"] = "All notifications marked as read";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error marking all as read: {ex.Message}");
                TempData["ErrorMessage"] = "Error marking notifications as read";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [Route("Delete")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _notificationService.DeleteNotificationAsync(id);
                TempData["SuccessMessage"] = "Notification deleted";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error deleting notification: {ex.Message}");
                TempData["ErrorMessage"] = "Error deleting notification";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpGet]
        [Route("GetUnreadCount")]
        public async Task<IActionResult> GetUnreadCount()
        {
            try
            {
                var count = await _notificationService.GetUnreadNotificationCountAsync(GetUserId());
                return Ok(new { count });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting unread count: {ex.Message}");
                return BadRequest(new { count = 0 });
            }
        }
    }
}
