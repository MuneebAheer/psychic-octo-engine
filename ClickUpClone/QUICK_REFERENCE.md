# Quick Reference Guide - ViewModels & Partial Views

## 📋 Quick Navigation

### ViewModels Location
- `ViewModels/Dashboard/` - DashboardViewModel
- `ViewModels/Tasks/` - TaskBoardViewModel, TaskDetailViewModel, TaskIndexViewModel, MyTasksViewModel
- `ViewModels/Workspaces/` - WorkspaceDetailViewModel, WorkspaceIndexViewModel, WorkspaceUserDto
- `ViewModels/Projects/` - ProjectDetailViewModel, ProjectListViewModel
- `ViewModels/Shared/` - PaginationViewModel, FilterViewModel

### Partial Views Location
- `Views/Shared/Components/` - Reusable UI components
- `Views/Shared/Modals/` - Dialog boxes

---

## 🎯 Common Tasks

### 1. Display Task List with Partial View

**Controller:**
```csharp
public async Task<IActionResult> Index()
{
    var tasks = await _taskService.GetTasksAsync();
    return View(tasks);
}
```

**View:**
```html
@model IList<TaskDto>

<div class="task-list">
    @foreach (var task in Model)
    {
        <partial name="Components/_TaskCard" model="task" />
    }
</div>
```

**Output**: Cards displayed with AJAX-enabled status/priority updates

---

### 2. Show Task Details with all Related Data

**Controller:**
```csharp
public async Task<IActionResult> Details(int id)
{
    var vm = new TaskDetailViewModel
    {
        Task = await _taskService.GetTaskAsync(id),
        Subtasks = await _taskService.GetSubtasksAsync(id),
        Comments = await _commentService.GetTaskCommentsAsync(id),
        Attachments = await _attachmentService.GetTaskAttachmentsAsync(id),
        AssigneeOptions = await _workspaceService.GetWorkspaceUsersAsync(workspaceId)
    };
    return View(vm);
}
```

**View:**
```html
@model TaskDetailViewModel

<h2>@Model.Task.Title</h2>

<partial name="Components/_CommentThread" model="@Model.Comments" />
<partial name="Components/_AttachmentList" model="@Model.Attachments" />
<partial name="Components/_SubtaskList" model="@Model.Subtasks" />
```

**Output**: Full task detail page with all interactive components

---

### 3. Confirm Delete Action

**JavaScript:**
```javascript
function deleteTask(taskId) {
    showDeleteConfirm('Delete this task?', () => {
        // Your deletion logic here
        fetch(`/api/tasks/${taskId}`, { method: 'DELETE' });
    });
}
```

**How it works:**
1. Modal appears with confirmation message
2. User clicks Delete
3. Callback function executes
4. Modal closes automatically

---

### 4. Upload File to Task

**HTML Button:**
```html
<button onclick="openFileUploadModal(@Model.Task.Id)">Upload File</button>
```

**How it works:**
1. Modal opens with file input
2. User selects file
3. Submitting calls `uploadAttachment()` from ajax.js
4. File uploads via FormData
5. Modal closes
6. Attachment list updates

---

### 5. Paginate Task List

**Controller:**
```csharp
public async Task<IActionResult> Index(int page = 1, int pageSize = 10)
{
    var total = await _taskService.GetTaskCountAsync();
    var tasks = await _taskService.GetTasksAsync((page - 1) * pageSize, pageSize);
    
    var pagination = new PaginationViewModel
    {
        CurrentPage = page,
        PageSize = pageSize,
        TotalCount = total
    };
    
    return View(new { Tasks = tasks, Pagination = pagination });
}
```

**View:**
```html
@foreach (var task in Model.Tasks)
{
    <partial name="Components/_TaskCard" model="task" />
}

<partial name="Components/_Pagination" model="@Model.Pagination" />
```

---

### 6. Update Dashboard Statistics

**Controller:**
```csharp
public async Task<IActionResult> Dashboard()
{
    var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    var tasks = await _taskService.GetUserTasksAsync(userId);
    
    var vm = new DashboardViewModel
    {
        TaskCount = tasks.Count(),
        OverdueTaskCount = tasks.Count(t => t.DueDate < DateTime.Now),
        TasksByStatus = tasks.GroupBy(t => t.Status)
            .ToDictionary(g => g.Key.ToString(), g => g.Count()),
        MyTasks = tasks.Take(10).ToList()
    };
    
    return View(vm);
}
```

---

### 7. Quick Edit Task Modal

**Button to trigger:**
```html
<button onclick="openQuickEditModal(@task.Id, '@task.Title', '@task.Status', 
                                    '@task.Priority', '@task.DueDate', 
                                    '@task.Description')">
    Quick Edit
</button>
```

**Modal updates:**
- Status dropdown triggers AJAX via `updateTaskStatus()`
- Priority dropdown triggers AJAX via `updateTaskPriority()`
- Submit button for full update (needs implementation)

---

### 8. Add to Sidebar Navigation

**Edit**: `Views/Shared/Components/_Sidebar.cshtml`

```html
<a href="@Url.Action("YourAction", "YourController")" 
   class="sidebar-item d-flex align-items-center p-2 text-decoration-none text-dark">
    <i class="bi bi-icon-name"></i> Label
</a>
```

---

## 🔧 Extending Components

### Create Custom Partial View

**Step 1**: Create file in `Views/Shared/Components/`

