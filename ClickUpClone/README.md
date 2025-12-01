# ClickUp Clone - ASP.NET Core MVC

A full-featured project management and task tracking application built with ASP.NET Core MVC, Entity Framework Core, and SQL Server.

## Features

- **User Authentication & Authorization**
  - User registration and login
  - Role-based access control (Owner, Admin, Member)
  - Secure password management

- **Workspace Management**
  - Create and manage multiple workspaces
  - Add/remove members
  - Assign roles to team members

- **Project & List Management**
  - Create projects within workspaces
  - Organize tasks into lists
  - Support for multiple projects per workspace

- **Task Management**
  - Create, read, update, delete tasks
  - Assign tasks to team members
  - Set priorities and due dates
  - Track task status (To Do, In Progress, In Review, Done)
  - Add subtasks to break down work
  - Add comments for collaboration
  - Attach files to tasks

- **Views & Filters**
  - List view for tasks
  - Filter tasks by status, priority, and assignee
  - My Tasks dashboard showing assigned work

- **Activity & Notifications**
  - Activity logging for all workspace events
  - Notification system for task updates
  - User notification dashboard

## Tech Stack

- **Framework**: ASP.NET Core 8.0 MVC
- **Database**: SQL Server
- **ORM**: Entity Framework Core 8.0
- **Authentication**: ASP.NET Core Identity
- **UI**: Bootstrap 5.3, HTML5, CSS3, JavaScript
- **Architecture**: Clean MVC Pattern with Repositories and Services

## Project Structure

```
ClickUpClone/
├── Models/                 # Data models
├── DTOs/                  # Data transfer objects
├── Data/                  # Database context
├── Repositories/          # Data access layer
├── Services/              # Business logic layer
├── Controllers/           # MVC controllers
├── Views/                 # Razor views
├── wwwroot/              # Static files (CSS, JS, images)
├── Migrations/           # EF Core migrations
├── Program.cs            # Application startup
├── appsettings.json      # Configuration
└── ClickUpClone.csproj   # Project file
```

## Getting Started

### Prerequisites

- .NET 8.0 SDK or later
- SQL Server 2019 or later (or SQL Server Express)
- Visual Studio 2022 or Visual Studio Code with C# extension

### Installation

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd ClickUpClone
   ```

2. **Update Connection String**
   Edit `appsettings.json` and update the SQL Server connection string:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=YOUR_SERVER;Database=ClickUpCloneDb;Trusted_Connection=true;TrustServerCertificate=true;"
   }
   ```

3. **Restore NuGet Packages**
   ```bash
   dotnet restore
   ```

4. **Apply Database Migrations**
   ```bash
   dotnet ef database update
   ```

5. **Run the Application**
   ```bash
   dotnet run
   ```

   The application will start at `https://localhost:5001` (or the configured port).

## Usage

1. **Register a New Account**
   - Navigate to the Registration page
   - Create a new user account
   - Log in with your credentials

2. **Create a Workspace**
   - Click "Create Workspace" on the dashboard
   - Add workspace details (name, description, color)
   - You'll become the owner

3. **Invite Team Members**
   - Go to Workspace → Members
   - Enter team member email addresses
   - Assign roles (Admin or Member)

4. **Create Projects**
   - Click "New Project" in the workspace
   - Add project details

5. **Create Lists & Tasks**
   - Create lists within projects to organize tasks
   - Add tasks to lists with details (title, description, priority, due date)
   - Assign tasks to team members

6. **Manage Tasks**
   - Update task status, priority, and assignments
   - Add subtasks to break down work
   - Leave comments for collaboration
   - Track activity and changes

## API Endpoints

### Authentication
- `POST /account/register` - Register new user
- `POST /account/login` - Login user
- `POST /account/logout` - Logout user

### Workspaces
- `GET /workspaces` - List user workspaces
- `GET /workspaces/{id}` - Get workspace details
- `POST /workspaces/create` - Create workspace
- `POST /workspaces/{id}/edit` - Update workspace
- `POST /workspaces/{id}/delete` - Delete workspace
- `GET /workspaces/{id}/members` - List workspace members
- `POST /workspaces/{id}/inviteuser` - Invite user to workspace
- `POST /workspaces/removeuser` - Remove user from workspace

