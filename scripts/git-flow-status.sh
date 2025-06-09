#!/bin/bash

# 📊 Script de Estado del Git Flow
# Muestra el estado actual del workflow

echo "📊 Estado del Git Flow - PimFlow"
echo "================================="

# Colores para output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
PURPLE='\033[0;35m'
NC='\033[0m' # No Color

# Función para mostrar información
info() {
    echo -e "${BLUE}$1${NC}"
}

# Función para mostrar éxito
success() {
    echo -e "${GREEN}$1${NC}"
}

# Función para mostrar advertencias
warning() {
    echo -e "${YELLOW}$1${NC}"
}

# Función para mostrar errores
error() {
    echo -e "${RED}$1${NC}"
}

# 1. Información básica
echo ""
info "🌊 Información del Repositorio:"
echo "Repository: $(basename $(git rev-parse --show-toplevel))"
echo "Remote: $(git remote get-url origin 2>/dev/null || echo 'No remote configured')"

# 2. Branch actual
echo ""
info "📍 Branch Actual:"
current_branch=$(git branch --show-current)
if [[ $current_branch == "main" ]]; then
    error "⚠️ Estás en MAIN - ¡Cuidado!"
elif [[ $current_branch == "develop" ]]; then
    success "✅ Estás en DEVELOP"
elif [[ $current_branch == feature/* ]]; then
    success "✅ Estás en FEATURE: $current_branch"
else
    warning "⚠️ Branch no estándar: $current_branch"
fi

# 3. Estado del workspace
echo ""
info "📁 Estado del Workspace:"

# Cambios no commiteados
if git diff-index --quiet HEAD --; then
    success "✅ No hay cambios pendientes"
else
    warning "⚠️ Hay cambios no commiteados"
    git status --porcelain | head -5
fi

# Archivos no trackeados
untracked=$(git ls-files --others --exclude-standard | wc -l)
if [ $untracked -eq 0 ]; then
    success "✅ No hay archivos no trackeados"
else
    warning "⚠️ $untracked archivos no trackeados"
fi

# 4. Estado de branches principales
echo ""
info "🌊 Estado de Branches Principales:"

# Fetch para obtener info actualizada
git fetch origin --quiet 2>/dev/null

# Main
if git show-ref --verify --quiet refs/heads/main; then
    main_local=$(git rev-parse main 2>/dev/null)
    main_remote=$(git rev-parse origin/main 2>/dev/null)
    if [[ $main_local == $main_remote ]]; then
        success "✅ main: Sincronizado"
    else
        warning "⚠️ main: Desincronizado"
    fi
else
    error "❌ main: No existe localmente"
fi

# Develop
if git show-ref --verify --quiet refs/heads/develop; then
    develop_local=$(git rev-parse develop 2>/dev/null)
    develop_remote=$(git rev-parse origin/develop 2>/dev/null)
    if [[ $develop_local == $develop_remote ]]; then
        success "✅ develop: Sincronizado"
    else
        warning "⚠️ develop: Desincronizado"
    fi
else
    error "❌ develop: No existe localmente"
fi

# 5. Features activas
echo ""
info "🔧 Features Activas:"
feature_branches=$(git branch | grep "feature/" | wc -l)
if [ $feature_branches -eq 0 ]; then
    info "No hay features locales activas"
else
    echo "Features locales ($feature_branches):"
    git branch | grep "feature/" | sed 's/^/  /'
fi

# Features remotas
remote_features=$(git branch -r | grep "origin/feature/" | wc -l)
if [ $remote_features -gt 0 ]; then
    echo ""
    echo "Features remotas ($remote_features):"
    git branch -r | grep "origin/feature/" | sed 's/^/  /' | head -5
    if [ $remote_features -gt 5 ]; then
        echo "  ... y $(($remote_features - 5)) más"
    fi
fi

# 6. Últimos commits
echo ""
info "📝 Últimos Commits en $current_branch:"
git log --oneline -5 | sed 's/^/  /'

# 7. Tests (si es posible ejecutarlos)
echo ""
info "🧪 Estado de Tests:"
if command -v dotnet &> /dev/null; then
    if [ -f "*.sln" ] || find . -name "*.csproj" -type f | head -1 | grep -q "."; then
        echo "Ejecutando tests rápidos..."
        if dotnet test --verbosity quiet --no-build 2>/dev/null; then
            success "✅ Tests: Pasando"
        else
            error "❌ Tests: Fallando"
        fi
    else
        info "No se encontraron proyectos .NET"
    fi
else
    info "dotnet no disponible"
fi

# 8. Sugerencias
echo ""
info "💡 Sugerencias:"

if [[ $current_branch == "develop" ]]; then
    echo "  • Para crear nueva feature: git checkout -b feature/\$(date +%Y-%m-%d)-descripcion"
    echo "  • Para mergear feature: ./scripts/pre-merge-check.sh feature/nombre"
elif [[ $current_branch == feature/* ]]; then
    echo "  • Para validar feature: ./scripts/validate-feature.sh"
    echo "  • Para hacer push: git push -u origin $current_branch"
    echo "  • Para finalizar: git checkout develop && ./scripts/pre-merge-check.sh $current_branch"
else
    echo "  • Considera cambiar a develop o crear una feature"
fi

echo ""
echo "================================="
echo -e "${GREEN}📊 Estado del Git Flow completado${NC}"
