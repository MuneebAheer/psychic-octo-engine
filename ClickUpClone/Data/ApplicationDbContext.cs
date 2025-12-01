using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ClickUpClone.Models;

namespace ClickUpClone.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Workspace> Workspaces { get; set; }
        public DbSet<WorkspaceUser> WorkspaceUsers { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<TaskList> TaskLists { get; set; }
        public DbSet<Models.Task> Tasks { get; set; }
        public DbSet<Subtask> Subtasks { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Attachment> Attachments { get; set; }
        public DbSet<ActivityLog> ActivityLogs { get; set; }
        public DbSet<Notification> Notifications { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Workspace
            builder.Entity<Workspace>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.HasOne(e => e.Owner)
                    .WithMany()
                    .HasForeignKey(e => e.OwnerId)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.HasMany(e => e.WorkspaceUsers)
                    .WithOne(e => e.Workspace)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // WorkspaceUser
            builder.Entity<WorkspaceUser>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => new { e.WorkspaceId, e.UserId }).IsUnique();
                entity.HasOne(e => e.Workspace)
                    .WithMany(e => e.WorkspaceUsers)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.User)
                    .WithMany(e => e.WorkspaceUsers)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Project
            builder.Entity<Project>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
                entity.HasOne(e => e.Workspace)
                    .WithMany(e => e.Projects)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.CreatedBy)
                    .WithMany()
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // TaskList
            builder.Entity<TaskList>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.HasOne(e => e.Project)
                    .WithMany(e => e.TaskLists)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Task
            builder.Entity<Models.Task>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Title).IsRequired().HasMaxLength(300);
                entity.HasOne(e => e.TaskList)
                    .WithMany(e => e.Tasks)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.Project)
                    .WithMany(e => e.Tasks)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.AssignedTo)
                    .WithMany(e => e.AssignedTasks)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            // Subtask
            builder.Entity<Subtask>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Title).IsRequired().HasMaxLength(300);
                entity.HasOne(e => e.Task)
                    .WithMany(e => e.Subtasks)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Comment
            builder.Entity<Comment>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Content).IsRequired();
                entity.HasOne(e => e.Task)
                    .WithMany(e => e.Comments)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.User)
                    .WithMany(e => e.Comments)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Attachment
            builder.Entity<Attachment>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.FileName).IsRequired().HasMaxLength(255);
                entity.Property(e => e.FilePath).IsRequired();
                entity.HasOne(e => e.Task)
                    .WithMany(e => e.Attachments)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.UploadedBy)
                    .WithMany()
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // ActivityLog
            builder.Entity<ActivityLog>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasOne(e => e.User)
                    .WithMany(e => e.ActivityLogs)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.Workspace)
                    .WithMany(e => e.ActivityLogs)
                    .OnDelete(DeleteBehavior.SetNull);
                entity.HasOne(e => e.Project)
                    .WithMany(e => e.ActivityLogs)
                    .OnDelete(DeleteBehavior.SetNull);
                entity.HasOne(e => e.Task)
                    .WithMany(e => e.ActivityLogs)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            // Notification
            builder.Entity<Notification>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasOne(e => e.User)
                    .WithMany(e => e.Notifications)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.Task)
                    .WithMany()
                    .OnDelete(DeleteBehavior.SetNull);
                entity.HasOne(e => e.Project)
                    .WithMany()
                    .OnDelete(DeleteBehavior.SetNull);
            });
        }
    }
}