```html
<!-- Views/Shared/Components/_MyComponent.cshtml -->
@model YourDto

<div class="my-component">
    @foreach (var item in Model)
    {
        <div>@item.Property</div>
    }
</div>
```

**Step 2**: Use in view

```html
<partial name="Components/_MyComponent" model="@Model.Items" />
```

---

### Create New ViewModel

**Step 1**: Create in appropriate folder

```csharp
// ViewModels/Custom/MyViewModel.cs
public class MyViewModel
{
    public IList<SomeDto> Items { get; set; } = new List<SomeDto>();
    public int TotalCount { get; set; }
}
```

**Step 2**: Use in controller

```csharp
var vm = new MyViewModel { Items = items, TotalCount = items.Count() };
return View(vm);
```

**Step 3**: Declare in view

```html
@model MyViewModel

<h2>@Model.TotalCount items</h2>
```

---

## 📦 Important Classes Reference

### DashboardViewModel
```csharp
public int WorkspaceCount { get; set; }
public int ProjectCount { get; set; }
public int TaskCount { get; set; }
public IList<TaskDto> MyTasks { get; set; }
public Dictionary<string, int> TasksByStatus { get; set; }
```

### TaskDetailViewModel
```csharp
public TaskDto Task { get; set; }
public IList<CommentDto> Comments { get; set; }
public IList<AttachmentDto> Attachments { get; set; }
public IList<SubtaskDto> Subtasks { get; set; }
public IList<ActivityLogDto> ActivityLogs { get; set; }
```

### TaskBoardViewModel
```csharp
public ProjectDto Project { get; set; }
public IList<TaskListDto> TaskLists { get; set; }
public Dictionary<int, IList<TaskDto>> TasksByList { get; set; }
public IList<ApplicationUserDto> TeamMembers { get; set; }
```

### PaginationViewModel
```csharp
public int CurrentPage { get; set; }
public int PageSize { get; set; }
public int TotalCount { get; set; }
public int TotalPages { get; }
public bool HasPreviousPage { get; }
public bool HasNextPage { get; }
```

---

## 🎨 Bootstrap Icons Used

| Icon | Class | Usage |
|------|-------|-------|
| Briefcase | `bi-briefcase` | Workspaces |
| Kanban | `bi-kanban` | Projects |
| Check Square | `bi-check-square` | Tasks |
| Bell | `bi-bell` | Notifications |
| Speedometer | `bi-speedometer2` | Dashboard |
| Pencil | `bi-pencil` | Edit |
| Trash | `bi-trash` | Delete |
| Plus | `bi-plus` | Add/Create |
| Download | `bi-download` | Download |
| Cloud Upload | `bi-cloud-upload` | Upload |

[Full list](https://icons.getbootstrap.com/)

---

## 🔗 AJAX Integration Points

### From _CommentThread.cshtml
```javascript
addComment(taskId, content)          // POST new comment
updateComment(commentId, content)    // PUT existing comment
deleteComment(commentId)              // DELETE comment
```

### From _AttachmentList.cshtml
```javascript
uploadAttachment(taskId, file)       // POST file with FormData
deleteAttachment(attachmentId)       // DELETE file
```

### From _SubtaskList.cshtml
```javascript
toggleSubtask(subtaskId)             // PATCH subtask completed status
```

### From _TaskCard.cshtml
```javascript
// Drag and drop setup
makeDraggable(element)               // Make task draggable
makeDropZone(listElement, listId)    // Make list droppable
```

---

## 🚀 Performance Tips

1. **Use `Take(n)` for recent items**
   ```csharp
   MyTasks = userTasks.Take(10).ToList()
   ```

2. **Include only necessary data in DTOs**
   ```csharp
   // Good: Only properties needed for card
   var taskCard = new TaskCardDto { Id, Title, Status, Priority };
   
   // Bad: Everything including comments, attachments
   var fullTask = task; // Too much data
   ```

3. **Lazy load details on demand**
   ```html
   <a href="/tasks/@Model.Id">View Full Details</a>
   <!-- Don't include all comments/attachments on list page -->
   ```

---

## 🐛 Debugging Checklist

- [ ] View has `@model` declaration
- [ ] Model properties match ViewModel
- [ ] Partial name includes `Components/` or `Modals/` prefix
- [ ] `@Url.Action()` uses correct controller/action names
- [ ] Bootstrap CSS and Icons CDN loaded
- [ ] ajax.js script loaded for authenticated users
- [ ] Controller returns correct ViewModel type
- [ ] Database includes required data (not deleted/inactive)

---

## 📝 Common Patterns

### Pattern 1: List + Detail
```csharp
// Index action
var vm = new ItemIndexViewModel { Items = items };
return View(vm);

// Details action  
var vm = new ItemDetailViewModel { Item = item, RelatedItems = related };
return View(vm);
```

### Pattern 2: Create + Edit
```csharp
// GET: Show form
var vm = new ItemCreateEditViewModel();
return View(vm);

// POST: Save
var vm = new ItemCreateEditViewModel { /* form data */ };
if (ModelState.IsValid) { /* save */ }
return View(vm);
```

### Pattern 3: List + Filter + Paginate
```csharp
var items = await _service.GetItemsAsync(filter, page, pageSize);
var vm = new ItemListViewModel 
{ 
    Items = items,
    Filter = filter,
    Pagination = new PaginationViewModel { /* ... */ }
};
return View(vm);
```

---

**Last Updated**: November 30, 2025  
**Version**: 1.0  
**Status**: Ready for Development

