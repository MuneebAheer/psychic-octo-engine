using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ClickUpClone.DTOs;
using ClickUpClone.Services;

namespace ClickUpClone.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AccountController> _logger;

        public AccountController(IAuthService authService, ILogger<AccountController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Register()
        {
            if (User.Identity?.IsAuthenticated == true)
                return RedirectToAction("Dashboard", "Home");

            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterDto model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var (success, message, user) = await _authService.RegisterAsync(model);

            if (!success)
            {
                ModelState.AddModelError(string.Empty, message);
                return View(model);
            }

            _logger.LogInformation($"User {user?.Email} registered successfully");
            TempData["SuccessMessage"] = "Registration successful! Please log in.";
            return RedirectToAction(nameof(Login));
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            if (User.Identity?.IsAuthenticated == true)
                return RedirectToAction("Dashboard", "Home");

            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginDto model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (!ModelState.IsValid)
                return View(model);

            var (success, message) = await _authService.LoginAsync(model);

            if (!success)
            {
                ModelState.AddModelError(string.Empty, message);
                return View(model);
            }

            _logger.LogInformation($"User {model.Email} logged in successfully");

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction("Dashboard", "Home");
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _authService.LogoutAsync();
            _logger.LogInformation($"User {User.Identity?.Name} logged out");
            return RedirectToAction("Index", "Home");
        }

        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
