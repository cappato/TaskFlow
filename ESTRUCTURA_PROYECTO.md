# 📁 Estructura del Proyecto TaskFlow PIM

## 🎯 Descripción General

Sistema PIM (Product Information Manager) para gestión de artículos deportivos con atributos dinámicos, desarrollado en .NET 8 con Blazor WebAssembly.

## ✅ **ARQUITECTURA IMPLEMENTADA: Clean Architecture + DDD**

### 📋 **Estructura Actual (Clean Architecture)**

**🟢 IMPLEMENTADO (Clean Architecture):**

```
src/
├── TaskFlow.Domain/          # ✅ Entidades + Interfaces + Enums
├── TaskFlow.Server/          # ✅ Controllers + Repositorios + Servicios + DbContext
├── TaskFlow.Client/          # ✅ Frontend Blazor WebAssembly
└── TaskFlow.Shared/          # ✅ DTOs de comunicación API
```

### 🎯 **Beneficios Logrados:**

- ✅ **Separación de Responsabilidades**: Domain layer independiente
- ✅ **Testabilidad**: Entidades puras sin dependencias externas
- ✅ **Mantenibilidad**: Código organizado por capas
- ✅ **Reutilización**: Domain entities reutilizables
- ✅ **SOLID**: Principios aplicados correctamente
- ✅ **DDD**: Domain-Driven Design implementado

## �📂 Estructura de Carpetas

