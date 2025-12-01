# ViewModels & Partial Views - Implementation Guide

## Overview
This document provides a complete guide for creating ViewModels and Partial Views to improve code organization and enable reusable UI components.

---

## 🗂️ DIRECTORY STRUCTURE TO CREATE

```
ClickUpClone/
├── ViewModels/                           ← NEW FOLDER
│   ├── Dashboard/
│   │   ├── DashboardViewModel.cs
│   │   └── DashboardIndexViewModel.cs
│   │
│   ├── Workspaces/
│   │   ├── WorkspaceIndexViewModel.cs
│   │   ├── WorkspaceDetailViewModel.cs
│   │   ├── WorkspaceCreateEditViewModel.cs
│   │   └── WorkspaceMembersViewModel.cs
│   │
│   ├── Projects/
│   │   ├── ProjectCreateEditViewModel.cs
│   │   ├── ProjectDetailViewModel.cs
│   │   └── ProjectListViewModel.cs
│   │
│   ├── Tasks/
│   │   ├── TaskIndexViewModel.cs
│   │   ├── TaskDetailViewModel.cs
│   │   ├── TaskCreateEditViewModel.cs
│   │   ├── TaskBoardViewModel.cs
│   │   └── MyTasksViewModel.cs
│   │
│   ├── Notifications/
│   │   └── NotificationIndexViewModel.cs
│   │
│   ├── ActivityLogs/
│   │   └── ActivityLogViewModel.cs
│   │
│   └── Shared/
│       ├── PaginationViewModel.cs
│       └── FilterViewModel.cs
│
└── Views/
    └── Shared/
        ├── Components/
        │   ├── _TaskCard.cshtml           ← NEW
        │   ├── _CommentThread.cshtml      ← NEW
        │   ├── _Sidebar.cshtml            ← NEW
        │   ├── _AttachmentList.cshtml     ← NEW
        │   ├── _NotificationBadge.cshtml  ← NEW
        │   ├── _SubtaskList.cshtml        ← NEW
        │   ├── _Pagination.cshtml         ← NEW
        │   └── _FilterPanel.cshtml        ← NEW
        │
        └── Modals/
            ├── _TaskQuickEdit.cshtml       ← NEW
            ├── _ConfirmDelete.cshtml       ← NEW
            └── _FileUpload.cshtml          ← NEW
```

---

## 📝 VIEWMODEL IMPLEMENTATIONS

### 1. Dashboard ViewModels

#### DashboardViewModel.cs
```csharp
namespace ClickUpClone.ViewModels.Dashboard
{
    public class DashboardViewModel
    {
        public int WorkspaceCount { get; set; }
        public int ProjectCount { get; set; }
        public int TaskCount { get; set; }
        public int OverdueTaskCount { get; set; }
        public int UnreadNotificationCount { get; set; }
        public int CompletedTasksThisWeek { get; set; }
        
        public IList<WorkspaceDto> RecentWorkspaces { get; set; } = new List<WorkspaceDto>();
        public IList<TaskDto> MyTasks { get; set; } = new List<TaskDto>();
        public IList<TaskDto> OverdueTasks { get; set; } = new List<TaskDto>();
        public IList<NotificationDto> RecentNotifications { get; set; } = new List<NotificationDto>();
        
        public Dictionary<string, int> TasksByStatus { get; set; } = new Dictionary<string, int>();
        public Dictionary<string, int> TasksByPriority { get; set; } = new Dictionary<string, int>();
    }
}
```

**Usage in Controller**:
```csharp
[HttpGet]
public async Task<IActionResult> Dashboard()
{
    var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    
    var vm = new DashboardViewModel
    {
        WorkspaceCount = (await _workspaceService.GetUserWorkspacesAsync(userId)).Count(),
        MyTasks = (await _taskService.GetUserTasksAsync(userId)).ToList(),
        RecentWorkspaces = (await _workspaceService.GetUserWorkspacesAsync(userId))
            .Take(5).ToList(),
        UnreadNotificationCount = await _notificationService.GetUnreadNotificationCountAsync(userId)
    };
    
    return View(vm);
}
```

