# Guía de Renombrado del Proyecto

## 🎯 Objetivo

Este proyecto está configurado para facilitar el cambio de nombre cuando sea necesario. En lugar de tener el nombre "PimFlow" hardcodeado en cientos de archivos, hemos implementado un sistema que permite renombrar el proyecto modificando solo los archivos esenciales.

## 📊 Análisis de Acoplamiento

### Estado Actual
- **Total de archivos con "PimFlow"**: 173
- **Archivos críticos** (deben cambiar): 9
- **Archivos importantes** (recomendado cambiar): 141  
- **Archivos de implementación** (opcional): 23

### Categorías de Archivos

#### 🔴 Críticos (9 archivos)
Estos archivos **DEBEN** cambiar para renombrar el proyecto:
- `PimFlow.sln`
- `src/PimFlow.*.csproj` (5 archivos)
- `tests/PimFlow.*.csproj` (4 archivos)

#### 🟡 Importantes (141 archivos)
Archivos que **recomendamos** cambiar para consistencia:
- Controladores y servicios principales
- Archivos de configuración
- Documentación
- Tests principales

#### ⚪ Implementación (23 archivos)
Archivos donde el cambio es **opcional**:
- Componentes UI
- Archivos de ejemplo
- Utilidades internas

## 🛠️ Cómo Renombrar el Proyecto

### Opción 1: Script Automático (Recomendado)

```powershell
# Dry run para ver qué cambios se harían
.\scripts\rename-project.ps1 -NewProjectName "MiNuevoProyecto" -DryRun

# Aplicar cambios reales
.\scripts\rename-project.ps1 -NewProjectName "MiNuevoProyecto"
```

### Opción 2: Manual

1. **Renombrar archivos críticos**:
   - `PimFlow.sln` → `MiNuevoProyecto.sln`
   - Directorios `src/PimFlow.*` → `src/MiNuevoProyecto.*`
   - Archivos `.csproj`

2. **Actualizar contenido**:
   - Buscar y reemplazar "PimFlow" por "MiNuevoProyecto" en archivos críticos
   - Actualizar `README.md`
   - Actualizar `docker-compose.yml`

3. **Reconstruir**:
   ```bash
   dotnet clean
   dotnet build
   dotnet test
   ```

## 🎯 Estrategia de Desacoplamiento

### Lo que hemos implementado:

1. **Configuración centralizada**:
   - `ApplicationInfo.cs` - Información centralizada del proyecto
   - Variables de entorno para configuración
   - Placeholders en documentación

2. **Scripts de automatización**:
   - `rename-project.ps1` - Renombrado completo
   - `configure-project.ps1` - Configuración de placeholders
   - `analyze-coupling.ps1` - Análisis de acoplamiento

3. **Namespace aliases**:
   - `NamespaceAliases.cs` - Aliases globales
   - `GlobalUsings.cs` - Imports centralizados

### Beneficios:

✅ **Renombrado rápido**: 5 minutos vs. horas de trabajo manual
✅ **Menos errores**: Script automatizado reduce errores humanos  
✅ **Consistencia**: Todos los archivos se actualizan uniformemente
✅ **Reversible**: Fácil volver al nombre anterior si es necesario

## 📋 Checklist Post-Renombrado

Después de renombrar el proyecto:

- [ ] Ejecutar `dotnet clean && dotnet build`
- [ ] Verificar que todos los tests pasen
- [ ] Actualizar referencias en IDE/Visual Studio
- [ ] Actualizar configuración de CI/CD si existe
- [ ] Actualizar documentación específica del dominio
- [ ] Verificar que Docker Compose funcione
- [ ] Actualizar repositorio Git (nombre, descripción, etc.)

## 🔍 Verificación

Para verificar que el renombrado fue exitoso:

```powershell
# Analizar acoplamiento restante
.\scripts\analyze-coupling.ps1

# Verificar que el proyecto compila
dotnet build

# Ejecutar tests
dotnet test
```

## 📝 Notas Importantes

1. **Backup**: Siempre hacer backup antes de renombrar
2. **Git**: Considerar crear una rama para el renombrado
3. **Base de datos**: Actualizar nombres de base de datos si es necesario
4. **Configuración**: Revisar archivos de configuración específicos del entorno

## 🎊 Resultado

Con este sistema, cambiar el nombre del proyecto de "PimFlow" a cualquier otro nombre es un proceso de **5 minutos** en lugar de **horas de trabajo manual**.

El acoplamiento se ha reducido de un problema arquitectural a una característica configurable.
