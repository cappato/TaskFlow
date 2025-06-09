# 🌊 Git Flow - TaskFlow PIM

## 🎯 Descripción

Git Flow es una metodología de branching que organiza el desarrollo usando ramas específicas para diferentes tipos de trabajo. Este proyecto implementa Git Flow usando `main` como rama principal.

## 🏗️ Estructura de Ramas

```
main                    # 🚀 Producción (releases estables)
├── develop             # 🔧 Desarrollo (integración)
│   ├── feature/login   # ✨ Nueva característica
│   ├── feature/api     # ✨ Nueva característica
│   └── feature/ui      # ✨ Nueva característica
├── release/v1.2.0      # 🚀 Preparación de release
└── hotfix/v1.1.1       # 🔥 Corrección urgente
```

## 🚀 Instalación y Configuración

### **Paso 1: Inicializar Git Flow**
```bash
# Windows
gitflow init

# Linux/Mac
./gitflow.sh init
```

### **Paso 2: Verificar Estado**
```bash
# Windows
gitflow status

# Linux/Mac
./gitflow.sh status
```

## 🔧 Trabajando con Features

### **Crear Nueva Feature**
```bash
# Windows
gitflow feature start nombre-feature

# Linux/Mac
./gitflow.sh feature start nombre-feature
```

**Ejemplo:**
```bash
gitflow feature start user-authentication
```

### **Trabajar en la Feature**
```bash
# Hacer cambios en tu código
git add .
git commit -m "feat: implement user login functionality"
git push origin feature/user-authentication
```

### **Finalizar Feature**
```bash
# Windows
gitflow feature finish user-authentication

# Linux/Mac
./gitflow.sh feature finish user-authentication
```

### **Listar Features Activas**
```bash
# Windows
gitflow feature list

# Linux/Mac
./gitflow.sh feature list
```

## 🚀 Trabajando con Releases

### **Crear Nueva Release**
```bash
# Windows
gitflow release start v1.2.0

# Linux/Mac
./gitflow.sh release start v1.2.0
```

### **Preparar Release**
```bash
# En la rama release/v1.2.0
# - Actualizar versiones
# - Ejecutar tests finales
# - Actualizar CHANGELOG.md
# - Correcciones menores

git add .
git commit -m "chore: prepare release v1.2.0"
git push origin release/v1.2.0
```

### **Finalizar Release**
```bash
# Windows
gitflow release finish v1.2.0

# Linux/Mac
./gitflow.sh release finish v1.2.0
```

**Esto automáticamente:**
- ✅ Merge a `main`
- ✅ Crea tag `v1.2.0`
- ✅ Merge a `develop`
- ✅ Elimina rama `release/v1.2.0`

## 🔥 Trabajando con Hotfixes

### **Crear Hotfix**
```bash
# Windows
gitflow hotfix start v1.1.1

# Linux/Mac
./gitflow.sh hotfix start v1.1.1
```

### **Aplicar Corrección**
```bash
# En la rama hotfix/v1.1.1
# - Corregir el bug crítico
# - Ejecutar tests

git add .
git commit -m "fix: resolve critical security vulnerability"
git push origin hotfix/v1.1.1
```

### **Finalizar Hotfix**
```bash
# Windows
gitflow hotfix finish v1.1.1

# Linux/Mac
./gitflow.sh hotfix finish v1.1.1
```

**Esto automáticamente:**
- ✅ Merge a `main`
- ✅ Crea tag `v1.1.1`
- ✅ Merge a `develop`
- ✅ Elimina rama `hotfix/v1.1.1`

## 📋 Flujo de Trabajo Típico

### **🔧 Desarrollo de Feature**
1. `gitflow feature start nueva-funcionalidad`
2. Desarrollar y commitear cambios
3. `gitflow feature finish nueva-funcionalidad`

### **🚀 Preparación de Release**
1. `gitflow release start v1.3.0`
2. Preparar release (tests, docs, versioning)
3. `gitflow release finish v1.3.0`

### **🔥 Corrección Urgente**
1. `gitflow hotfix start v1.2.1`
2. Corregir bug crítico
3. `gitflow hotfix finish v1.2.1`

## 🎯 Convenciones de Naming

### **Features**
```
feature/user-authentication
feature/payment-integration
feature/admin-dashboard
```

### **Releases**
```
release/v1.0.0
release/v1.1.0
release/v2.0.0
```

### **Hotfixes**
```
hotfix/v1.0.1
hotfix/v1.1.1
hotfix/v2.0.1
```

## 📊 Comandos de Referencia Rápida

| Acción | Windows | Linux/Mac |
|--------|---------|-----------|
| Inicializar | `gitflow init` | `./gitflow.sh init` |
| Estado | `gitflow status` | `./gitflow.sh status` |
| Nueva feature | `gitflow feature start <name>` | `./gitflow.sh feature start <name>` |
| Finalizar feature | `gitflow feature finish <name>` | `./gitflow.sh feature finish <name>` |
| Nueva release | `gitflow release start <version>` | `./gitflow.sh release start <version>` |
| Finalizar release | `gitflow release finish <version>` | `./gitflow.sh release finish <version>` |
| Nuevo hotfix | `gitflow hotfix start <version>` | `./gitflow.sh hotfix start <version>` |
| Finalizar hotfix | `gitflow hotfix finish <version>` | `./gitflow.sh hotfix finish <version>` |

## ⚠️ Mejores Prácticas

### **✅ Hacer:**
- Usar conventional commits en todas las ramas
- Ejecutar tests antes de finalizar features/releases
- Mantener `develop` siempre estable
- Usar semantic versioning (v1.2.3)
- Documentar cambios en CHANGELOG.md

### **❌ Evitar:**
- Commitear directamente a `main`
- Mergear features incompletas a `develop`
- Crear releases sin testing completo
- Usar nombres de rama inconsistentes

## 🔍 Troubleshooting

### **Error: Rama no existe**
```bash
git fetch origin
git checkout develop
git pull origin develop
```

### **Conflictos de merge**
```bash
# Resolver conflictos manualmente
git add .
git commit -m "resolve merge conflicts"
```

### **Revertir cambios**
```bash
git reset --hard HEAD~1
```

## 🎯 Integración con CI/CD

- **main**: Deploys automáticos a producción
- **develop**: Deploys a staging/testing
- **feature/***: Builds y tests automáticos
- **release/***: Tests completos y preparación
- **hotfix/***: Tests rápidos y deploy urgente
