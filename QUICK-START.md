# 🚀 TaskFlow - Guía de Inicio Rápido

## ✅ Verificar Requisitos

```bash
# Verificar .NET 8
dotnet --version
# Debe mostrar: 8.0.x
```

Si no tienes .NET 8:
- **Windows**: Descargar desde https://dotnet.microsoft.com/download/dotnet/8.0
- **Linux**: `sudo apt install dotnet-sdk-8.0`
- **Mac**: `brew install dotnet`

## 🏃‍♂️ Ejecución Rápida

### Opción 1: Script Automático
```bash
# Windows
./run-dev.bat

# Linux/Mac
chmod +x run-dev.sh && ./run-dev.sh
```

### Opción 2: Manual
```bash
# 1. Restaurar dependencias
dotnet restore

# 2. Ejecutar API (Terminal 1)
cd src/TaskFlow.Server
dotnet run
# API disponible en: https://localhost:7000

# 3. Ejecutar Cliente (Terminal 2)
cd src/TaskFlow.Client
dotnet run
# Cliente disponible en: https://localhost:7001
```

## 🌐 URLs Importantes

- **Aplicación**: https://localhost:7001
- **API**: https://localhost:7000
- **Swagger**: https://localhost:7000/swagger

## 🗄️ Base de Datos

### Opción A: SQL Server (Por defecto)
- Usa LocalDB automáticamente
- Se crea al primer arranque

### Opción B: SQLite (Más simple)
1. Cambiar en `src/TaskFlow.Server/TaskFlow.Server.csproj`:
```xml
<PackageReference Include="Microsoft.EntityFrameworkCore.Sqlite" Version="8.0.0" />
```

2. Cambiar en `src/TaskFlow.Server/appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=taskflow.db"
  }
}
```

3. Cambiar en `src/TaskFlow.Server/Program.cs`:
```csharp
options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"))
```

## 🧪 Verificar Funcionamiento

```bash
# Ejecutar tests
cd tests/TaskFlow.Server.Tests
dotnet test

# Verificar compilación
dotnet build
```

## 🎯 Primeros Pasos en la App

1. **Ir a Projects** → Crear un proyecto
2. **Ir a Tasks** → Crear tareas
3. **Dashboard** → Ver estadísticas

## 🐛 Solución de Problemas

| Error | Solución |
|-------|----------|
| `dotnet: command not found` | Instalar .NET 8 SDK |
| `Database connection failed` | Verificar SQL Server o cambiar a SQLite |
| `Port already in use` | Cambiar puertos en `launchSettings.json` |
| `CORS error` | Verificar URLs en CORS policy |

## 📱 Funcionalidades Disponibles

✅ **Gestión de Tareas**
- Crear, editar, eliminar tareas
- Estados: Pending, In Progress, Completed
- Prioridades: Low, Medium, High, Critical

✅ **Gestión de Proyectos**  
- Crear y gestionar proyectos
- Asignar tareas a proyectos
- Ver progreso de proyectos

✅ **Dashboard**
- Estadísticas de tareas
- Métricas de productividad
- Alertas de tareas vencidas

## 🔧 Desarrollo

### VSCode (Recomendado)
```bash
code .
# F5 para debug
```

### Visual Studio
```bash
# Abrir TaskFlow.sln
# Configurar múltiples proyectos de inicio
# F5 para ejecutar
```

### Docker
```bash
docker-compose up
```

## 📚 Estructura del Proyecto

```
TaskFlow/
├── src/
│   ├── TaskFlow.Client/     # Blazor WebAssembly
│   ├── TaskFlow.Server/     # ASP.NET Core API  
│   └── TaskFlow.Shared/     # Modelos compartidos
├── tests/
│   └── TaskFlow.Server.Tests/
└── Configuración y scripts
```

¡Listo para empezar! 🎉