**Usage in View**:
```html
@model DashboardViewModel

<div class="container-fluid py-4">
    <div class="row">
        <div class="col-md-3">
            <div class="card">
                <div class="card-body">
                    <h5 class="card-title">My Tasks</h5>
                    <h2>@Model.TaskCount</h2>
                </div>
            </div>
        </div>
        <!-- More cards -->
    </div>
</div>
```

---

### 2. Task Board ViewModel

#### TaskBoardViewModel.cs
```csharp
namespace ClickUpClone.ViewModels.Tasks
{
    public class TaskBoardViewModel
    {
        public ProjectDto Project { get; set; }
        public IList<TaskListDto> TaskLists { get; set; } = new List<TaskListDto>();
        
        // Grouped tasks by list for Kanban board
        public Dictionary<int, IList<TaskDto>> TasksByList { get; set; } 
            = new Dictionary<int, IList<TaskDto>>();
        
        // Team members for assignment dropdown
        public IList<ApplicationUserDto> TeamMembers { get; set; } = new List<ApplicationUserDto>();
        
        // Filter options
        public TaskStatus? FilterStatus { get; set; }
        public TaskPriority? FilterPriority { get; set; }
        public string? FilterAssignedTo { get; set; }
    }
}
```

**Usage Example**:
```csharp
[HttpGet("board/{projectId}")]
public async Task<IActionResult> Board(int projectId)
{
    var project = await _projectService.GetProjectAsync(projectId);
    if (project == null) return NotFound();
    
    var taskLists = await _taskListService.GetProjectTaskListsAsync(projectId);
    
    var vm = new TaskBoardViewModel
    {
        Project = project,
        TaskLists = taskLists.ToList()
    };
    
    // Group tasks by list
    foreach (var list in taskLists)
    {
        var tasks = await _taskService.GetListTasksAsync(list.Id);
        vm.TasksByList[list.Id] = tasks.ToList();
    }
    
    return View(vm);
}
```

---

### 3. Task Detail ViewModel

#### TaskDetailViewModel.cs
```csharp
namespace ClickUpClone.ViewModels.Tasks
{
    public class TaskDetailViewModel
    {
        public TaskDto Task { get; set; }
        public IList<SubtaskDto> Subtasks { get; set; } = new List<SubtaskDto>();
        public IList<CommentDto> Comments { get; set; } = new List<CommentDto>();
        public IList<AttachmentDto> Attachments { get; set; } = new List<AttachmentDto>();
        public IList<ActivityLogDto> ActivityLogs { get; set; } = new List<ActivityLogDto>();
        
        // For dropdowns
        public IList<ApplicationUserDto> AssigneeOptions { get; set; } = new List<ApplicationUserDto>();
        public IList<string> StatusOptions { get; set; } = new List<string>();
        public IList<string> PriorityOptions { get; set; } = new List<string>();
    }
}
```

---

### 4. Workspace ViewModel

#### WorkspaceDetailViewModel.cs
```csharp
namespace ClickUpClone.ViewModels.Workspaces
{
    public class WorkspaceDetailViewModel
    {
        public WorkspaceDto Workspace { get; set; }
        public IList<ProjectDto> Projects { get; set; } = new List<ProjectDto>();
        public IList<WorkspaceUserDto> Members { get; set; } = new List<WorkspaceUserDto>();
        public int ProjectCount => Projects.Count;
        public int MemberCount => Members.Count;
    }
}
```

---

## 🎨 PARTIAL VIEW IMPLEMENTATIONS

### 1. Task Card Component

