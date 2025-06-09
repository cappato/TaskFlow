#!/bin/bash

# 🧪 Script de Validación de Feature
# Valida que la feature esté lista para push

echo "🧪 Iniciando validación de feature..."
echo "=================================="

# Colores para output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

# Función para mostrar errores
error() {
    echo -e "${RED}❌ Error: $1${NC}"
    exit 1
}

# Función para mostrar éxito
success() {
    echo -e "${GREEN}✅ $1${NC}"
}

# Función para mostrar advertencias
warning() {
    echo -e "${YELLOW}⚠️ $1${NC}"
}

# 1. Verificar que estamos en una feature branch
echo "🔍 Verificando branch actual..."
current_branch=$(git branch --show-current)
if [[ ! $current_branch == feature/* ]]; then
    error "No estás en una feature branch. Branch actual: $current_branch"
fi
success "Branch válida: $current_branch"

# 2. Verificar naming convention
echo "🏷️ Verificando naming convention..."
if [[ $current_branch =~ ^feature/[0-9]{4}-[0-9]{2}-[0-9]{2}-.+ ]]; then
    success "Naming convention correcta"
else
    warning "Naming convention no sigue el patrón: feature/YYYY-MM-DD-descripcion"
fi

# 3. Verificar que hay commits
echo "📝 Verificando commits..."
commit_count=$(git rev-list --count HEAD ^develop)
if [ $commit_count -eq 0 ]; then
    error "No hay commits en esta feature branch"
fi
success "Feature tiene $commit_count commits"

# 4. Ejecutar tests
echo "🧪 Ejecutando tests..."
if command -v dotnet &> /dev/null; then
    if ! dotnet test --verbosity minimal --no-build; then
        error "Tests fallaron"
    fi
    success "Todos los tests pasaron"
else
    warning "dotnet no encontrado, saltando tests"
fi

# 5. Verificar build
echo "🔧 Verificando build..."
if command -v dotnet &> /dev/null; then
    if ! dotnet build --verbosity minimal; then
        error "Build falló"
    fi
    success "Build exitoso"
else
    warning "dotnet no encontrado, saltando build"
fi

# 6. Verificar commits convencionales (básico)
echo "💬 Verificando commits convencionales..."
non_conventional=$(git log --oneline develop..HEAD | grep -v -E '^[a-f0-9]+ (feat|fix|docs|test|refactor|style|chore)(\(.+\))?: .+' | wc -l)
if [ $non_conventional -gt 0 ]; then
    warning "$non_conventional commits no siguen conventional commits"
    git log --oneline develop..HEAD | grep -v -E '^[a-f0-9]+ (feat|fix|docs|test|refactor|style|chore)(\(.+\))?: .+'
else
    success "Todos los commits siguen conventional commits"
fi

# 7. Verificar archivos no trackeados
echo "📁 Verificando archivos no trackeados..."
untracked=$(git ls-files --others --exclude-standard | wc -l)
if [ $untracked -gt 0 ]; then
    warning "Hay $untracked archivos no trackeados:"
    git ls-files --others --exclude-standard
fi

# 8. Verificar cambios no commiteados
echo "📝 Verificando cambios pendientes..."
if ! git diff-index --quiet HEAD --; then
    error "Hay cambios no commiteados"
fi
success "No hay cambios pendientes"

echo ""
echo "=================================="
echo -e "${GREEN}🎉 Validación completada exitosamente!${NC}"
echo -e "${GREEN}✅ Feature lista para push${NC}"
echo ""
echo "Para hacer push:"
echo "git push -u origin $current_branch"
