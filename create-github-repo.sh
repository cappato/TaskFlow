#!/bin/bash

# Script para crear repositorio TaskFlow en GitHub usando API

set -e

echo "Creando repositorio TaskFlow en GitHub..."

# Colores para output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m'

# Función para logging
log_info() {
    echo -e "${BLUE}INFO: $1${NC}"
}

log_success() {
    echo -e "${GREEN}SUCCESS: $1${NC}"
}

log_warning() {
    echo -e "${YELLOW}WARNING: $1${NC}"
}

log_error() {
    echo -e "${RED}ERROR: $1${NC}"
}

# Verificar que tenemos git configurado
if ! git config user.name > /dev/null 2>&1; then
    log_error "Git user.name no está configurado"
    exit 1
fi

if ! git config user.email > /dev/null 2>&1; then
    log_error "Git user.email no está configurado"
    exit 1
fi

# Verificar que estamos en un repositorio git
if ! git rev-parse --git-dir > /dev/null 2>&1; then
    log_error "No estamos en un repositorio git"
    exit 1
fi

# Información del repositorio
REPO_NAME="TaskFlow"
REPO_DESCRIPTION="A modern task and project management application built with Blazor WebAssembly and ASP.NET Core Web API"
REPO_HOMEPAGE="https://github.com/yourusername/TaskFlow"

# Crear payload JSON para la API
REPO_PAYLOAD=$(cat <<EOF
{
  "name": "$REPO_NAME",
  "description": "$REPO_DESCRIPTION",
  "homepage": "$REPO_HOMEPAGE",
  "private": false,
  "has_issues": true,
  "has_projects": true,
  "has_wiki": true,
  "has_discussions": true,
  "allow_squash_merge": true,
  "allow_merge_commit": false,
  "allow_rebase_merge": true,
  "delete_branch_on_merge": true,
  "allow_auto_merge": false,
  "auto_init": false,
  "license_template": "mit",
  "gitignore_template": "VisualStudio"
}
EOF
)

log_info "Configuración del repositorio:"
echo "Nombre: $REPO_NAME"
echo "Descripción: $REPO_DESCRIPTION"
echo "Público: Sí"
echo "Issues: Habilitado"
echo "Projects: Habilitado"
echo "Wiki: Habilitado"
echo "Discussions: Habilitado"
echo ""

# Nota: Para crear el repositorio necesitarías un token de GitHub
log_warning "NOTA IMPORTANTE:"
echo "Para crear el repositorio automáticamente necesitas:"
echo "1. Un token de GitHub con permisos de 'repo'"
echo "2. Configurar el token como variable de entorno GITHUB_TOKEN"
echo ""
echo "Comando para crear el repositorio:"
echo "curl -X POST \\"
echo "  -H 'Authorization: token \$GITHUB_TOKEN' \\"
echo "  -H 'Accept: application/vnd.github.v3+json' \\"
echo "  https://api.github.com/user/repos \\"
echo "  -d '$REPO_PAYLOAD'"
echo ""

# Verificar si tenemos token
if [ -z "$GITHUB_TOKEN" ]; then
    log_warning "Variable GITHUB_TOKEN no está configurada"
    echo ""
    echo "INSTRUCCIONES MANUALES:"
    echo "1. Ir a https://github.com/new"
    echo "2. Nombre del repositorio: $REPO_NAME"
    echo "3. Descripción: $REPO_DESCRIPTION"
    echo "4. Público"
    echo "5. NO inicializar con README (ya tenemos uno)"
    echo "6. Crear repositorio"
    echo ""
    echo "Luego ejecutar:"
    echo "git remote add origin https://github.com/TUUSUARIO/TaskFlow.git"
    echo "git push -u origin main"
    echo "git push origin v1.0.0"
    echo ""
    exit 0
fi

# Intentar crear el repositorio
log_info "Creando repositorio en GitHub..."

RESPONSE=$(curl -s -X POST \
  -H "Authorization: token $GITHUB_TOKEN" \
  -H "Accept: application/vnd.github.v3+json" \
  https://api.github.com/user/repos \
  -d "$REPO_PAYLOAD")

# Verificar respuesta
if echo "$RESPONSE" | grep -q '"clone_url"'; then
    CLONE_URL=$(echo "$RESPONSE" | grep '"clone_url"' | cut -d'"' -f4)
    log_success "Repositorio creado exitosamente!"
    echo "URL: $CLONE_URL"
    
    # Agregar remote y hacer push
    log_info "Configurando remote y haciendo push..."
    git remote add origin "$CLONE_URL"
    git push -u origin main
    git push origin v1.0.0
    
    log_success "Repositorio TaskFlow creado y subido a GitHub!"
    echo "URL del repositorio: $CLONE_URL"
    
else
    log_error "Error creando el repositorio"
    echo "Respuesta de la API:"
    echo "$RESPONSE"
    exit 1
fi