#### Views/Shared/Components/_TaskCard.cshtml
```html
@model TaskDto

<div class="card task-card mb-2" data-task-id="@Model.Id" draggable="true">
    <div class="card-body p-3">
        <div class="d-flex justify-content-between align-items-start">
            <div class="flex-grow-1">
                <h6 class="card-title mb-1">
                    <a href="@Url.Action("Details", "Tasks", new { id = Model.Id })" 
                       class="text-decoration-none">
                        @Model.Title
                    </a>
                </h6>
                
                @if (!string.IsNullOrEmpty(Model.Description))
                {
                    <p class="card-text text-muted small mb-2">
                        @Model.Description.Substring(0, Math.Min(50, Model.Description.Length))...
                    </p>
                }
            </div>
        </div>
        
        <div class="task-metadata mt-2">
            <!-- Status Badge -->
            <span class="badge bg-@GetStatusColor(Model.Status) me-1">
                @Model.Status
            </span>
            
            <!-- Priority Badge -->
            <span class="badge bg-@GetPriorityColor(Model.Priority)">
                @Model.Priority
            </span>
            
            <!-- Due Date -->
            @if (Model.DueDate.HasValue)
            {
                <small class="d-block text-muted mt-2">
                    Due: @Model.DueDate.Value.ToString("MMM dd")
                </small>
            }
        </div>
    </div>
</div>

@functions {
    string GetStatusColor(string status) => status switch {
        "ToDo" => "secondary",
        "InProgress" => "info",
        "InReview" => "warning",
        "Done" => "success",
        _ => "secondary"
    };
    
    string GetPriorityColor(string priority) => priority switch {
        "Urgent" => "danger",
        "High" => "warning",
        "Normal" => "info",
        "Low" => "success",
        _ => "secondary"
    };
}
```

**Usage in View**:
```html
@foreach (var task in Model.Tasks)
{
    <partial name="Components/_TaskCard" model="task" />
}
```

---

### 2. Comment Thread Component

#### Views/Shared/Components/_CommentThread.cshtml
```html
@model IList<CommentDto>

<div class="comments-section mt-4">
    <h6>Comments (@Model.Count)</h6>
    
    <div class="comments-list">
        @foreach (var comment in Model.OrderByDescending(c => c.CreatedAt))
        {
            <div class="card mb-3" data-comment-id="@comment.Id">
                <div class="card-body">
                    <div class="d-flex justify-content-between">
                        <strong>@comment.UserName</strong>
                        <small class="text-muted">@comment.CreatedAt.ToString("MMM dd HH:mm")</small>
                    </div>
                    
                    <p class="comment-content mb-2">@Html.Raw(comment.Content)</p>
                    
                    @if (comment.IsEdited)
                    {
                        <small class="text-muted">edited</small>
                    }
                    
                    <div class="comment-actions">
                        <button class="btn btn-sm btn-link" onclick="editCommentUI(@comment.Id)">
                            Edit
                        </button>
                        <button class="btn btn-sm btn-link text-danger" 
                                onclick="deleteComment(@comment.Id)">
                            Delete
                        </button>
                    </div>
                </div>
            </div>
        }
    </div>
    
    <!-- Add Comment Form -->
    <form class="mt-3" onsubmit="addCommentHandler(event, @ViewBag.TaskId)">
        <textarea class="form-control" id="comment-text" placeholder="Add a comment..." 
                  rows="3" required></textarea>
        <button type="submit" class="btn btn-sm btn-primary mt-2">Post Comment</button>
    </form>
</div>
```

---

### 3. Sidebar Navigation

