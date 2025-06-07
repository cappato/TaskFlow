# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Planned
- User authentication and authorization
- Real-time notifications with SignalR
- File attachments for tasks
- Task comments and discussions
- Advanced analytics and reporting
- Mobile app with .NET MAUI

## [1.0.0] - 2024-01-XX

### Added
- 🎯 **Task Management**
  - Create, read, update, delete tasks
  - Task status tracking (Pending, In Progress, Completed, Cancelled, On Hold)
  - Priority levels (Low, Medium, High, Critical)
  - Due date management with overdue alerts
  - Task descriptions and metadata

- 📁 **Project Management**
  - Create and manage projects
  - Assign tasks to projects
  - Project progress tracking
  - Project timeline management
  - Active/inactive project status

- 📊 **Dashboard & Analytics**
  - Real-time task statistics
  - Task completion rates
  - Priority distribution charts
  - Overdue task alerts
  - Project progress overview

- 🎨 **User Interface**
  - Responsive Blazor WebAssembly frontend
  - Bootstrap-based UI components
  - Mobile-friendly design
  - Clean and intuitive navigation
  - Task and project cards
  - Modal forms for CRUD operations

- 🔧 **Backend API**
  - RESTful API with ASP.NET Core
  - Entity Framework Core with SQL Server
  - Repository pattern implementation
  - Service layer architecture
  - Swagger/OpenAPI documentation
  - CORS configuration for frontend

- 🧪 **Testing & Quality**
  - Unit tests with XUnit
  - FluentAssertions for readable tests
  - Moq for mocking dependencies
  - GitHub Actions CI/CD pipeline
  - Code coverage reporting

- 🛠️ **Development Tools**
  - Docker and Docker Compose support
  - VSCode configuration and debugging
  - Development scripts (run-dev.bat/sh)
  - EditorConfig for code consistency
  - Comprehensive documentation

- 📦 **Project Structure**
  - Clean architecture with separated concerns
  - Shared models between client and server
  - Proper dependency injection
  - Configuration management
  - Environment-specific settings

### Technical Details
- **Frontend**: Blazor WebAssembly with .NET 8
- **Backend**: ASP.NET Core Web API with .NET 8
- **Database**: SQL Server with Entity Framework Core
- **Testing**: XUnit, FluentAssertions, Moq
- **CI/CD**: GitHub Actions
- **Containerization**: Docker and Docker Compose

### Dependencies
- Microsoft.AspNetCore.Components.WebAssembly (8.0.0)
- Microsoft.EntityFrameworkCore.SqlServer (8.0.0)
- Microsoft.EntityFrameworkCore.Tools (8.0.0)
- Swashbuckle.AspNetCore (6.4.0)
- XUnit (2.6.1)
- FluentAssertions (6.12.0)
- Moq (4.20.69)

## [0.1.0] - 2024-01-XX

### Added
- Initial project setup
- Basic project structure
- Development environment configuration

---

## Types of Changes
- `Added` for new features
- `Changed` for changes in existing functionality
- `Deprecated` for soon-to-be removed features
- `Removed` for now removed features
- `Fixed` for any bug fixes
- `Security` for vulnerability fixes
