#!/bin/bash

# 🚀 Script para Crear Feature Automáticamente
# Crea una nueva feature siguiendo las mejores prácticas

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

# Función para mostrar información
info() {
    echo -e "${BLUE}ℹ️ $1${NC}"
}

echo "🚀 Creador de Features - Git Flow Mejorado"
echo "=========================================="

# 1. Verificar argumentos
if [ $# -eq 0 ]; then
    echo "Uso: $0 <descripcion-de-la-feature>"
    echo ""
    echo "Ejemplos:"
    echo "  $0 user-authentication"
    echo "  $0 payment-integration"
    echo "  $0 email-notifications"
    echo ""
    echo "Se creará: feature/$(date +%Y-%m-%d)-<descripcion>"
    exit 1
fi

description=$1

# Validar descripción
if [[ ! $description =~ ^[a-z0-9-]+$ ]]; then
    error "La descripción debe contener solo letras minúsculas, números y guiones"
fi

# 2. Generar nombre de feature
feature_name="feature/$(date +%Y-%m-%d)-$description"
info "Feature a crear: $feature_name"

# 3. Verificar que estamos en develop
current_branch=$(git branch --show-current)
if [[ $current_branch != "develop" ]]; then
    echo "Cambiando a develop..."
    git checkout develop || error "No se pudo cambiar a develop"
fi

# 4. Actualizar develop
echo "🔄 Actualizando develop..."
git fetch origin develop || error "No se pudo hacer fetch de develop"
git pull origin develop || error "No se pudo actualizar develop"
success "Develop actualizado"

# 5. Verificar que la feature no existe
if git show-ref --verify --quiet refs/heads/$feature_name; then
    error "La feature '$feature_name' ya existe localmente"
fi

if git show-ref --verify --quiet refs/remotes/origin/$feature_name; then
    error "La feature '$feature_name' ya existe en el remoto"
fi

# 6. Crear feature branch
echo "🌱 Creando feature branch..."
git checkout -b $feature_name || error "No se pudo crear la feature branch"
success "Feature branch creada: $feature_name"

# 7. Crear commit inicial (opcional)
read -p "¿Crear commit inicial? (y/N): " create_initial_commit
if [[ $create_initial_commit =~ ^[Yy]$ ]]; then
    # Crear archivo README para la feature
    cat > "FEATURE_${description}.md" << EOF
# Feature: $description

## Descripción
[Describe aquí qué hace esta feature]

## Objetivos
- [ ] [Objetivo 1]
- [ ] [Objetivo 2]
- [ ] [Objetivo 3]

## Criterios de Aceptación
- [ ] [Criterio 1]
- [ ] [Criterio 2]
- [ ] [Criterio 3]

## Tests
- [ ] Unit tests
- [ ] Integration tests
- [ ] E2E tests (si aplica)

## Documentación
- [ ] Actualizar README
- [ ] Documentar API (si aplica)
- [ ] Actualizar changelog

## Notas de Desarrollo
[Notas técnicas, decisiones de diseño, etc.]

---
Creado: $(date)
Branch: $feature_name
EOF

    git add "FEATURE_${description}.md"
    git commit -m "feat: initialize $description feature

- Add feature documentation template
- Set up development structure

Scope: Initial setup"
    
    success "Commit inicial creado"
fi

# 8. Mostrar información útil
echo ""
echo "=========================================="
success "🎉 Feature creada exitosamente!"
echo ""
info "📋 Información de la Feature:"
echo "  Nombre: $feature_name"
echo "  Descripción: $description"
echo "  Fecha: $(date +%Y-%m-%d)"
echo ""
info "📝 Próximos pasos:"
echo "  1. Desarrollar la funcionalidad"
echo "  2. Hacer commits convencionales:"
echo "     git commit -m \"feat: add new functionality\""
echo "     git commit -m \"test: add unit tests\""
echo "     git commit -m \"docs: update documentation\""
echo ""
echo "  3. Validar antes de push:"
echo "     ./scripts/validate-feature.sh"
echo ""
echo "  4. Push de la feature:"
echo "     git push -u origin $feature_name"
echo ""
echo "  5. Finalizar feature:"
echo "     git checkout develop"
echo "     ./scripts/pre-merge-check.sh $feature_name"
echo "     git merge --no-ff $feature_name"
echo ""
info "💡 Tip: Usa './scripts/git-flow-status.sh' para ver el estado actual"
echo ""
echo "¡Feliz desarrollo! 🚀"
