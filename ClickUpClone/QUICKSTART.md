# ClickUp Clone - Quick Start Guide

A complete, production-ready ClickUp clone built with ASP.NET Core MVC. This is a full-featured project management platform with workspaces, projects, tasks, team collaboration, and more.

## ⚡ Quick Setup (5 minutes)

### Prerequisites
- .NET 8.0 SDK ([Download](https://dotnet.microsoft.com/download))
- SQL Server 2019+ or Express ([Download](https://www.microsoft.com/en-us/sql-server/sql-server-downloads))

### Installation

```bash
# 1. Navigate to project
cd ClickUpClone

# 2. Update connection string in appsettings.json
# Edit: "Server=.;Database=ClickUpCloneDb;Trusted_Connection=true;"

# 3. Restore packages
dotnet restore

# 4. Create database
dotnet ef database update

# 5. Run application
dotnet run

# Access at: https://localhost:5001
```

## 🎯 First Steps

1. **Register** - Create your account
2. **Create Workspace** - Start with a new workspace
3. **Add Members** - Invite your team (Members tab)
4. **Create Project** - Add a project in the workspace
5. **Create Tasks** - Start managing tasks

## 📁 Project Structure

```
Models/           → Data models (Workspace, Task, User, etc.)
DTOs/            → Data Transfer Objects for API
Data/            → Database context (ApplicationDbContext)
Repositories/    → Data access layer
Services/        → Business logic layer
Controllers/     → MVC controllers
Views/           → Razor views (HTML templates)
wwwroot/         → Static files (CSS, JavaScript)
Program.cs       → Application configuration
appsettings.json → Connection strings & settings
```

## 🔑 Key Features

### 🔐 Authentication
- User registration & login
- Secure password management
- Role-based access control

### 👥 Workspaces
- Create multiple workspaces
- Invite team members
- Manage permissions (Owner, Admin, Member)

### 📊 Projects
- Organize projects within workspaces
- Create multiple lists per project
- Track all project activities

### ✅ Tasks
- Create, update, delete tasks
- Assign to team members
- Set priorities (Urgent, High, Normal, Low)
- Track status (To Do, In Progress, In Review, Done)
- Set due dates
- Add subtasks
- Leave comments
- Attach files

### 🔔 Collaboration
- Real-time activity logging
- Task notifications
- Comment threads
- Team member mentions

### 📈 Filtering & Views
- Filter by status, priority, assignee
- My Tasks dashboard
- Activity logs per workspace/project
- Notification center

## 🚀 Deployment

### Deploy to IIS
See [DEPLOYMENT.md](DEPLOYMENT.md) for detailed instructions

### Deploy to Azure
```bash
# Create resources in Azure
az group create -n ClickUpClone -l eastus
az appservice plan create -n ClickUpClonePlan -g ClickUpClone --sku B1
az webapp create -n clickupclone -g ClickUpClone -p ClickUpClonePlan

# Publish from VS
# Right-click project → Publish → Select Azure
```

### Deploy with Docker
```bash
docker build -t clickupclone .
docker run -p 80:80 -e ConnectionStrings__DefaultConnection="..." clickupclone
```

## 📚 API Endpoints

### Workspaces
- `GET /workspaces` - List all workspaces
- `GET /workspaces/{id}` - Get workspace details
- `POST /workspaces/create` - Create workspace
- `POST /workspaces/{id}/edit` - Update workspace
- `GET /workspaces/{id}/members` - List members

### Projects
- `GET /projects?workspaceId={id}` - List projects
- `GET /projects/{id}` - Get project
- `POST /projects/create` - Create project

### Tasks
- `GET /tasks?listId={id}` - List tasks
- `GET /tasks/{id}` - Get task details
- `POST /tasks/create` - Create task
- `GET /tasks/mytasks` - Get assigned tasks
- `GET /tasks/filtered` - Filter tasks

### Comments & Activity
- `POST /tasks/{id}/addcomment` - Add comment
- `GET /activitylogs/workspace/{id}` - Workspace activity
- `GET /notifications` - User notifications

## 🔧 Configuration

### Connection String
Edit `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER;Database=ClickUpCloneDb;Trusted_Connection=true;"
  }
}
```

### Email Configuration (Optional)
Add in Program.cs for email notifications:
```csharp
builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("Smtp"));
```

### Security
- Passwords are hashed using ASP.NET Core Identity
- CSRF protection enabled on all forms
- Authorization checks on sensitive operations
- Validation on all inputs

## 🧪 Testing

```bash
# Build and run tests
cd ClickUpClone.Tests
dotnet test

# Run specific test
dotnet test --filter "WorkspaceServiceTests"
```

## 📖 Documentation

- [README.md](README.md) - Full documentation
- [DEPLOYMENT.md](DEPLOYMENT.md) - Deployment guide
- Code comments throughout the project

## 🐛 Troubleshooting

**Database connection failed**
- Verify SQL Server is running
- Check connection string in appsettings.json
- Ensure database exists

**Port already in use**
- Change port in launchSettings.json
- Or kill process using the port

**Migrations failed**
- Delete Migrations folder and database
- Run: `dotnet ef migrations add InitialCreate`
- Run: `dotnet ef database update`

## 📞 Support

For issues or questions:
1. Check README.md and DEPLOYMENT.md
2. Review application logs
3. Check database connection
4. Verify .NET version: `dotnet --version`

## 🎓 Learning Resources

- ASP.NET Core: https://docs.microsoft.com/en-us/aspnet/core
- Entity Framework: https://docs.microsoft.com/en-us/ef/core
- Identity: https://docs.microsoft.com/en-us/aspnet/core/security/authentication/identity
- Bootstrap: https://getbootstrap.com/docs

## 📋 Roadmap

- [ ] Real-time notifications (SignalR)
- [ ] File uploads to cloud storage
- [ ] Email notifications
- [ ] Gantt chart view
- [ ] Calendar view
- [ ] Mobile app
- [ ] Advanced reporting
- [ ] Dark mode

## 📄 License

MIT License - Free to use for personal or commercial projects

## 👥 Contributing

Contributions welcome! Feel free to submit pull requests.

---

**Built with ❤️ using ASP.NET Core MVC**

Start building with ClickUp Clone today! 🚀
