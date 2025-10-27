# TaskManager - Collaborative Task & Project Management Tool

## Project Title
**TaskManager** - A Collaborative Task and Project Management System

## Description
TaskManager is a comprehensive web-based application built with ASP.NET Core 9.0 that enables teams to collaborate on projects and tasks efficiently. It provides a modern, responsive interface for managing projects, assigning tasks, tracking progress, and collaborating with team members.

### Key Features:
- **Project Management**: Create and manage multiple projects
- **Task Management**: Create, assign, and track tasks with status updates
- **Team Collaboration**: Invite team members and manage project access
- **Role-based Access Control**: Different permissions for Owners, Admins, and Members
- **Real-time Updates**: Dynamic UI with AJAX for seamless user experience
- **Email Invitations**: Send invitations to team members via email
- **Responsive Design**: Works perfectly on desktop and mobile devices
- **SQL Server Integration**: Robust data storage with Entity Framework Core

### Technology Stack:
- **Backend**: ASP.NET Core 9.0 MVC
- **Frontend**: Bootstrap 5, JavaScript, jQuery
- **Database**: SQL Server with Entity Framework Core
- **Authentication**: ASP.NET Core Identity
- **Email**: SMTP integration for notifications

## Installation Steps

### Prerequisites:
1. **.NET 9.0 SDK** - [Download here](https://dotnet.microsoft.com/download/dotnet/9.0)
2. **SQL Server Express** - [Download here](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)
3. **Visual Studio Code** or **Visual Studio 2022** (Recommended)
4. **Git** for version control

### Step-by-Step Installation:

1. **Clone or Download the Project**
   ```bash
   # If using Git
   git clone <repository-url>
   cd TaskManagementSystem/TaskManager
   
   # Or simply extract the project files to a folder
   ```

2. **Configure Database Connection**
   - Open `appsettings.json`
   - Update the connection string if your SQL Server instance is different:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=.\\SQLEXPRESS;Database=TaskManager;Trusted_Connection=true;MultipleActiveResultSets=true;TrustServerCertificate=true"
   }
   ```

3. **Configure Email Settings (Optional - for email invitations)**
   - Update SMTP settings in `appsettings.json` if you want email functionality:
   ```json
   "SmtpSettings": {
     "Server": "smtp.gmail.com",
     "Port": 587,
     "Username": "your-email@gmail.com",
     "Password": "your-app-password",
     "FromEmail": "your-email@gmail.com",
     "EnableSsl": true
   }
   ```
   *Note: For Gmail, you need to enable 2-factor authentication and generate an app password*

4. **Install Dependencies**
   ```bash
   dotnet restore
   ```

5. **Database Setup**
   ```bash
   dotnet ef database update
   ```

## How to Run the Project

### Development Mode:
1. **Build the Application**
   ```bash
   dotnet build
   ```

2. **Run the Application**
   ```bash
   dotnet run
   ```

3. **Access the Application**
   - Open your web browser
   - Navigate to: `https://localhost:7000` or `http://localhost:5000`
   - The application will start automatically

### First-Time Setup:
1. **Register a New Account**
   - Click "Register" in the navigation bar
   - Fill in your details (First Name, Last Name, Email, Password)
   - Click "Register" to create your account

2. **Create Your First Project**
   - After logging in, you'll see the projects dashboard
   - Click "New Project" button
   - Enter project name and description
   - Click "Create Project"

3. **Add Team Members**
   - Go to project details by clicking "View" on your project
   - Use "Add Existing User" to search and add registered users
   - Or use "Invite by Email" to send invitations to new users

4. **Create Tasks**
   - From project details, click "New Task" or "Create New Task"
   - Fill in task details (title, description, due date, priority, assignee)
   - Click "Create Task"

### Default Users (Automatically Created):
The application automatically creates these users for testing:
- **Admin User**: `admin@taskmanager.com` / `Admin123!`
- **Sample User**: `user@taskmanager.com` / `User123!`

## Project Structure
```
TaskManager/
├── Controllers/          # MVC Controllers (Projects, Tasks, Invitations)
├── Models/              # Data Models (Project, Task, User, etc.)
├── Views/               # Razor Views for UI
├── ViewModels/          # View Models for data transfer
├── Services/            # Business Logic Services
├── Data/                # Database Context
├── wwwroot/             # Static files (CSS, JS)
└── Program.cs           # Application configuration
```

## Key Functionality

### For Project Owners/Admins:
- ✅ Create and delete projects
- ✅ Add/remove team members
- ✅ Send email invitations
- ✅ Manage all project tasks
- ✅ View team statistics

### For Team Members:
- ✅ View assigned projects
- ✅ Create and manage tasks
- ✅ Update task status (To Do → In Progress → Review → Done)
- ✅ Add comments to tasks
- ✅ Collaborate with team

### Task Management Features:
- **Status Tracking**: To Do, In Progress, Review, Done
- **Priority Levels**: Low, Medium, High, Critical
- **Assignment**: Assign tasks to specific team members
- **Due Dates**: Set and track deadlines
- **Comments**: Collaborative discussions on tasks
- **Filters**: Filter tasks by status and priority

## User Roles & Permissions

### Project Owner:
- Full control over the project
- Can delete project
- Can add/remove any member
- Can change member roles

### Project Admin:
- Can manage tasks and members
- Cannot delete project
- Can add/remove regular members

### Project Member:
- Can view project and tasks
- Can create and update tasks
- Can only be removed by admins/owner

## API Endpoints
- `GET /Projects` - List user projects
- `POST /Projects/Create` - Create new project
- `GET /Projects/Details/{id}` - Project details with team members
- `POST /Projects/Delete/{id}` - Delete project
- `POST /Projects/SendInvitation` - Send member invitation
- `GET /Projects/SearchUsers` - Search users by name/email
- `GET /Tasks/Index/{projectId}` - Project tasks
- `POST /Tasks/Create` - Create new task
- `POST /Tasks/UpdateStatus` - Update task status

## Troubleshooting

### Common Issues and Solutions:

1. **Database Connection Error**
   ```bash
   # Ensure SQL Server is running
   # Verify connection string in appsettings.json
   dotnet ef database update
   ```

2. **Build Errors**
   ```bash
   dotnet clean
   dotnet restore
   dotnet build
   ```

3. **Migration Issues**
   ```bash
   # Remove existing migrations and recreate
   dotnet ef migrations add InitialCreate
   dotnet ef database update
   ```

4. **Email Not Sending**
   - Check SMTP settings in `appsettings.json`
   - For Gmail: Enable 2FA and use app password
   - Check application logs for detailed errors

5. **Port Already in Use**
   ```bash
   # Use different port
   dotnet run --urls="http://localhost:5001"
   ```

### Development Tips:
- The application logs email content to console in development mode
- Use the default test users for quick testing
- All form submissions include validation and error handling
- The UI is fully responsive for mobile devices

## Support
For technical issues:
1. Check the application console logs
2. Verify database connection
3. Ensure all dependencies are installed
4. Check the browser console for JavaScript errors

## License
This project is developed for educational purposes as part of a 3rd-year college project demonstrating ASP.NET Core, Entity Framework, and modern web development practices.

---

**Start managing your projects efficiently!** 🚀

*For any questions or support, please refer to the application logs or contact the development team.*
