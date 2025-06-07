#!/bin/bash

# 🔒 Script de Configuración Automática de Reglas de GitHub
# Este script configura automáticamente las Branch Protection Rules y otros settings

set -e

echo "🔒 TaskFlow - Configuración Automática de Reglas de GitHub"
echo "=========================================================="

# Colores para output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Función para logging
log_info() {
    echo -e "${BLUE}ℹ️  $1${NC}"
}

log_success() {
    echo -e "${GREEN}✅ $1${NC}"
}

log_warning() {
    echo -e "${YELLOW}⚠️  $1${NC}"
}

log_error() {
    echo -e "${RED}❌ $1${NC}"
}

# Verificar dependencias
check_dependencies() {
    log_info "Verificando dependencias..."
    
    if ! command -v gh &> /dev/null; then
        log_error "GitHub CLI no está instalado"
        echo "Instala GitHub CLI desde: https://cli.github.com/"
        exit 1
    fi
    
    if ! command -v jq &> /dev/null; then
        log_warning "jq no está instalado (opcional para mejor output)"
        echo "Instala con: sudo apt install jq (Ubuntu) o brew install jq (Mac)"
    fi
    
    log_success "Dependencias verificadas"
}

# Verificar autenticación
check_auth() {
    log_info "Verificando autenticación con GitHub..."
    
    if ! gh auth status &> /dev/null; then
        log_error "No estás autenticado con GitHub CLI"
        echo "Ejecuta: gh auth login"
        exit 1
    fi
    
    log_success "Autenticación verificada"
}

# Obtener información del repositorio
get_repo_info() {
    log_info "Obteniendo información del repositorio..."
    
    if [ -z "$GITHUB_REPOSITORY" ]; then
        # Intentar obtener del git remote
        REPO_URL=$(git remote get-url origin 2>/dev/null || echo "")
        if [[ $REPO_URL =~ github\.com[:/]([^/]+)/([^/.]+) ]]; then
            REPO_OWNER="${BASH_REMATCH[1]}"
            REPO_NAME="${BASH_REMATCH[2]}"
            GITHUB_REPOSITORY="$REPO_OWNER/$REPO_NAME"
        else
            log_error "No se pudo determinar el repositorio de GitHub"
            echo "Asegúrate de estar en un repositorio de GitHub o configura GITHUB_REPOSITORY"
            exit 1
        fi
    else
        IFS='/' read -r REPO_OWNER REPO_NAME <<< "$GITHUB_REPOSITORY"
    fi
    
    log_success "Repositorio: $GITHUB_REPOSITORY"
}

# Configurar Branch Protection Rules
setup_branch_protection() {
    log_info "Configurando Branch Protection Rules para 'main'..."
    
    # Crear el JSON de configuración
    PROTECTION_CONFIG=$(cat <<EOF
{
  "required_status_checks": {
    "strict": true,
    "contexts": [
      "🔒 Enforce Repository Rules / 📁 Validate Project Structure",
      "🔒 Enforce Repository Rules / 📝 Validate Commit Messages",
      "🔒 Enforce Repository Rules / 🔨 Build and Test",
      "🔍 Pull Request Validation / 📋 PR Rules Validation",
      "🔍 Pull Request Validation / 📝 Validate All Commits in PR",
      "🔍 Pull Request Validation / 📁 Validate File Changes"
    ]
  },
  "enforce_admins": true,
  "required_pull_request_reviews": {
    "required_approving_review_count": 1,
    "dismiss_stale_reviews": true,
    "require_code_owner_reviews": false,
    "require_last_push_approval": true
  },
  "restrictions": null,
  "allow_force_pushes": false,
  "allow_deletions": false,
  "required_conversation_resolution": true
}
EOF
)
    
    # Aplicar la configuración
    if gh api "repos/$GITHUB_REPOSITORY/branches/main/protection" \
        --method PUT \
        --input - <<< "$PROTECTION_CONFIG" > /dev/null 2>&1; then
        log_success "Branch Protection Rules configuradas"
    else
        log_error "Error configurando Branch Protection Rules"
        echo "Verifica que tengas permisos de administrador en el repositorio"
        return 1
    fi
}