### Projects
- `GET /projects` - List workspace projects
- `GET /projects/{id}` - Get project details
- `POST /projects/create` - Create project
- `POST /projects/{id}/edit` - Update project
- `POST /projects/{id}/delete` - Delete project

### Tasks
- `GET /tasks` - List tasks in a list
- `GET /tasks/{id}` - Get task details
- `POST /tasks/create` - Create task
- `POST /tasks/{id}/edit` - Update task
- `POST /tasks/{id}/delete` - Delete task
- `GET /tasks/mytasks` - Get user's assigned tasks
- `GET /tasks/filtered` - Filter tasks by status, priority, assignee

### Comments & Subtasks
- `POST /tasks/{id}/addcomment` - Add comment to task
- `POST /tasks/{id}/addsubtask` - Add subtask
- `POST /tasks/updatesubtask` - Mark subtask as complete

## Role-Based Access Control

### Owner
- Full control over workspace
- Can invite/remove members
- Can assign roles
- Can delete workspace

### Admin
- Can manage projects and tasks
- Can invite members as Member role
- Cannot delete workspace or change own role

### Member
- Can view and work on assigned tasks
- Can create tasks and comments
- Limited to viewing workspace content

## Database Schema

Key entities:
- **ApplicationUser** - System users
- **Workspace** - Team workspaces
- **WorkspaceUser** - User-workspace membership
- **Project** - Projects within workspaces
- **List** - Task lists within projects
- **Task** - Individual tasks
- **Subtask** - Subtasks within tasks
- **Comment** - Comments on tasks
- **Attachment** - File attachments to tasks
- **ActivityLog** - Activity tracking
- **Notification** - User notifications

## Development

### Running Locally

```bash
# Development
dotnet run

# With watch mode
dotnet watch run
```

### Creating Migrations

```bash
# Add a new migration
dotnet ef migrations add MigrationName

# Update database
dotnet ef database update

# Remove last migration
dotnet ef migrations remove
```

### Building

```bash
dotnet build
```

### Publishing

```bash
dotnet publish -c Release -o ./publish
```

## Deployment

### Deploy to IIS

1. Publish the application:
   ```bash
   dotnet publish -c Release
   ```

2. Create a new application in IIS
3. Set application pool .NET CLR version to "No Managed Code"
4. Point physical path to the publish folder
5. Set connection string in `appsettings.json` or environment variable
6. Ensure SQL Server is accessible from IIS server

### Deploy to Azure

1. Create an Azure App Service
2. Create an Azure SQL Database
3. Publish from Visual Studio:
   - Right-click project → Publish
   - Select Azure as target
   - Configure database connection
   - Set connection string in Application Settings

## Performance Optimization

- Database queries are optimized with eager loading
- Navigation properties are properly configured
- Indexes are set on foreign keys
- Pagination can be added for large datasets
- Static assets are cached

## Security Considerations

- All user inputs are validated
- Passwords are hashed using ASP.NET Core Identity
- CSRF protection enabled on all forms
- SQL injection is prevented through EF Core parameterized queries
- Authentication required for most operations
- Authorization checks on sensitive operations

## Future Enhancements

- Real-time notifications using SignalR
- File upload and storage (Azure Blob, AWS S3)
- Email notifications
- Advanced reporting and analytics
- Gantt chart view
- Calendar view for tasks
- Mobile app (Xamarin)
- API for third-party integrations
- Dark mode support
- Multi-language support

## Testing

Unit tests can be added using xUnit:

```bash
dotnet add package xunit
dotnet add package Moq
```

## Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Submit a pull request

## License

MIT License - feel free to use this project for personal or commercial purposes.

## Support

For issues, questions, or suggestions:
- Create an GitHub issue
- Contact the development team

## Changelog

### Version 1.0.0 (2024)
- Initial release
- Core workspace management
- Project and task management
- User authentication and authorization
- Team collaboration features
- Activity logging and notifications

---

**Built with ❤️ using ASP.NET Core**