```
TaskFlow/
├── 📄 README.md                           # Documentación principal
├── 📄 README_PIM.md                       # Documentación específica del PIM
├── 📄 TaskFlow.sln                        # Solución de Visual Studio
├── 📄 CHANGELOG.md                        # Historial de cambios
├── 📄 DEPLOYMENT.md                       # Guía de despliegue
├── 📄 AZURE_DEPLOYMENT.md                 # Despliegue en Azure
├── 📄 QUICK-START.md                      # Guía de inicio rápido
├── 📄 CONTRIBUTING.md                     # Guía de contribución
├── 📄 SECURITY.md                         # Políticas de seguridad
├── 📄 LICENSE                             # Licencia del proyecto
├── 📄 REGLAS_DEL_REPOSITORIO.md          # Reglas del repositorio
├── 📄 docker-compose.yml                 # Configuración Docker
├── 📄 run-dev.bat                         # Script de desarrollo (Windows)
├── 📄 run-dev.sh                          # Script de desarrollo (Linux/Mac)
├── 📄 run-tests.ps1                       # Script de tests (PowerShell)
├── 📄 deploy.bat                          # Script de despliegue
├── 📄 setup-github-rules.sh               # Configuración GitHub
│
├── 📁 docs/                               # Documentación adicional
│   ├── 📄 AUDITORIA_PROYECTO_PIM.md      # Auditoría del proyecto
│   └── 📄 respuesta a auditoria.md       # Respuesta a auditoría
│
├── 📁 src/                                # Código fuente
│   ├── 📁 TaskFlow.Domain/                # 🆕 Capa de Dominio (Clean Architecture)
│   │   ├── 📄 TaskFlow.Domain.csproj     # Archivo de proyecto
│   │   ├── 📁 Entities/                   # Entidades de dominio
│   │   │   ├── 📄 Article.cs             # Entidad artículo
│   │   │   ├── 📄 Category.cs            # Entidad categoría
│   │   │   ├── 📄 CustomAttribute.cs     # Entidad atributo personalizado
│   │   │   ├── 📄 ArticleAttributeValue.cs # Entidad valor atributo
│   │   │   ├── 📄 ArticleVariant.cs      # Entidad variante artículo
│   │   │   └── 📄 User.cs                # Entidad usuario
│   │   ├── 📁 Enums/                      # Enumeraciones de dominio
│   │   │   ├── 📄 ArticleType.cs         # Tipos de artículos
│   │   │   └── 📄 AttributeType.cs       # Tipos de atributos
│   │   └── 📁 Interfaces/                 # Interfaces de repositorios
│   │       ├── 📄 IArticleRepository.cs  # Interfaz repositorio artículos
│   │       ├── 📄 ICategoryRepository.cs # Interfaz repositorio categorías
│   │       ├── 📄 ICustomAttributeRepository.cs # Interfaz repositorio atributos
│   │       └── 📄 IArticleAttributeValueRepository.cs # Interfaz repositorio valores
│   │
│   ├── 📁 TaskFlow.Shared/                # Proyecto compartido (DTOs)
│   │   ├── 📄 TaskFlow.Shared.csproj     # Archivo de proyecto
│   │   └── 📁 DTOs/                       # Data Transfer Objects
│   │       ├── 📄 ArticleDto.cs          # DTO de artículos
│   │       ├── 📄 CategoryDto.cs         # DTO de categorías
│   │       ├── 📄 CustomAttributeDto.cs  # DTO de atributos
│   │       ├── 📄 CreateArticleDto.cs    # DTO creación artículos
│   │       ├── 📄 UpdateArticleDto.cs    # DTO actualización artículos
│   │       ├── 📄 CreateCustomAttributeDto.cs # DTO creación atributos
│   │       ├── 📄 UpdateCustomAttributeDto.cs # DTO actualización atributos
│   │       └── 📄 ArticleAttributeValueDto.cs # DTO valores atributos
│   │
│   ├── 📁 TaskFlow.Server/                # Backend API (Infrastructure Layer)
│   │   ├── 📄 TaskFlow.Server.csproj     # Archivo de proyecto (referencia Domain)
│   │   ├── 📄 Program.cs                 # Punto de entrada y configuración DI
│   │   ├── 📄 appsettings.json           # Configuración general
│   │   ├── 📄 appsettings.Development.json # Configuración desarrollo
│   │   ├── 📄 appsettings.Production.json # Configuración producción
│   │   ├── 📄 TaskFlow.db                # Base de datos SQLite
│   │   ├── 📄 Dockerfile                 # Configuración Docker
│   │   ├── 📄 web.config                 # Configuración IIS
│   │   │
│   │   ├── 📁 Controllers/               # Controladores API
│   │   │   ├── 📄 ArticlesController.cs  # API de artículos (usa Domain entities)
│   │   │   ├── 📄 CategoriesController.cs # API de categorías
│   │   │   └── 📄 CustomAttributesController.cs # API de atributos
│   │   │
│   │   ├── 📁 Data/                      # Contexto de datos
│   │   │   └── 📄 TaskFlowDbContext.cs   # Contexto EF (usa Domain entities)
│   │   │
│   │   ├── 📁 Repositories/              # Implementaciones de repositorios
│   │   │   ├── 📄 ArticleRepository.cs   # Implementa IArticleRepository (Domain)
│   │   │   ├── 📄 CategoryRepository.cs  # Implementa ICategoryRepository (Domain)
│   │   │   ├── 📄 CustomAttributeRepository.cs # Implementa ICustomAttributeRepository
│   │   │   └── 📄 ArticleAttributeValueRepository.cs # Implementa IArticleAttributeValueRepository
│   │   │
│   │   ├── 📁 Services/                  # Servicios de aplicación
│   │   │   ├── 📄 IArticleService.cs     # Interfaz servicio artículos
│   │   │   ├── 📄 ArticleService.cs      # Servicio artículos (usa Domain entities)
│   │   │   ├── 📄 ICategoryService.cs    # Interfaz servicio categorías
│   │   │   ├── 📄 CategoryService.cs     # Servicio categorías
│   │   │   ├── 📄 ICustomAttributeService.cs # Interfaz servicio atributos
│   │   │   └── 📄 CustomAttributeService.cs # Servicio atributos
│   │   │
│   │   └── 📁 Properties/                # Propiedades del proyecto
│   │       └── 📄 launchSettings.json    # Configuración de lanzamiento
│   │
│   └── 📁 TaskFlow.Client/               # Frontend Blazor WebAssembly
│       ├── 📄 TaskFlow.Client.csproj     # Archivo de proyecto
│       ├── 📄 Program.cs                 # Punto de entrada
│       ├── 📄 App.razor                  # Componente raíz
│       ├── 📄 _Imports.razor             # Importaciones globales
│       ├── 📄 Dockerfile                 # Configuración Docker
│       ├── 📄 nginx.conf                 # Configuración Nginx
│       │
│       ├── 📁 Pages/                     # Páginas de la aplicación
│       │   ├── 📄 Home.razor             # Página de inicio
│       │   ├── 📄 Articles.razor         # Gestión de artículos
│       │   └── 📄 AttributeAdmin.razor   # Administración de atributos
│       │
│       ├── 📁 Components/                # Componentes reutilizables
│       │   ├── 📄 DynamicForm.razor      # Formulario dinámico
│       │   ├── 📄 DashboardStats.razor   # Estadísticas dashboard
│       │   ├── 📄 ModernDashboardStats.razor # Stats modernas
│       │   ├── 📄 TaskCard.razor         # Tarjeta de tarea
│       │   ├── 📄 ModernTaskCard.razor   # Tarjeta moderna
│       │   ├── 📄 MainLayout.razor       # Layout principal
│       │   ├── 📄 MainLayout.razor.css   # Estilos layout
│       │   ├── 📄 NavMenu.razor          # Menú de navegación
│       │   ├── 📄 NavMenu.razor.css      # Estilos menú
│       │   │
│       │   ├── 📁 Layout/                # Layouts
│       │   │   └── 📄 ModernLayout.razor # Layout moderno
│       │   │
│       │   └── 📁 UI/                    # Componentes UI
│       │       ├── 📄 Button.razor       # Componente botón
│       │       ├── 📄 ButtonEnums.cs     # Enums de botón
│       │       └── 📄 Card.razor         # Componente tarjeta
│       │
│       ├── 📁 Services/                  # Servicios del cliente
│       │   ├── 📄 IArticleApiService.cs  # Interfaz API artículos
│       │   ├── 📄 ArticleApiService.cs   # Servicio API artículos
│       │   ├── 📄 ICustomAttributeApiService.cs # Interfaz API atributos
│       │   └── 📄 CustomAttributeApiService.cs # Servicio API atributos
│       │
│       ├── 📁 Properties/                # Propiedades del proyecto
│       │   └── 📄 launchSettings.json    # Configuración de lanzamiento
│       │
│       └── 📁 wwwroot/                   # Archivos estáticos
│           ├── 📄 index.html             # Página principal
│           ├── 📄 favicon.ico            # Icono del sitio
│           ├── 📄 app.css                # Estilos globales
│           └── 📁 css/                   # Hojas de estilo
│
└── 📁 tests/                             # Pruebas unitarias
    └── 📁 TaskFlow.Server.Tests/         # Tests del servidor
        ├── 📄 TaskFlow.Server.Tests.csproj # Archivo de proyecto
        │
        ├── 📁 Controllers/               # Tests de controladores
        │   └── 📄 ArticlesControllerTests.cs # Tests API artículos
        │
        ├── 📁 Services/                  # Tests de servicios
        │   ├── 📄 ArticleServiceTests.cs # Tests servicio artículos
        │   └── 📄 CustomAttributeServiceTests.cs # Tests servicio atributos
        │
        ├── 📁 Repositories/              # Tests de repositorios
        │   ├── 📄 ArticleRepositoryTests.cs # Tests repositorio artículos
        │   └── 📄 CustomAttributeRepositoryTests.cs # Tests repositorio atributos
        │
        └── 📁 Integration/               # Tests de integración
            └── 📄 PIMIntegrationTests.cs # Tests end-to-end
```