#### Views/Shared/Components/_Sidebar.cshtml
```html
@model IList<WorkspaceDto>

<nav class="sidebar bg-light p-3">
    <div class="sidebar-header mb-4">
        <h5>ClickUp Clone</h5>
    </div>
    
    <!-- Workspaces -->
    <div class="workspaces-section">
        <h6 class="sidebar-title mb-3">Workspaces</h6>
        
        @foreach (var workspace in Model)
        {
            <a href="@Url.Action("Details", "Workspaces", new { id = workspace.Id })" 
               class="sidebar-item d-block mb-2 p-2 rounded text-decoration-none 
                      @(ViewBag.CurrentWorkspaceId == workspace.Id ? "active bg-primary text-white" : "")">
                <i class="bi bi-briefcase"></i>
                @workspace.Name
            </a>
        }
        
        <a href="@Url.Action("Create", "Workspaces")" class="sidebar-item d-block p-2 text-primary">
            <i class="bi bi-plus-circle"></i> New Workspace
        </a>
    </div>
    
    <!-- Navigation -->
    <div class="navigation-section mt-4">
        <a href="@Url.Action("Dashboard", "Home")" class="sidebar-item d-block mb-2 p-2 text-decoration-none">
            <i class="bi bi-grid-3x3-gap"></i> Dashboard
        </a>
        <a href="@Url.Action("MyTasks", "Tasks")" class="sidebar-item d-block mb-2 p-2 text-decoration-none">
            <i class="bi bi-check-square"></i> My Tasks
        </a>
        <a href="@Url.Action("Index", "Notifications")" class="sidebar-item d-block mb-2 p-2 text-decoration-none">
            <i class="bi bi-bell"></i> Notifications
        </a>
    </div>
</nav>
```

---

### 4. Attachment List

#### Views/Shared/Components/_AttachmentList.cshtml
```html
@model IList<AttachmentDto>

<div class="attachments-section mt-4">
    <h6>Attachments (@Model.Count)</h6>
    
    <div class="attachments-list list-group">
        @foreach (var attachment in Model.OrderByDescending(a => a.CreatedAt))
        {
            <div class="list-group-item d-flex justify-content-between align-items-center" 
                 data-attachment-id="@attachment.Id">
                <div>
                    <a href="@attachment.FilePath" target="_blank" class="text-decoration-none">
                        <i class="bi bi-file"></i>
                        @attachment.FileName
                    </a>
                    <br>
                    <small class="text-muted">
                        @((attachment.FileSize / 1024.0).ToString("0.##")) KB • 
                        Uploaded by @attachment.UploadedByName
                    </small>
                </div>
                <button class="btn btn-sm btn-danger" onclick="deleteAttachment(@attachment.Id)">
                    Delete
                </button>
            </div>
        }
    </div>
    
    <!-- Upload Form -->
    <form class="mt-3">
        <div class="input-group">
            <input type="file" class="form-control" id="file-input">
            <button class="btn btn-primary" type="button" 
                    onclick="handleFileUpload(@ViewBag.TaskId)">
                Upload
            </button>
        </div>
    </form>
</div>
```

---

### 5. Pagination Component

#### Views/Shared/Components/_Pagination.cshtml
```html
@model PaginationViewModel

@if (Model.TotalPages > 1)
{
    <nav aria-label="Page navigation" class="mt-4">
        <ul class="pagination">
            <!-- Previous -->
            <li class="page-item @(Model.CurrentPage == 1 ? "disabled" : "")">
                <a class="page-link" href="@Model.GetPageUrl(Model.CurrentPage - 1)">Previous</a>
            </li>
            
            <!-- Pages -->
            @for (int i = Math.Max(1, Model.CurrentPage - 2); 
                  i <= Math.Min(Model.TotalPages, Model.CurrentPage + 2); 
                  i++)
            {
                <li class="page-item @(i == Model.CurrentPage ? "active" : "")">
                    <a class="page-link" href="@Model.GetPageUrl(i)">@i</a>
                </li>
            }
            
            <!-- Next -->
            <li class="page-item @(Model.CurrentPage == Model.TotalPages ? "disabled" : "")">
                <a class="page-link" href="@Model.GetPageUrl(Model.CurrentPage + 1)">Next</a>
            </li>
        </ul>
    </nav>
}

@functions {
    public class PaginationViewModel
    {
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; }
        public int PageSize { get; set; } = 10;
        
        public string GetPageUrl(int page)
        {
            return $"?page={page}&pageSize={PageSize}";
        }
    }
}
```

---

## 🔧 IMPLEMENTATION CHECKLIST

