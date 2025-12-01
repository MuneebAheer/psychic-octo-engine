using Microsoft.AspNetCore.Identity;
using ClickUpClone.Models;
using ClickUpClone.DTOs;

namespace ClickUpClone.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AuthService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<(bool Success, string Message, ApplicationUser? User)> RegisterAsync(RegisterDto dto)
        {
            if (dto.Password != dto.ConfirmPassword)
                return (false, "Passwords do not match", null);

            var user = new ApplicationUser
            {
                UserName = dto.Email,
                Email = dto.Email,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                EmailConfirmed = true // In production, require email confirmation
            };

            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
                return (false, string.Join(", ", result.Errors.Select(e => e.Description)), null);

            return (true, "User registered successfully", user);
        }

        public async Task<(bool Success, string Message)> LoginAsync(LoginDto dto)
        {
            var result = await _signInManager.PasswordSignInAsync(dto.Email, dto.Password, dto.RememberMe, false);
            if (!result.Succeeded)
                return (false, "Invalid email or password");

            return (true, "Login successful");
        }

        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
        }

        public async Task<ApplicationUser?> GetCurrentUserAsync(string userId)
        {
            return await _userManager.FindByIdAsync(userId);
        }
    }
}
