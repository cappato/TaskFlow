#!/bin/bash

# 🔍 Script de Verificación Pre-Merge
# Verifica que todo esté listo para mergear a develop

echo "🔍 Iniciando verificación pre-merge..."
echo "====================================="

# Colores para output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
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

# Función para mostrar información
info() {
    echo -e "${BLUE}ℹ️ $1${NC}"
}

# 1. Verificar que estamos en develop
echo "🌊 Verificando branch actual..."
current_branch=$(git branch --show-current)
if [[ $current_branch != "develop" ]]; then
    error "Debes estar en la branch develop para hacer merge. Branch actual: $current_branch"
fi
success "Estás en develop"

# 2. Verificar que develop está actualizado
echo "🔄 Verificando que develop está actualizado..."
git fetch origin develop
local_commit=$(git rev-parse develop)
remote_commit=$(git rev-parse origin/develop)

if [[ $local_commit != $remote_commit ]]; then
    warning "Develop local no está actualizado con origin/develop"
    echo "Ejecuta: git pull origin develop"
    exit 1
fi
success "Develop está actualizado"

# 3. Verificar que la feature branch existe
if [ $# -eq 0 ]; then
    error "Debes proporcionar el nombre de la feature branch como argumento"
fi

feature_branch=$1
echo "🔍 Verificando feature branch: $feature_branch"

if ! git show-ref --verify --quiet refs/heads/$feature_branch; then
    error "La feature branch '$feature_branch' no existe localmente"
fi
success "Feature branch existe: $feature_branch"

# 4. Verificar que la feature tiene commits nuevos
echo "📝 Verificando commits de la feature..."
commit_count=$(git rev-list --count $feature_branch ^develop)
if [ $commit_count -eq 0 ]; then
    error "La feature branch no tiene commits nuevos respecto a develop"
fi
success "Feature tiene $commit_count commits nuevos"

# 5. Verificar que no hay conflictos
echo "⚔️ Verificando conflictos potenciales..."
if ! git merge-tree $(git merge-base develop $feature_branch) develop $feature_branch | grep -q "<<<<<<< "; then
    success "No hay conflictos detectados"
else
    error "Se detectaron conflictos potenciales. Resuelve los conflictos antes del merge."
fi

# 6. Ejecutar tests completos
echo "🧪 Ejecutando tests completos..."
if command -v dotnet &> /dev/null; then
    if ! dotnet test --verbosity minimal; then
        error "Tests fallaron en develop"
    fi
    success "Todos los tests pasan en develop"
else
    warning "dotnet no encontrado, saltando tests"
fi

# 7. Verificar build completo
echo "🔧 Verificando build completo..."
if command -v dotnet &> /dev/null; then
    if ! dotnet build --verbosity minimal; then
        error "Build falló en develop"
    fi
    success "Build exitoso en develop"
else
    warning "dotnet no encontrado, saltando build"
fi

# 8. Mostrar resumen de la feature
echo ""
echo "📊 Resumen de la feature:"
echo "========================"
info "Feature: $feature_branch"
info "Commits: $commit_count"
echo ""
echo "Últimos commits de la feature:"
git log --oneline develop..$feature_branch | head -5

# 9. Sugerir comando de merge
echo ""
echo "====================================="
echo -e "${GREEN}🎉 Verificación pre-merge completada!${NC}"
echo -e "${GREEN}✅ Listo para mergear${NC}"
echo ""
echo "Comando sugerido para merge:"
echo -e "${BLUE}git merge --no-ff $feature_branch -m \"feat: [descripción de la feature]\"${NC}"
echo ""
echo "Después del merge:"
echo -e "${BLUE}git push origin develop${NC}"
echo -e "${BLUE}git branch -d $feature_branch${NC}"