### Step 1: Create Folder Structure
- [ ] Create `ViewModels` folder
- [ ] Create ViewModels subfolders (Dashboard, Workspaces, Projects, Tasks, etc.)
- [ ] Create `Views/Shared/Components` folder
- [ ] Create `Views/Shared/Modals` folder

### Step 2: Create ViewModels (Priority)
- [ ] DashboardViewModel
- [ ] TaskBoardViewModel
- [ ] TaskDetailViewModel
- [ ] WorkspaceDetailViewModel
- [ ] ProjectDetailViewModel
- [ ] WorkspaceIndexViewModel
- [ ] TaskIndexViewModel

### Step 3: Create Partial Views (Priority)
- [ ] _TaskCard.cshtml
- [ ] _CommentThread.cshtml
- [ ] _Sidebar.cshtml
- [ ] _AttachmentList.cshtml
- [ ] _SubtaskList.cshtml
- [ ] _Pagination.cshtml

### Step 4: Update Controllers
- [ ] HomeController.Dashboard() - use DashboardViewModel
- [ ] TasksController.Board() - use TaskBoardViewModel
- [ ] TasksController.Details() - use TaskDetailViewModel
- [ ] WorkspacesController.Details() - use WorkspaceDetailViewModel

### Step 5: Update Views
- [ ] Update _Layout.cshtml to include sidebar
- [ ] Update Dashboard view
- [ ] Update Task Board view
- [ ] Update Task Detail view
- [ ] Update Workspace views

### Step 6: Update _Layout.cshtml
- [ ] Add sidebar reference: `<partial name="_Sidebar" />`
- [ ] Add AJAX script reference: `<script src="~/js/ajax.js"></script>`
- [ ] Add Bootstrap Icons CDN
- [ ] Update navbar styling

### Step 7: Test All Views
- [ ] Test dashboard loading
- [ ] Test task board rendering
- [ ] Test partial views rendering
- [ ] Test AJAX functionality
- [ ] Test pagination
- [ ] Test responsive design

---

## 📊 Migration Path

### Phase 1 (This Week)
1. Create all ViewModels
2. Create critical partial views
3. Update main controllers
4. Update main views

### Phase 2 (Next Week)  
1. Update remaining views
2. Implement Kanban board view
3. Add filtering UI
4. Performance optimization

---

## 💾 Code Example: Migration

### BEFORE (Without ViewModel)
```csharp
// Controller
var tasks = await _taskService.GetUserTasksAsync(userId);
var workspaces = await _workspaceService.GetUserWorkspacesAsync(userId);
ViewBag.TaskCount = tasks.Count();
ViewBag.WorkspaceCount = workspaces.Count();
ViewBag.RecentWorkspaces = workspaces.Take(5);
return View(tasks);
```

```html
<!-- View -->
@{
    var taskCount = (int)ViewBag.TaskCount;
    var workspaceCount = (int)ViewBag.WorkspaceCount;
}
<h2>Dashboard (@taskCount tasks)</h2>
```

### AFTER (With ViewModel)
```csharp
// Controller
var vm = new DashboardViewModel
{
    TaskCount = tasks.Count(),
    WorkspaceCount = workspaces.Count(),
    MyTasks = tasks.ToList(),
    RecentWorkspaces = workspaces.Take(5).ToList()
};
return View(vm);
```

```html
<!-- View -->
<h2>Dashboard (@Model.TaskCount tasks)</h2>
```

---

## 🎯 BENEFITS OF THIS REFACTORING

1. **Type Safety**: Strong typing instead of ViewBag
2. **IntelliSense**: Full IDE support
3. **Maintainability**: Clear data contracts
4. **Reusability**: Partial views used across multiple pages
5. **Testability**: ViewModels can be unit tested
6. **Documentation**: Clear structure of required data

---

**Status**: Ready to implement  
**Estimated Time**: 6-8 hours  
**Complexity**: Medium  
**Priority**: High (blocks other improvements)

