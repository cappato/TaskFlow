# 🚀 TaskFlow PIM - Sistema de Gestión de Productos

## 📋 Descripción

TaskFlow PIM es un sistema de gestión de información de productos (Product Information Management) especializado en artículos deportivos. Permite gestionar catálogos con **atributos dinámicos personalizables** sin necesidad de modificar código.

## ✨ Características Principales

### 🧩 **Atributos Dinámicos**
- Crear campos personalizados: `color`, `talle`, `material`, `temporada`, etc.
- Tipos de atributos: Texto, Número, Booleano, Fecha, Color, Email, URL
- Validaciones automáticas por tipo
- Búsqueda y filtrado por atributos personalizados

### 📦 **Gestión de Artículos**
- CRUD completo de artículos deportivos
- SKU único por artículo
- Categorización flexible
- Gestión de proveedores
- Búsqueda full-text

### 🎨 **Interfaz Moderna**
- Blazor WebAssembly + Server
- Tailwind CSS para diseño responsivo
- Formularios dinámicos que se adaptan a los atributos
- UI en español

## 🏗️ Arquitectura

### **Backend (.NET 8)**
- **Patrón Repository**: Abstracción de acceso a datos
- **Servicios de Negocio**: Lógica de aplicación separada
- **EAV Pattern**: Entity-Attribute-Value para atributos dinámicos
- **API REST**: Endpoints completos con validaciones

### **Frontend (Blazor)**
- **Componentes reutilizables**: DynamicForm, Button, Icon
- **Servicios de API**: Comunicación tipada con el backend
- **Layout moderno**: Navegación responsiva

### **Base de Datos (SQLite → MongoDB Ready)**
- **SQLite**: Para desarrollo y testing
- **Preparado para MongoDB**: Repositorios abstractos
- **Migraciones**: Entity Framework Core

## 🧪 Testing

### **Suite Completa de Tests (25+ tests)**
- ✅ **Tests de Repositorios**: CRUD y búsquedas
- ✅ **Tests de Servicios**: Lógica de negocio
- ✅ **Tests de Controladores**: API REST
- ✅ **Tests de Integración**: Workflow completo

### **Herramientas**
- xUnit + FluentAssertions + Moq
- InMemory Database para tests aislados
- Cobertura completa del sistema de atributos dinámicos

## 🚀 Instalación y Ejecución

### **Prerrequisitos**
- .NET 8 SDK
- Visual Studio 2022 o VS Code
- Git

### **Pasos de Instalación**

1. **Clonar el repositorio**
```bash
git clone <repository-url>
cd taskflow-pim
```

2. **Restaurar dependencias**
```bash
dotnet restore
```

3. **Ejecutar el servidor**
```bash
cd src/TaskFlow.Server
dotnet run
```

4. **Ejecutar el cliente (en otra terminal)**
```bash
cd src/TaskFlow.Client
dotnet run
```

5. **Abrir en el navegador**
```
https://localhost:7001
```

### **Ejecutar Tests**
```bash
dotnet test tests/TaskFlow.Server.Tests/
```

## 📖 Uso del Sistema

### **1. Gestionar Atributos Personalizados**
1. Ir a "Atributos" en el menú
2. Crear nuevos atributos: `color`, `talle`, `material`
3. Definir tipo y si es requerido

### **2. Crear Artículos**
1. Ir a "Artículos" en el menú
2. Crear nuevo artículo
3. Completar campos básicos (SKU, Nombre, Tipo, Marca)
4. Completar atributos personalizados dinámicamente

### **3. Buscar y Filtrar**
- Búsqueda por texto en nombre, SKU, marca
- Filtros por tipo de artículo
- Búsqueda por atributos personalizados

## 🔧 Configuración

### **Base de Datos**
El sistema usa SQLite por defecto. La base de datos se crea automáticamente en:
```
src/TaskFlow.Server/TaskFlow.db
```

### **Datos de Ejemplo**
El sistema incluye datos de ejemplo:
- Categorías: Calzado, Ropa, Zapatillas Running
- Atributos: talle, color, material, temporada, género
- Usuarios: Admin Cruzado, proveedores de ejemplo

## 🛠️ Desarrollo

### **Estructura del Proyecto**
```
src/
├── TaskFlow.Server/          # Backend API
│   ├── Controllers/          # Controladores REST
│   ├── Services/            # Lógica de negocio
│   ├── Repositories/        # Acceso a datos
│   ├── Models/              # Entidades de dominio
│   └── Data/                # DbContext y configuración
├── TaskFlow.Client/         # Frontend Blazor
│   ├── Pages/               # Páginas principales
│   ├── Components/          # Componentes reutilizables
│   └── Services/            # Servicios de API
├── TaskFlow.Shared/         # DTOs y enums compartidos
└── tests/
    └── TaskFlow.Server.Tests/ # Tests completos
```

### **Principios de Diseño**
- **SOLID**: Principios de diseño orientado a objetos
- **DRY**: No repetir código
- **Separation of Concerns**: Responsabilidades separadas
- **Future-Proof**: Preparado para migración a MongoDB

## 🔮 Roadmap Futuro

### **Fase 1 - Completado ✅**
- Sistema de atributos dinámicos
- CRUD de artículos y atributos
- Frontend Blazor funcional
- Tests completos

### **Fase 2 - Próximas Funcionalidades**
- Variantes de productos (talle/color por SKU)
- Categorías jerárquicas
- Gestión de stock
- Precios mayoristas

### **Fase 3 - Escalabilidad**
- Migración a MongoDB
- API de importación/exportación
- Reportes y analytics
- Multi-tenancy

## 🤝 Contribución

1. Fork el proyecto
2. Crear feature branch (`git checkout -b feature/nueva-funcionalidad`)
3. Commit cambios (`git commit -am 'Agregar nueva funcionalidad'`)
4. Push al branch (`git push origin feature/nueva-funcionalidad`)
5. Crear Pull Request

## 📄 Licencia

Este proyecto está bajo la Licencia MIT. Ver `LICENSE` para más detalles.

## 👥 Equipo

- **Desarrollador Principal**: Alejandro Cruzado
- **Arquitectura**: Basada en principios SOLID y buenas prácticas
- **Stack**: .NET 8, Blazor, Tailwind CSS, SQLite

---

**¡El PIM está listo para gestionar tu catálogo de productos deportivos! 🚀**
