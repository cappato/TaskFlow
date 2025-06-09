# 🛠️ Scripts de Git Flow Mejorado

Este directorio contiene scripts para automatizar y mejorar nuestro workflow de Git Flow.

## 📁 Estructura de Scripts

```
scripts/
├── README.md                    # Este archivo
├── validate-feature.sh          # Validación de feature (Bash)
├── validate-feature.ps1         # Validación de feature (PowerShell)
├── pre-merge-check.sh           # Verificaciones pre-merge (Bash)
├── git-flow-status.sh           # Estado del workflow (Bash)
└── create-feature.sh            # Crear feature automáticamente (Bash)
```

## 🚀 Scripts Disponibles

### **1. 🧪 validate-feature**
Valida que una feature esté lista para push.

**Uso:**
```bash
# Bash (Linux/Mac)
./scripts/validate-feature.sh

# PowerShell (Windows)
./scripts/validate-feature.ps1
```

**Verificaciones:**
- ✅ Branch es una feature válida
- ✅ Naming convention correcta
- ✅ Hay commits en la feature
- ✅ Tests pasan
- ✅ Build exitoso
- ✅ Commits convencionales
- ✅ No hay cambios pendientes

### **2. 🔍 pre-merge-check**
Verifica que todo esté listo para mergear a develop.

**Uso:**
```bash
# Desde develop
git checkout develop
./scripts/pre-merge-check.sh feature/2025-06-09-nombre-feature
```

**Verificaciones:**
- ✅ Estás en develop
- ✅ Develop actualizado
- ✅ Feature existe
- ✅ No hay conflictos
- ✅ Tests pasan
- ✅ Build exitoso

### **3. 📊 git-flow-status**
Muestra el estado actual del workflow.

**Uso:**
```bash
./scripts/git-flow-status.sh
```

**Información mostrada:**
- 📍 Branch actual
- 📁 Estado del workspace
- 🌊 Estado de branches principales
- 🔧 Features activas
- 📝 Últimos commits
- 🧪 Estado de tests

### **4. 🚀 create-feature**
Crea una nueva feature automáticamente.

**Uso:**
```bash
./scripts/create-feature.sh nombre-de-la-feature
```

### **5. 🏗️ test-architecture**
Ejecuta tests de arquitectura por categorías.

**Uso:**
```bash
# Tests críticos (obligatorios)
./scripts/test-architecture.ps1 -Category Critical

# Tests aspiracionales (objetivos)
./scripts/test-architecture.ps1 -Category Aspirational

# Tests de monitoreo (métricas)
./scripts/test-architecture.ps1 -Category Monitoring

# Todos los tests
./scripts/test-architecture.ps1 -Category All
```

**Categorías:**
- **Critical**: Deben pasar siempre (bloquean desarrollo)
- **Aspirational**: Objetivos arquitectónicos (pueden fallar temporalmente)
- **Monitoring**: Métricas y tendencias (informativos)

**Ejemplo:**
```bash
./scripts/create-feature.sh user-authentication
# Crea: feature/2025-06-09-user-authentication
```

**Proceso automático:**
- 🔄 Actualiza develop
- 🌱 Crea feature branch
- 📝 Commit inicial (opcional)
- 💡 Muestra próximos pasos

## 🔧 Configuración

### **Permisos (Linux/Mac):**
```bash
chmod +x scripts/*.sh
```

### **Ejecución en Windows:**
```powershell
# Permitir ejecución de scripts
Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope CurrentUser

# Ejecutar script
./scripts/validate-feature.ps1
```

## 🎯 Workflow Completo con Scripts

### **1. 🚀 Crear Nueva Feature:**
```bash
./scripts/create-feature.sh mi-nueva-feature
```

### **2. 🔧 Desarrollar:**
```bash
# Hacer commits convencionales
git add .
git commit -m "feat: add new functionality"
git commit -m "test: add unit tests"
```

### **3. 🧪 Validar Feature:**
```bash
./scripts/validate-feature.sh
```

### **4. 📤 Push Feature:**
```bash
git push -u origin $(git branch --show-current)
```

### **5. 🔄 Finalizar Feature:**
```bash
git checkout develop
./scripts/pre-merge-check.sh feature/2025-06-09-mi-nueva-feature
git merge --no-ff feature/2025-06-09-mi-nueva-feature -m "feat: complete mi nueva feature"
git push origin develop
git branch -d feature/2025-06-09-mi-nueva-feature
```

### **6. 📊 Verificar Estado:**
```bash
./scripts/git-flow-status.sh
```

## 🎨 Personalización

### **Modificar Validaciones:**
Edita los scripts para agregar/quitar validaciones según las necesidades del proyecto.

### **Agregar Nuevos Scripts:**
Sigue el patrón de naming y documentación existente.

### **Integración con CI/CD:**
Los scripts pueden integrarse con GitHub Actions, GitLab CI, etc.

## 🐛 Troubleshooting

### **Error: "Permission denied"**
```bash
chmod +x scripts/nombre-script.sh
```

### **Error: "dotnet not found"**
Los scripts funcionan sin dotnet, pero saltan las validaciones de build/test.

### **Error: "Branch no válida"**
Asegúrate de estar en una feature branch que siga el naming convention.

## 📚 Recursos Adicionales

- [Git Flow Mejorado](../docs/git-flow-improved.md)
- [Conventional Commits](https://www.conventionalcommits.org/)
- [Reglas de Onboarding](../docs/onboarding-rules.md)

## 🤝 Contribución

Para mejorar los scripts:
1. Crear feature para mejoras
2. Probar scripts modificados
3. Actualizar documentación
4. Seguir el workflow estándar
