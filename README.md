# PimFlow

<div align="center">

![PimFlow Logo](https://via.placeholder.com/200x100/1b6ec2/ffffff?text=PimFlow)

**A modern Product Information Management (PIM) system built with Blazor WebAssembly and ASP.NET Core Web API**

[![.NET](https://img.shields.io/badge/.NET-8.0-purple.svg)](https://dotnet.microsoft.com/download/dotnet/8.0)
[![Blazor](https://img.shields.io/badge/Blazor-WebAssembly-blue.svg)](https://blazor.net/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
[![Build Status](https://github.com/cappato/PimFlow/workflows/.NET%20Build%20and%20Test/badge.svg)](https://github.com/cappato/PimFlow/actions)

[Quick Start](#getting-started) • [Documentation](#documentation) • [Contributing](#contributing) • [License](#license)

</div>

---

## Features

**Product Information Management**

- Create, edit, and delete articles with rich metadata
- Multiple article types (Footwear, Clothing, Accessories)
- Custom attributes management (Text, Number, Select, Color, etc.)
- Dynamic attribute values per article

**Category Organization**

- Organize articles into categories
- Hierarchical category structure
- Category-based filtering and search

**Custom Attributes System**

- Define custom attributes for products
- Multiple data types support
- Flexible attribute assignment
- Dynamic form generation

**Dashboard & Analytics**

- Real-time product statistics
- Inventory insights and metrics
- Visual data representation

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
PimFlow/
├── src/
│   ├── PimFlow.Domain/              # 🏛️ Domain Layer (Clean Architecture)
│   │   ├── Entities/                # Domain entities (Article, Category, etc.)
│   │   ├── Enums/                   # Domain enums (ArticleType, AttributeType)
│   │   └── Interfaces/              # Repository interfaces
│   │
│   ├── PimFlow.Server/              # 🔧 Infrastructure Layer (API + Data)
│   │   ├── Controllers/             # API Controllers
│   │   ├── Services/                # Application services
│   │   ├── Repositories/            # Repository implementations
│   │   ├── Data/                    # DbContext and migrations
│   │   └── Program.cs               # API configuration
│   │
│   ├── PimFlow.Client/              # 🎨 Presentation Layer (Blazor WebAssembly)
│   │   ├── Pages/                   # Razor pages (Home, Articles, Attributes)
│   │   ├── Components/              # Reusable UI components
│   │   ├── Services/                # API service calls
│   │   └── Program.cs               # Client configuration
│   │
│   ├── PimFlow.Shared/              # 📦 Communication Layer
│   │   └── DTOs/                    # Data Transfer Objects
│   │
│   └── PimFlow.sln                  # Main solution
│
├── tests/
│   └── PimFlow.Server.Tests/        # Backend unit tests (67 tests)
│
├── gitflow.sh                       # Git Flow commands (Linux/Mac)
├── gitflow.bat                      # Git Flow commands (Windows)
├── GITFLOW.md                       # Git Flow documentation
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
   git clone https://github.com/cappato/PimFlow.git
   cd PimFlow
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
   - Select `PimFlow.sln`

2. **Configure multiple startup projects**

   - Right-click on the solution → `Properties`
   - `Startup Project` → `Multiple startup projects`
   - Set both projects to `Start`:
     - ✅ **PimFlow.Server** → `Start`
     - ✅ **PimFlow.Client** → `Start`
     - ⚪ **PimFlow.Shared** → `None`

3. **Run the application**

   - Press `F5` (with debugging) or `Ctrl+F5` (without debugging)
   - Both server and client will start automatically
   - Your browser will open to the Blazor client

4. **Verify everything works**
   - Navigate to the **Artículos** section
   - Create a new article to test functionality
   - Check that you see custom attributes and categories

### Manual Setup

<details>
<summary>Click to expand manual setup instructions</summary>

1. **Restore packages**

   ```bash
   dotnet restore
   ```

2. **Run the API (Terminal 1)**

   ```bash
   cd src/PimFlow.Server
   dotnet run
   ```

3. **Run the Client (Terminal 2)**

   ```bash
   cd src/PimFlow.Client
   dotnet run
   ```

4. **Run tests**
   ```bash
   cd tests/PimFlow.Server.Tests
   dotnet test
   ```

</details>

### Troubleshooting

#### Common Issues in Visual Studio

**🔴 Port already in use error:**

- Change ports in `launchSettings.json` files
- Or stop other applications using those ports

**🔴 CORS errors:**

- Verify URLs in `src/PimFlow.Server/Program.cs` match your client URLs
- Default: `"https://localhost:7001", "http://localhost:5001"`

**🔴 Database errors:**

- SQLite database is created automatically on first run
- Location: `src/PimFlow.Server/PimFlow.db`
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

- ✅ **Article management** (CRUD operations)
- ✅ **Category management** (CRUD operations)
- ✅ **Custom attributes system** (Text, Number, Select, Color, Boolean, etc.)
- ✅ **Article types** (Footwear, Clothing, Accessories)
- ✅ **Dynamic attribute values** per article
- ✅ **Spanish UI** (Complete interface translation)
- ✅ **Dashboard with statistics** (article counts, attribute distribution)
- ✅ **Responsive UI** with Tailwind CSS
- ✅ **REST API** with Swagger documentation
- ✅ **SQLite database** with Entity Framework migrations
- ✅ **Unit tests** for business logic (67/67 passing)
- ✅ **Clean Architecture** with Domain-Driven Design
- ✅ **Git Flow** workflow implementation

### Planned Features

- User authentication and authorization
- Multi-tenant support for different companies
- Product variants and SKU management
- Inventory tracking and stock levels
- Product images and media management
- Advanced search and filtering
- Data import/export functionality
- Real-time updates with SignalR
- Blazor WebAssembly Hosted architecture

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

- **Database file**: `src/PimFlow.Server/PimFlow.db`
- **Connection string**: Configured in `src/PimFlow.Server/appsettings.Development.json`
- **Seed data**: Includes sample articles, categories, and custom attributes

To reset the database, simply delete the `PimFlow.db` file and restart the application.

### CORS

CORS is configured to allow the Blazor client to communicate with the API. Update the CORS policy in `Program.cs` if needed.

## API Documentation

When running in development mode, Swagger UI is available at:
`https://localhost:7000/swagger`

## Screenshots

<details>
<summary>Click to view screenshots</summary>

### Dashboard

![Dashboard](https://via.placeholder.com/800x400/f8f9fa/333333?text=PIM+Dashboard+Screenshot)

### Article Management

![Articles](https://via.placeholder.com/800x400/f8f9fa/333333?text=Article+Management+Screenshot)

### Custom Attributes

![Attributes](https://via.placeholder.com/800x400/f8f9fa/333333?text=Custom+Attributes+Screenshot)

</details>

## Roadmap

- [ ] Blazor WebAssembly Hosted architecture
- [ ] User authentication and authorization
- [ ] Multi-tenant support
- [ ] Product variants and SKU management
- [ ] Inventory tracking
- [ ] Product images and media management
- [ ] Advanced search and filtering
- [ ] Data import/export (CSV, Excel)
- [ ] Real-time notifications (SignalR)
- [ ] Mobile app (MAUI)
- [ ] Advanced analytics and reporting
- [ ] Dark mode theme
- [ ] Third-party integrations (ERP systems, e-commerce platforms)

## Contributing

We welcome contributions! Please see our [Contributing Guide](CONTRIBUTING.md) for details.

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'feat: add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## Support

- **Bug Reports**: [Create an issue](https://github.com/cappato/PimFlow/issues/new?template=bug_report.md)
- **Feature Requests**: [Create an issue](https://github.com/cappato/PimFlow/issues/new?template=feature_request.md)
- **Discussions**: [GitHub Discussions](https://github.com/cappato/PimFlow/discussions)

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

Made with care by the PimFlow team

</div>

