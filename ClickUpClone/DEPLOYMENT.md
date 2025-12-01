# Deployment Guide - ClickUp Clone

This guide covers deploying the ClickUp Clone ASP.NET Core MVC application to various environments.

## Table of Contents
1. [Local Development](#local-development)
2. [IIS Deployment](#iis-deployment)
3. [Azure Deployment](#azure-deployment)
4. [Docker Deployment](#docker-deployment)
5. [Database Setup](#database-setup)
6. [Security Considerations](#security-considerations)
7. [Troubleshooting](#troubleshooting)

## Local Development

### Prerequisites
- .NET 8.0 SDK
- SQL Server 2019+ (LocalDB or Express)
- Visual Studio 2022 or VS Code

### Setup Steps

1. **Clone and Navigate**
   ```bash
   git clone <repository>
   cd ClickUpClone
   ```

2. **Update Connection String**
   - Edit `appsettings.Development.json`
   - Set your SQL Server connection

3. **Install Dependencies**
   ```bash
   dotnet restore
   ```

4. **Create Database**
   ```bash
   dotnet ef database update
   ```

5. **Run Application**
   ```bash
   dotnet run
   ```

   Access at: https://localhost:5001

## IIS Deployment

### Prerequisites
- Windows Server 2016+ or Windows 10+
- IIS 10+ with ASP.NET Core Hosting Module
- SQL Server (local or remote)

### Installation Steps

1. **Install ASP.NET Core Hosting Bundle**
   - Download from: https://dotnet.microsoft.com/download/dotnet
   - Run installer matching your .NET version
   - Restart IIS after installation

   ```powershell
   # Restart IIS
   iisreset
   ```

2. **Publish Application**
   ```bash
   dotnet publish -c Release -o "C:\www\ClickUpClone"
   ```

3. **Create IIS Application Pool**
   - Open IIS Manager
   - Create new Application Pool named "ClickUpClone"
   - Set .NET Runtime to "No Managed Code"
   - Set Managed Pipeline Mode to "Integrated"

4. **Create IIS Website**
   - Create new website
   - Name: ClickUpClone
   - Physical path: `C:\www\ClickUpClone`
   - Binding: http://*:80 (or HTTPS)
   - Application pool: ClickUpClone

5. **Configure Connection String**
   - Edit `C:\www\ClickUpClone\appsettings.json`
   - Update SQL Server connection string:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=YOUR_SERVER;Database=ClickUpCloneDb;Trusted_Connection=true;"
     }
   }
   ```

6. **Set Folder Permissions**
   ```powershell
   $iisAppPoolIdentity = "IIS AppPool\ClickUpClone"
   $folder = "C:\www\ClickUpClone"
   
   # Give IIS app pool read/write permissions
   icacls $folder /grant "${iisAppPoolIdentity}:(OI)(CI)(F)" /T
   ```

7. **Create Database on Server**
   ```bash
   # Connect to server
   dotnet ef database update --connection "Server=YOUR_SERVER;Database=ClickUpCloneDb;..."
   ```

8. **Start Website**
   - In IIS Manager, right-click website and select "Start"

### Troubleshooting IIS

**500.30 - ASP.NET Core app failed to start**
- Check event viewer for errors
- Verify .NET Core Hosting Bundle is installed
- Check connection string is valid

**403.14 - Directory Listing Denied**
- Add default.aspx or configure default document
- Enable the Static File module

**Access Denied**
- Verify IIS App Pool permissions on folder
- Run IIS as Administrator

## Azure Deployment

### Prerequisites
- Azure subscription
- Azure App Service
- Azure SQL Database

### Deployment Steps

1. **Create Azure Resources**
   ```bash
   # Create resource group
   az group create -n ClickUpClone -l eastus
   
   # Create App Service Plan
   az appservice plan create -n ClickUpClonePlan -g ClickUpClone -sku B1 --is-linux
   
   # Create Web App
   az webapp create -n clickupclone -g ClickUpClone -p ClickUpClonePlan --runtime "DOTNET|8.0"
   
   # Create SQL Database
   az sql server create -n clickupclone-server -g ClickUpClone -u admin -p YourPassword123!
   az sql db create -n ClickUpCloneDb -s clickupclone-server -g ClickUpClone
   ```

2. **Update appsettings.json**
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=tcp:clickupclone-server.database.windows.net,1433;Database=ClickUpCloneDb;User ID=admin;Password=YourPassword123!;..."
     }
   }
   ```

3. **Deploy from Visual Studio**
   - Right-click project → Publish
   - Select Azure as target
   - Create App Service
   - Configure database in Application Settings
   - Publish

4. **Initialize Database**
   - Connect to database via Azure
   - Run migrations:
   ```bash
   dotnet ef database update --connection "Server=tcp:clickupclone-server.database.windows.net..."
   ```

### Configure Azure App Settings

In Azure Portal:
1. Go to App Service → Configuration
2. Add Application Settings:
   ```
   ASPNETCORE_ENVIRONMENT=Production
   ```

3. Add Connection String:
   - Name: DefaultConnection
   - Value: Your SQL connection string
   - Type: SQLAzure

## Docker Deployment

### Create Dockerfile

```dockerfile
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["ClickUpClone.csproj", ""]
RUN dotnet restore "ClickUpClone.csproj"
COPY . .
RUN dotnet build "ClickUpClone.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ClickUpClone.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=publish /app/publish .
EXPOSE 80 443
ENTRYPOINT ["dotnet", "ClickUpClone.dll"]
```

### Build and Run Docker Image

```bash
# Build image
docker build -t clickupclone:latest .

# Run container
docker run -d \
  -p 80:80 \
  -p 443:443 \
  -e "ConnectionStrings__DefaultConnection=Server=sql-server;Database=ClickUpCloneDb;User ID=sa;Password=YourPassword123!" \
  --name clickupclone \
  clickupclone:latest
```

### Docker Compose

```yaml
version: '3.8'

services:
  app:
    build: .
    ports:
      - "80:80"
      - "443:443"
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
      - ConnectionStrings__DefaultConnection=Server=db;Database=ClickUpCloneDb;User ID=sa;Password=YourPassword123!
    depends_on:
      - db
    volumes:
      - ./uploads:/app/uploads

  db:
    image: mcr.microsoft.com/mssql/server:2019-latest
    environment:
      SA_PASSWORD: YourPassword123!
      ACCEPT_EULA: "Y"
    ports:
      - "1433:1433"
    volumes:
      - ./sqldata:/var/opt/mssql
```

Run with:
```bash
docker-compose up -d
```

## Database Setup

### Create Database from Migrations

```bash
# Using Entity Framework CLI
dotnet ef database update --connection "Server=.;Database=ClickUpCloneDb;Trusted_Connection=true;"

# Or manually
sqlcmd -S . -d ClickUpCloneDb -i setup.sql
```

### Backup and Restore

```sql
-- Backup
BACKUP DATABASE ClickUpCloneDb TO DISK = 'C:\Backups\ClickUpCloneDb.bak'

-- Restore
RESTORE DATABASE ClickUpCloneDb FROM DISK = 'C:\Backups\ClickUpCloneDb.bak'
```

## Security Considerations

### HTTPS/SSL
1. Generate self-signed certificate for development
2. Install trusted certificate on production
3. Force HTTPS in appsettings.json:
   ```json
   "Kestrel": {
     "Endpoints": {
       "Https": {
         "Url": "https://*:443",
         "Certificate": {
           "Path": "path/to/cert.pfx",
           "Password": "password"
         }
       }
     }
   }
   ```

### Environment Variables
Store sensitive data in environment variables, not appsettings.json:
```bash
export ConnectionStrings__DefaultConnection="Server=..."
export ASPNETCORE_ENVIRONMENT=Production
```

### SQL Server Security
- Use strong passwords for SQL Server
- Restrict database access to application user
- Use Windows Authentication where possible
- Enable encryption at rest

### Application Security
- Enable CORS only for trusted domains
- Set secure cookie options
- Implement rate limiting
- Enable CSRF protection (already in place)
- Sanitize all user inputs

### Firewall Rules
```powershell
# Open ports
New-NetFirewallRule -DisplayName "HTTP" -Direction Inbound -LocalPort 80 -Protocol TCP -Action Allow
New-NetFirewallRule -DisplayName "HTTPS" -Direction Inbound -LocalPort 443 -Protocol TCP -Action Allow
```

## Monitoring and Logging

### Application Insights (Azure)

1. Add Application Insights:
   ```bash
   dotnet add package Microsoft.ApplicationInsights.AspNetCore
   ```

2. Configure in Program.cs:
   ```csharp
   builder.Services.AddApplicationInsightsTelemetry();
   ```

3. Add connection string to appsettings:
   ```json
   "ApplicationInsights": {
     "InstrumentationKey": "your-key"
   }
   ```

### Logging Configuration

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    },
    "File": {
      "LogPath": "/var/log/clickupclone/"
    }
  }
}
```

## Performance Optimization

### Caching
- Implement response caching for static content
- Use distributed caching for sessions
- Cache frequently accessed data

### Database
- Add indexes on foreign keys
- Use connection pooling
- Archive old data

### Web Server
- Enable gzip compression
- Minify CSS/JavaScript
- Use CDN for static assets

## Backup and Recovery

### Automated Backups

```powershell
# Scheduled SQL backup
$schedule = New-JobTrigger -Daily -At 2:00AM
$options = New-ScheduledJobOption -RunElevated
Register-ScheduledJob -Name SQLBackup -Trigger $schedule -ScriptBlock {
    sqlcmd -S . -Q "BACKUP DATABASE ClickUpCloneDb TO DISK = 'C:\Backups\ClickUpCloneDb.bak'"
}
```

### Disaster Recovery

1. Maintain regular backups (daily)
2. Test restore procedures regularly
3. Document recovery steps
4. Keep backups off-site
5. Monitor backup logs

## Troubleshooting

### Application Won't Start
1. Check logs in `Program.cs` output
2. Verify connection string
3. Check database accessibility
4. Review event viewer on Windows

### Database Connection Issues
```bash
# Test connection
sqlcmd -S YOUR_SERVER -d ClickUpCloneDb -U sa -P password
```

### Performance Issues
1. Check database indexes
2. Monitor CPU and memory
3. Review Application Insights
4. Check connection pool size

### SSL/Certificate Issues
1. Verify certificate is valid
2. Check certificate binding
3. Restart IIS/application
4. Review certificate logs

---

For additional help, consult the main README.md and ASP.NET Core documentation.
