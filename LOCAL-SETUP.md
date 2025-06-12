# 🚀 PimFlow - Guía de Ejecución Local

## ⚡ Inicio Rápido

### 1. **Ejecutar el proyecto:**
```bash
.\run-local.bat
```

### 2. **Verificar estado:**
```bash
.\check-status.bat
```

## 🌐 URLs Disponibles

| Servicio | URL | Descripción |
|----------|-----|-------------|
| **Aplicación Principal** | http://localhost:5020 | Interfaz web principal |
| **Swagger API** | http://localhost:5020/swagger | Documentación interactiva de la API |
| **Health Check** | http://localhost:5020/health | Estado del sistema |
| **Test Endpoint** | http://localhost:5020/api/test | Endpoint de prueba |

## 📚 API Endpoints Principales

### 🏷️ **Articles (Artículos)**
- `GET /api/articles` - Listar todos los artículos
- `GET /api/articles/{id}` - Obtener artículo por ID
- `POST /api/articles` - Crear nuevo artículo
- `PUT /api/articles/{id}` - Actualizar artículo
- `DELETE /api/articles/{id}` - Eliminar artículo

### 📂 **Categories (Categorías)**
- `GET /api/categories` - Listar todas las categorías
- `GET /api/categories/{id}` - Obtener categoría por ID
- `POST /api/categories` - Crear nueva categoría
- `PUT /api/categories/{id}` - Actualizar categoría
- `DELETE /api/categories/{id}` - Eliminar categoría

### ⚙️ **Custom Attributes (Atributos Personalizados)**
- `GET /api/customattributes` - Listar atributos personalizados
- `GET /api/customattributes/{id}` - Obtener atributo por ID
- `POST /api/customattributes` - Crear nuevo atributo
- `PUT /api/customattributes/{id}` - Actualizar atributo
- `DELETE /api/customattributes/{id}` - Eliminar atributo

## 🛠️ Comandos de Desarrollo

### **Compilar:**
```bash
dotnet build
```

### **Ejecutar tests:**
```bash
# Todos los tests
dotnet test

# Solo tests del dominio
dotnet test tests/PimFlow.Domain.Tests/

# Solo tests del servidor
dotnet test tests/PimFlow.Server.Tests/
```

### **Limpiar y reconstruir:**
```bash
dotnet clean
dotnet restore
dotnet build
```

## 💾 Base de Datos

- **Tipo**: SQLite
- **Ubicación**: `src/PimFlow.Server/App_Data/application-dev.db`
- **Inicialización**: Automática al iniciar la aplicación
- **Datos de ejemplo**: Se cargan automáticamente

## 🔧 Configuración

### **Entorno de Desarrollo:**
- Puerto: `5020`
- Entorno: `Development`
- Logs: Habilitados
- Swagger: Habilitado

### **Variables de Entorno:**
```bash
ASPNETCORE_ENVIRONMENT=Development
ASPNETCORE_URLS=http://localhost:5020
```

## 🧪 Testing

### **Estado Actual de Tests:**
- ✅ **Domain**: 40/40 tests (100%)
- ✅ **Server**: 139/139 tests (100%)
- ✅ **Architecture**: 19/19 tests (100%)
- ⚠️ **Shared**: 149/152 tests (98%)
- 🎯 **Total**: 347/350 tests (99.1%)

## 🚨 Solución de Problemas

### **Si el servidor no inicia:**
1. Verificar que el puerto 5020 esté libre
2. Ejecutar `taskkill /f /im dotnet.exe` para limpiar procesos
3. Ejecutar `.\run-local.bat` nuevamente

### **Si Swagger no carga:**
1. Verificar que el servidor esté ejecutándose
2. Ir a http://localhost:5020/swagger
3. Verificar la consola por errores

### **Si hay errores de compilación:**
1. Ejecutar `dotnet clean`
2. Ejecutar `dotnet restore`
3. Ejecutar `dotnet build`

## 📁 Estructura del Proyecto

```
PimFlow/
├── src/
│   ├── PimFlow.Domain/          # Lógica de dominio (refactorizada)
│   ├── PimFlow.Server/          # API REST y servidor web
│   ├── PimFlow.Client/          # Cliente Blazor
│   ├── PimFlow.Shared/          # DTOs y validadores compartidos
│   └── PimFlow.Contracts/       # Interfaces y contratos
├── tests/
│   ├── PimFlow.Domain.Tests/    # Tests del dominio
│   ├── PimFlow.Server.Tests/    # Tests del servidor
│   ├── PimFlow.Shared.Tests/    # Tests compartidos
│   └── PimFlow.Architecture.Tests/ # Tests de arquitectura
└── scripts/                    # Scripts de utilidad
```

## 🎯 Características Principales

- ✅ **Arquitectura DDD** con estructura por features
- ✅ **API REST completa** con Swagger
- ✅ **Base de datos SQLite** con Entity Framework
- ✅ **Validación robusta** con FluentValidation
- ✅ **Eventos de dominio** implementados
- ✅ **Tests automatizados** (99.1% funcionando)
- ✅ **Documentación automática** con Swagger

---

**¡PimFlow está listo para desarrollo productivo!** 🎉
