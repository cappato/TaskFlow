#!/bin/bash

# 🔍 Script de Validación de Documentación de Onboarding
# Verifica que la documentación esté consistente con el proyecto actual

echo "🔍 Validando Documentación de Onboarding..."
echo "=============================================="

# Colores para output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

# Contadores
ERRORS=0
WARNINGS=0
SUCCESS=0

# Función para reportar errores
report_error() {
    echo -e "${RED}❌ ERROR: $1${NC}"
    ((ERRORS++))
}

# Función para reportar warnings
report_warning() {
    echo -e "${YELLOW}⚠️  WARNING: $1${NC}"
    ((WARNINGS++))
}

# Función para reportar éxitos
report_success() {
    echo -e "${GREEN}✅ OK: $1${NC}"
    ((SUCCESS++))
}

echo ""
echo "📋 Verificando Referencias de Proyecto..."

# 1. Verificar que no haya referencias a TaskFlow
if grep -r "TaskFlow" docs/onboarding-rules.md > /dev/null; then
    report_error "Encontradas referencias a 'TaskFlow' en onboarding-rules.md"
else
    report_success "No hay referencias incorrectas a 'TaskFlow'"
fi

# 2. Verificar que los scripts mencionados existan
echo ""
echo "📋 Verificando Scripts Mencionados..."

SCRIPTS_TO_CHECK=(
    "scripts/create-feature.sh"
    "scripts/git-flow-status.sh"
    "scripts/validate-feature.sh"
    "scripts/pre-merge-check.sh"
)

for script in "${SCRIPTS_TO_CHECK[@]}"; do
    if [ -f "$script" ]; then
        report_success "Script existe: $script"
    else
        report_error "Script no encontrado: $script"
    fi
done

# 3. Verificar estructura de proyectos
echo ""
echo "📋 Verificando Estructura de Proyectos..."

PROJECT_DIRS=(
    "src/PimFlow.Domain"
    "src/PimFlow.Server"
    "src/PimFlow.Client"
    "src/PimFlow.Shared"
    "src/PimFlow.Contracts"
    "tests/PimFlow.Domain.Tests"
    "tests/PimFlow.Server.Tests"
    "tests/PimFlow.Shared.Tests"
    "tests/PimFlow.Architecture.Tests"
)

for dir in "${PROJECT_DIRS[@]}"; do
    if [ -d "$dir" ]; then
        report_success "Directorio existe: $dir"
    else
        report_error "Directorio no encontrado: $dir"
    fi
done

# 4. Verificar que los comandos de test sean válidos
echo ""
echo "📋 Verificando Comandos de Test..."

# Verificar que los proyectos de test tengan archivos .csproj
TEST_PROJECTS=(
    "tests/PimFlow.Domain.Tests/PimFlow.Domain.Tests.csproj"
    "tests/PimFlow.Server.Tests/PimFlow.Server.Tests.csproj"
    "tests/PimFlow.Shared.Tests/PimFlow.Shared.Tests.csproj"
    "tests/PimFlow.Architecture.Tests/PimFlow.Architecture.Tests.csproj"
)

for project in "${TEST_PROJECTS[@]}"; do
    if [ -f "$project" ]; then
        report_success "Proyecto de test existe: $project"
    else
        report_error "Proyecto de test no encontrado: $project"
    fi
done

# 5. Verificar configuración de puertos
echo ""
echo "📋 Verificando Configuración de Puertos..."

if [ -f "src/PimFlow.Server/Properties/launchSettings.json" ]; then
    if grep -q "5001" "src/PimFlow.Server/Properties/launchSettings.json"; then
        report_success "Puerto 5001 configurado en launchSettings.json"
    else
        report_warning "Puerto 5001 no encontrado en launchSettings.json"
    fi
else
    report_error "launchSettings.json no encontrado"
fi

# 6. Verificar que el proyecto principal exista
echo ""
echo "📋 Verificando Proyecto Principal..."

if [ -f "src/PimFlow.Server/PimFlow.Server.csproj" ]; then
    report_success "Proyecto principal existe: PimFlow.Server"
else
    report_error "Proyecto principal no encontrado: PimFlow.Server"
fi

# Resumen final
echo ""
echo "=============================================="
echo "📊 RESUMEN DE VALIDACIÓN:"
echo -e "${GREEN}✅ Éxitos: $SUCCESS${NC}"
echo -e "${YELLOW}⚠️  Warnings: $WARNINGS${NC}"
echo -e "${RED}❌ Errores: $ERRORS${NC}"

if [ $ERRORS -eq 0 ]; then
    echo ""
    echo -e "${GREEN}🎉 ¡Documentación de onboarding validada exitosamente!${NC}"
    exit 0
else
    echo ""
    echo -e "${RED}💥 Se encontraron errores en la documentación de onboarding${NC}"
    exit 1
fi