# Configurar settings del repositorio
setup_repo_settings() {
    log_info "Configurando settings del repositorio..."
    
    # Habilitar features necesarios
    REPO_SETTINGS=$(cat <<EOF
{
  "has_issues": true,
  "has_projects": true,
  "has_wiki": true,
  "has_discussions": true,
  "allow_squash_merge": true,
  "allow_merge_commit": false,
  "allow_rebase_merge": true,
  "delete_branch_on_merge": true,
  "allow_auto_merge": false
}
EOF
)
    
    if gh api "repos/$GITHUB_REPOSITORY" \
        --method PATCH \
        --input - <<< "$REPO_SETTINGS" > /dev/null 2>&1; then
        log_success "Settings del repositorio configurados"
    else
        log_warning "No se pudieron configurar algunos settings (permisos insuficientes)"
    fi
}

# Configurar topics/tags
setup_topics() {
    log_info "Configurando topics del repositorio..."
    
    TOPICS=$(cat <<EOF
{
  "names": [
    "blazor",
    "aspnet-core", 
    "task-management",
    "csharp",
    "dotnet",
    "entity-framework",
    "bootstrap",
    "project-management",
    "web-api",
    "webassembly"
  ]
}
EOF
)
    
    if gh api "repos/$GITHUB_REPOSITORY/topics" \
        --method PUT \
        --input - <<< "$TOPICS" > /dev/null 2>&1; then
        log_success "Topics configurados"
    else
        log_warning "No se pudieron configurar los topics"
    fi
}

# Habilitar security features
setup_security() {
    log_info "Configurando features de seguridad..."
    
    # Habilitar Dependabot alerts
    if gh api "repos/$GITHUB_REPOSITORY/vulnerability-alerts" \
        --method PUT > /dev/null 2>&1; then
        log_success "Dependabot alerts habilitado"
    else
        log_warning "No se pudo habilitar Dependabot alerts"
    fi
    
    # Habilitar automated security fixes
    if gh api "repos/$GITHUB_REPOSITORY/automated-security-fixes" \
        --method PUT > /dev/null 2>&1; then
        log_success "Automated security fixes habilitado"
    else
        log_warning "No se pudo habilitar automated security fixes"
    fi
}

# Verificar configuración
verify_setup() {
    log_info "Verificando configuración..."
    
    # Verificar branch protection
    if gh api "repos/$GITHUB_REPOSITORY/branches/main/protection" > /dev/null 2>&1; then
        log_success "Branch protection está activo"
    else
        log_error "Branch protection NO está configurado correctamente"
        return 1
    fi
    
    # Verificar workflows
    if [ -f ".github/workflows/enforce-rules.yml" ] && [ -f ".github/workflows/pr-validation.yml" ]; then
        log_success "Workflows de validación están presentes"
    else
        log_error "Faltan workflows de validación"
        return 1
    fi
    
    log_success "Configuración verificada correctamente"
}

# Mostrar resumen
show_summary() {
    echo ""
    echo "🎉 ¡Configuración completada!"
    echo "=========================="
    echo ""
    echo "✅ Branch Protection Rules configuradas en 'main'"
    echo "✅ Workflows de validación activos"
    echo "✅ Settings de repositorio optimizados"
    echo "✅ Features de seguridad habilitados"
    echo ""
    echo "📋 Próximos pasos:"
    echo "1. Crear una rama de prueba: git checkout -b feature/test"
    echo "2. Hacer un commit: git commit -m 'feat: test branch protection'"
    echo "3. Crear un PR para verificar que las reglas funcionan"
    echo ""
    echo "📚 Documentación:"
    echo "- Reglas del repositorio: REGLAS_DEL_REPOSITORIO.md"
    echo "- Configuración manual: .github/BRANCH_PROTECTION_SETUP.md"
    echo "- Guía de contribución: CONTRIBUTING.md"
    echo ""
    echo "🔗 Repositorio: https://github.com/$GITHUB_REPOSITORY"
}

# Función principal
main() {
    echo ""
    log_info "Iniciando configuración automática..."
    echo ""
    
    check_dependencies
    check_auth
    get_repo_info
    
    echo ""
    log_info "Configurando reglas estrictas para: $GITHUB_REPOSITORY"
    echo ""
    
    setup_branch_protection
    setup_repo_settings
    setup_topics
    setup_security
    verify_setup
    
    show_summary
}

# Manejo de errores
trap 'log_error "Script interrumpido"; exit 1' INT TERM

# Ejecutar si es llamado directamente
if [[ "${BASH_SOURCE[0]}" == "${0}" ]]; then
    main "$@"
fi
