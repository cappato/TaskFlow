# Contributing to TaskFlow

¡Gracias por tu interés en contribuir a TaskFlow! 🎉

## 🚀 Cómo Contribuir

### 1. Fork del Repositorio
```bash
# Hacer fork en GitHub y luego clonar
git clone https://github.com/TU-USUARIO/TaskFlow.git
cd TaskFlow
```

### 2. Configurar el Entorno
```bash
# Instalar dependencias
dotnet restore

# Ejecutar tests
dotnet test

# Ejecutar la aplicación
./run-dev.sh  # Linux/Mac
./run-dev.bat # Windows
```

### 3. Crear una Rama
```bash
git checkout -b feature/nueva-funcionalidad
# o
git checkout -b fix/correccion-bug
```

### 4. Hacer Cambios
- Seguir las convenciones de código existentes
- Agregar tests para nuevas funcionalidades
- Actualizar documentación si es necesario

### 5. Commit y Push
```bash
git add .
git commit -m "feat: agregar nueva funcionalidad"
git push origin feature/nueva-funcionalidad
```

### 6. Crear Pull Request
- Ir a GitHub y crear un Pull Request
- Describir los cambios realizados
- Referenciar issues relacionados

## 📝 Convenciones de Código

### Commits
Usar [Conventional Commits](https://www.conventionalcommits.org/):
- `feat:` nueva funcionalidad
- `fix:` corrección de bug
- `docs:` cambios en documentación
- `style:` cambios de formato
- `refactor:` refactoring de código
- `test:` agregar o modificar tests
- `chore:` tareas de mantenimiento

### C# Code Style
- Seguir las convenciones de .NET
- Usar EditorConfig incluido
- Ejecutar `dotnet format` antes de commit

### Estructura de Archivos
```
src/
├── TaskFlow.Client/     # Frontend Blazor
├── TaskFlow.Server/     # Backend API
└── TaskFlow.Shared/     # Modelos compartidos
tests/
└── TaskFlow.Server.Tests/
```

## 🧪 Testing

### Ejecutar Tests
```bash
# Todos los tests
dotnet test

# Tests específicos
dotnet test tests/TaskFlow.Server.Tests/

# Con coverage
dotnet test --collect:"XPlat Code Coverage"
```

### Escribir Tests
- Tests unitarios para servicios y repositorios
- Tests de integración para controllers
- Usar FluentAssertions para assertions
- Mockear dependencias con Moq

## 🐛 Reportar Bugs

### Antes de Reportar
- Verificar que no exista un issue similar
- Reproducir el bug en la última versión
- Recopilar información del entorno

### Template de Bug Report
```markdown
**Descripción del Bug**
Descripción clara del problema.

**Pasos para Reproducir**
1. Ir a '...'
2. Hacer click en '...'
3. Ver error

**Comportamiento Esperado**
Lo que debería pasar.

**Screenshots**
Si aplica, agregar screenshots.

**Entorno**
- OS: [e.g. Windows 11]
- .NET Version: [e.g. 8.0.1]
- Browser: [e.g. Chrome 120]
```

## 💡 Solicitar Funcionalidades

### Template de Feature Request
```markdown
**¿Tu solicitud está relacionada con un problema?**
Descripción clara del problema.

**Describe la solución que te gustaría**
Descripción clara de lo que quieres que pase.

**Describe alternativas consideradas**
Otras soluciones o funcionalidades consideradas.

**Contexto adicional**
Cualquier otro contexto o screenshots.
```

## 📋 Roadmap

### Próximas Funcionalidades
- [ ] Autenticación con ASP.NET Core Identity
- [ ] Asignación de usuarios a tareas
- [ ] Notificaciones en tiempo real (SignalR)
- [ ] Comentarios en tareas
- [ ] Archivos adjuntos
- [ ] Reportes y analytics
- [ ] API móvil
- [ ] Temas personalizables

### Mejoras Técnicas
- [ ] Implementar CQRS
- [ ] Agregar Redis para caching
- [ ] Mejorar performance
- [ ] Agregar más tests
- [ ] Documentación API con OpenAPI
- [ ] Containerización completa

## 🏷️ Labels de Issues

- `bug` - Algo no funciona
- `enhancement` - Nueva funcionalidad
- `documentation` - Mejoras en documentación
- `good first issue` - Bueno para principiantes
- `help wanted` - Se necesita ayuda extra
- `question` - Pregunta o discusión
- `wontfix` - No se va a arreglar

## 📞 Contacto

- **Issues**: Para bugs y feature requests
- **Discussions**: Para preguntas generales
- **Email**: [tu-email@ejemplo.com]

## 📄 Licencia

Al contribuir, aceptas que tus contribuciones serán licenciadas bajo la MIT License.

---

¡Gracias por contribuir a TaskFlow! 🙏
