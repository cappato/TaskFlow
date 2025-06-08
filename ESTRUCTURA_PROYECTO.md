# 📁 Estructura del Proyecto TaskFlow PIM

## 🎯 Descripción General
Sistema PIM (Product Information Manager) para gestión de artículos deportivos con atributos dinámicos, desarrollado en .NET 8 con Blazor WebAssembly.

## 📂 Estructura de Carpetas

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
│   ├── 📁 TaskFlow.Shared/                # Proyecto compartido
│   │   ├── 📄 TaskFlow.Shared.csproj     # Archivo de proyecto
│   │   ├── 📁 DTOs/                       # Data Transfer Objects
│   │   │   ├── 📄 ArticleDto.cs          # DTO de artículos
│   │   │   ├── 📄 CategoryDto.cs         # DTO de categorías
│   │   │   ├── 📄 CustomAttributeDto.cs  # DTO de atributos
│   │   │   └── 📄 ArticleAttributeValueDto.cs # DTO valores atributos
│   │   └── 📁 Enums/                      # Enumeraciones
│   │       ├── 📄 ArticleType.cs         # Tipos de artículos
│   │       └── 📄 AttributeDataType.cs   # Tipos de datos atributos
│   │
│   ├── 📁 TaskFlow.Server/                # Backend API
│   │   ├── 📄 TaskFlow.Server.csproj     # Archivo de proyecto
│   │   ├── 📄 Program.cs                 # Punto de entrada
│   │   ├── 📄 appsettings.json           # Configuración general
│   │   ├── 📄 appsettings.Development.json # Configuración desarrollo
│   │   ├── 📄 appsettings.Production.json # Configuración producción
│   │   ├── 📄 TaskFlow.db                # Base de datos SQLite
│   │   ├── 📄 Dockerfile                 # Configuración Docker
│   │   ├── 📄 web.config                 # Configuración IIS
│   │   │
│   │   ├── 📁 Controllers/               # Controladores API
│   │   │   ├── 📄 ArticlesController.cs  # API de artículos
│   │   │   ├── 📄 CategoriesController.cs # API de categorías
│   │   │   └── 📄 CustomAttributesController.cs # API de atributos
│   │   │
│   │   ├── 📁 Data/                      # Contexto de datos
│   │   │   ├── 📄 TaskFlowDbContext.cs   # Contexto Entity Framework
│   │   │   └── 📄 DbInitializer.cs       # Inicializador de datos
│   │   │
│   │   ├── 📁 Models/                    # Modelos de datos
│   │   │   ├── 📄 Article.cs             # Modelo de artículo
│   │   │   ├── 📄 Category.cs            # Modelo de categoría
│   │   │   ├── 📄 CustomAttribute.cs     # Modelo de atributo
│   │   │   └── 📄 ArticleAttributeValue.cs # Modelo valor atributo
│   │   │
│   │   ├── 📁 Repositories/              # Repositorios de datos
│   │   │   ├── 📄 IArticleRepository.cs  # Interfaz artículos
│   │   │   ├── 📄 ArticleRepository.cs   # Implementación artículos
│   │   │   ├── 📄 ICategoryRepository.cs # Interfaz categorías
│   │   │   ├── 📄 CategoryRepository.cs  # Implementación categorías
│   │   │   ├── 📄 ICustomAttributeRepository.cs # Interfaz atributos
│   │   │   ├── 📄 CustomAttributeRepository.cs # Implementación atributos
│   │   │   ├── 📄 IArticleAttributeValueRepository.cs # Interfaz valores
│   │   │   └── 📄 ArticleAttributeValueRepository.cs # Implementación valores
│   │   │
│   │   ├── 📁 Services/                  # Servicios de negocio
│   │   │   ├── 📄 IArticleService.cs     # Interfaz servicio artículos
│   │   │   ├── 📄 ArticleService.cs      # Servicio artículos
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

### 🔧 Backend (TaskFlow.Server)
- **API REST** con controladores para artículos, categorías y atributos
- **Entity Framework Core** con SQLite para persistencia
- **Patrón Repository** para acceso a datos
- **Servicios de negocio** con validaciones y lógica
- **CORS configurado** para comunicación con frontend

### 🎨 Frontend (TaskFlow.Client)
- **Blazor WebAssembly** con diseño moderno
- **Tailwind CSS** para estilos responsivos
- **Componentes reutilizables** y layouts modernos
- **Servicios API** para comunicación con backend
- **Formularios dinámicos** para atributos personalizados

### 📦 Shared (TaskFlow.Shared)
- **DTOs** para transferencia de datos
- **Enums** compartidos entre frontend y backend
- **Modelos** de dominio comunes

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

- **📁 Carpetas**: 25+
- **📄 Archivos de código**: 50+
- **🧪 Tests**: 67 (100% éxito)
- **🎯 Funcionalidades**: Sistema PIM completo
- **📱 Responsive**: Diseño adaptativo
- **🔒 Seguro**: Validaciones y CORS configurado
