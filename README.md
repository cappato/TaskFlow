# TaskFlow

<div align="center">

![TaskFlow Logo](https://via.placeholder.com/200x100/1b6ec2/ffffff?text=TaskFlow)

**A modern task and project management application built with Blazor WebAssembly and ASP.NET Core Web API**

[![.NET](https://img.shields.io/badge/.NET-8.0-purple.svg)](https://dotnet.microsoft.com/download/dotnet/8.0)
[![Blazor](https://img.shields.io/badge/Blazor-WebAssembly-blue.svg)](https://blazor.net/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
[![Build Status](https://github.com/yourusername/TaskFlow/workflows/.NET%20Build%20and%20Test/badge.svg)](https://github.com/yourusername/TaskFlow/actions)

[Quick Start](#getting-started) • [Documentation](#documentation) • [Contributing](#contributing) • [License](#license)

</div>

---

## Features

**Task Management**

- Create, edit, and delete tasks with rich metadata
- Multiple status tracking (Pending, In Progress, Completed, etc.)
- Priority levels (Low, Medium, High, Critical)
- Due date management with overdue alerts

**Project Organization**

- Organize tasks into projects
- Track project progress and completion rates
- Project timeline management

**Dashboard & Analytics**

- Real-time task statistics
- Productivity metrics and insights
- Visual progress tracking

**Modern UI/UX**

- Responsive design with Bootstrap
- Clean and intuitive interface
- Mobile-friendly experience

## Technology Stack

### Frontend (Client)

- **Blazor WebAssembly** - SPA framework in C#
- **.NET 8** - Platform for Blazor and backend
- **Razor Components** - Reusable UI components
- **Bootstrap** - CSS framework for styling
- **HttpClient** - REST API consumption from Blazor

### Backend (Server)

- **ASP.NET Core Web API** - REST API for business logic and data access
- **Entity Framework Core** - ORM for relational database
- **SQL Server** - Database (configurable)

### Shared

- **Shared Models Project** - DTOs, enums, and shared contracts

### Testing

- **XUnit** - Unit testing framework
- **FluentAssertions** - Readable assertions library
- **Moq** - Mocking framework

## Project Structure

```
TaskFlow/
├── src/
│   ├── TaskFlow.Client/             # Blazor WebAssembly (frontend)
│   │   ├── Pages/                   # Razor pages (Home, Tasks, Projects)
│   │   ├── Components/              # Reusable components
│   │   ├── Services/                # API service calls
│   │   └── Program.cs               # Client configuration
│   │
│   ├── TaskFlow.Server/             # ASP.NET Core Web API (backend)
│   │   ├── Controllers/             # API Controllers
│   │   ├── Services/                # Business logic
│   │   ├── Repositories/            # Data access
│   │   ├── Models/                  # Domain entities
│   │   ├── Data/                    # DbContext and migrations
│   │   └── Program.cs               # API configuration
│   │
│   ├── TaskFlow.Shared/             # Shared models
│   │   ├── DTOs/                    # Data Transfer Objects
│   │   ├── Enums/                   # TaskStatus, Priority, etc.
│   │   └── Contracts/               # Shared interfaces
│   │
│   └── TaskFlow.sln                 # Main solution
│
├── tests/
│   └── TaskFlow.Server.Tests/       # Backend unit tests
│
└── README.md
```

## Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) (recommended) or [VS Code](https://code.visualstudio.com/)
- **ASP.NET and web development workload** (for Visual Studio)
- SQLite (included automatically, no setup required)

### Quick Start

1. **Clone the repository**

   ```bash
   git clone https://github.com/cappato/TaskFlow.git
   cd TaskFlow
   ```

2. **Run with automated script**

   ```bash
   # Windows
   ./run-dev.bat

   # Linux/Mac
   chmod +x run-dev.sh && ./run-dev.sh
   ```

3. **Access the application**
   - **Web App**: https://localhost:7001 or http://localhost:5001
   - **API**: https://localhost:7000 or http://localhost:5000
   - **Swagger**: https://localhost:7000/swagger

### Visual Studio Setup (Recommended)

For the best development experience with Visual Studio 2022:

1. **Open the solution**

   - `File` → `Open` → `Project/Solution`
   - Select `TaskFlow.sln`

2. **Configure multiple startup projects**

   - Right-click on the solution → `Properties`
   - `Startup Project` → `Multiple startup projects`
   - Set both projects to `Start`:
     - ✅ **TaskFlow.Server** → `Start`
     - ✅ **TaskFlow.Client** → `Start`
     - ⚪ **TaskFlow.Shared** → `None`

3. **Run the application**

   - Press `F5` (with debugging) or `Ctrl+F5` (without debugging)
   - Both server and client will start automatically
   - Your browser will open to the Blazor client

4. **Verify everything works**
   - Navigate to the **Tareas** section
   - Create a new task to test functionality
   - Check that you see "Admin Cruzado" and "Alejandro Cruzado Project"

### Manual Setup

<details>
<summary>Click to expand manual setup instructions</summary>

1. **Restore packages**

   ```bash
   dotnet restore
   ```

2. **Run the API (Terminal 1)**

   ```bash
   cd src/TaskFlow.Server
   dotnet run
   ```

3. **Run the Client (Terminal 2)**

   ```bash
   cd src/TaskFlow.Client
   dotnet run
   ```

4. **Run tests**
   ```bash
   cd tests/TaskFlow.Server.Tests
   dotnet test
   ```

</details>

### Troubleshooting

#### Common Issues in Visual Studio

**🔴 Port already in use error:**

- Change ports in `launchSettings.json` files
- Or stop other applications using those ports

**🔴 CORS errors:**

- Verify URLs in `src/TaskFlow.Server/Program.cs` match your client URLs
- Default: `"https://localhost:7001", "http://localhost:5001"`

**🔴 Database errors:**

- SQLite database is created automatically on first run
- Location: `src/TaskFlow.Server/TaskFlow.db`
- Delete the file to reset the database

**🔴 Build errors:**

- Clean solution: `Build` → `Clean Solution`
- Rebuild: `Build` → `Rebuild Solution`
- Restore packages: Right-click solution → `Restore NuGet Packages`

**✅ Quick verification:**

- Run tests: `Test` → `Run All Tests` (should pass 7/7)
- Check Swagger: Navigate to `https://localhost:7000/swagger`

## Current Features

### Implemented

- ✅ **Task management** (CRUD operations)
- ✅ **Project management** (CRUD operations)
- ✅ **Task status tracking** (Pendiente, En Progreso, Completada, etc.)
- ✅ **Priority levels** (Baja, Media, Alta, Crítica)
- ✅ **Task-Project relationships**
- ✅ **Spanish UI** (Complete interface translation)
- ✅ **Dashboard with statistics** (completion rates, priority distribution)
- ✅ **Responsive UI** with Bootstrap
- ✅ **REST API** with Swagger documentation
- ✅ **SQLite database** (no setup required)
- ✅ **Unit tests** for business logic (7/7 passing)
- ✅ **Real-time filtering** by status and project

### Planned Features

- User authentication and authorization
- Task assignments to users
- Due date notifications
- Task comments and attachments
- Dashboard with analytics
- Real-time updates with SignalR

## Architecture

The application follows a clean architecture pattern:

- **Presentation Layer**: Blazor WebAssembly client
- **API Layer**: ASP.NET Core Web API controllers
- **Business Logic Layer**: Service classes
- **Data Access Layer**: Repository pattern with Entity Framework Core
- **Domain Layer**: Entity models and business rules

## Configuration

### Database

The application uses **SQLite** for development (no setup required). The database is automatically created on first run.

- **Database file**: `src/TaskFlow.Server/TaskFlow.db`
- **Connection string**: Configured in `src/TaskFlow.Server/appsettings.Development.json`
- **Seed data**: Includes "Admin Cruzado" user and "Alejandro Cruzado Project"

To reset the database, simply delete the `TaskFlow.db` file and restart the application.

### CORS

CORS is configured to allow the Blazor client to communicate with the API. Update the CORS policy in `Program.cs` if needed.

## API Documentation

When running in development mode, Swagger UI is available at:
`https://localhost:7000/swagger`

## Screenshots

<details>
<summary>Click to view screenshots</summary>

### Dashboard

![Dashboard](https://via.placeholder.com/800x400/f8f9fa/333333?text=Dashboard+Screenshot)

### Task Management

![Tasks](https://via.placeholder.com/800x400/f8f9fa/333333?text=Task+Management+Screenshot)

### Project Overview

![Projects](https://via.placeholder.com/800x400/f8f9fa/333333?text=Project+Overview+Screenshot)

</details>

## Roadmap

- [ ] User authentication and authorization
- [ ] Team collaboration features
- [ ] Mobile app (MAUI)
- [ ] Real-time notifications (SignalR)
- [ ] File attachments
- [ ] Task comments and discussions
- [ ] Advanced analytics and reporting
- [ ] Dark mode theme
- [ ] Third-party integrations (GitHub, Slack, etc.)

## Contributing

We welcome contributions! Please see our [Contributing Guide](CONTRIBUTING.md) for details.

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'feat: add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## Support

- **Bug Reports**: [Create an issue](https://github.com/cappato/TaskFlow/issues/new?template=bug_report.md)
- **Feature Requests**: [Create an issue](https://github.com/cappato/TaskFlow/issues/new?template=feature_request.md)
- **Discussions**: [GitHub Discussions](https://github.com/cappato/TaskFlow/discussions)

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Acknowledgments

- Built with [Blazor WebAssembly](https://blazor.net/)
- Powered by [ASP.NET Core](https://docs.microsoft.com/aspnet/core/)
- UI components from [Bootstrap](https://getbootstrap.com/)
- Icons from [Open Iconic](https://useiconic.com/open)

---

<div align="center">

**If you found this project helpful, please give it a star!**

Made with care by the TaskFlow team

</div>