## 🎯 Descripción de Componentes Principales

### 🏛️ Domain Layer (TaskFlow.Domain)

- **Entidades de dominio** puras sin dependencias externas
- **Interfaces de repositorios** que definen contratos
- **Enums de dominio** centralizados
- **Lógica de negocio** encapsulada en entidades
- **Independiente del framework** y base de datos

### 🔧 Infrastructure Layer (TaskFlow.Server)

- **API REST** con controladores para artículos, categorías y atributos
- **Entity Framework Core** con SQLite para persistencia
- **Implementaciones de repositorios** que usan Domain interfaces
- **Servicios de aplicación** con validaciones y lógica
- **CORS configurado** para comunicación con frontend
- **Inyección de dependencias** configurada

### 🎨 Presentation Layer (TaskFlow.Client)

- **Blazor WebAssembly** con diseño moderno
- **Tailwind CSS** para estilos responsivos
- **Componentes reutilizables** y layouts modernos
- **Servicios API** para comunicación con backend
- **Formularios dinámicos** para atributos personalizados
- **Referencias a Domain** para usar enums

### 📦 Communication Layer (TaskFlow.Shared)

- **DTOs** para transferencia de datos entre capas
- **Contratos de API** bien definidos
- **Referencias a Domain** para consistencia de tipos

### 🧪 Tests (TaskFlow.Server.Tests)

- **67 tests unitarios** con 100% de éxito
- **Tests de integración** end-to-end
- **Cobertura completa** de controladores, servicios y repositorios
- **Validaciones** de funcionalidades críticas

## 🚀 Tecnologías Utilizadas

- **.NET 8** - Framework principal
- **Blazor WebAssembly** - Frontend SPA
- **Entity Framework Core** - ORM
- **SQLite** - Base de datos
- **xUnit** - Framework de testing
- **Tailwind CSS** - Framework de estilos
- **Docker** - Containerización
- **Azure** - Plataforma de despliegue

## 📊 Métricas del Proyecto

- **📁 Proyectos**: 4 (Domain, Server, Client, Shared)
- **📁 Carpetas**: 30+
- **📄 Archivos de código**: 60+
- **🧪 Tests**: 67 (100% éxito)
- **🎯 Funcionalidades**: Sistema PIM completo
- **📱 Responsive**: Diseño adaptativo
- **🔒 Seguro**: Validaciones y CORS configurado
- **🏗️ Arquitectura**: Clean Architecture + DDD implementado
- **🔄 Separación**: Domain layer independiente

