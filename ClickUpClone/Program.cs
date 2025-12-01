using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using ClickUpClone.Data;
using ClickUpClone.Models;
using ClickUpClone.Repositories;
using ClickUpClone.Services;

var builder = WebApplicationBuilder.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllersWithViews();

// Database configuration
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// Identity configuration
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.SignIn.RequireConfirmedEmail = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// Repository dependency injection
builder.Services.AddScoped<IWorkspaceRepository, WorkspaceRepository>();
builder.Services.AddScoped<IWorkspaceUserRepository, WorkspaceUserRepository>();
builder.Services.AddScoped<IProjectRepository, ProjectRepository>();
builder.Services.AddScoped<ITaskListRepository, TaskListRepository>();
builder.Services.AddScoped<ITaskRepository, TaskRepository>();
builder.Services.AddScoped<ISubtaskRepository, SubtaskRepository>();
builder.Services.AddScoped<ICommentRepository, CommentRepository>();
builder.Services.AddScoped<IAttachmentRepository, AttachmentRepository>();
builder.Services.AddScoped<IActivityLogRepository, ActivityLogRepository>();
builder.Services.AddScoped<INotificationRepository, NotificationRepository>();

// Service dependency injection
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IWorkspaceService, WorkspaceService>();
builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddScoped<ITaskListService, TaskListService>();
builder.Services.AddScoped<ITaskService, TaskService>();
builder.Services.AddScoped<ISubtaskService, SubtaskService>();
builder.Services.AddScoped<ICommentService, CommentService>();
builder.Services.AddScoped<IAttachmentService, AttachmentService>();
builder.Services.AddScoped<IActivityLogService, ActivityLogService>();
builder.Services.AddScoped<INotificationService, NotificationService>();

// Add HttpContext accessor
builder.Services.AddHttpContextAccessor();

// Configure cookie settings
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.SlidingExpiration = true;
    options.ExpireTimeSpan = TimeSpan.FromDays(30);
});

var app = builder.Build();

// Database migration
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    context.Database.Migrate();
}

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
