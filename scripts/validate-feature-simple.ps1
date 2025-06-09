# Script de Validacion de Feature - PowerShell Simple
# Valida que la feature este lista para push

Write-Host "Iniciando validacion de feature..." -ForegroundColor Cyan
Write-Host "==================================" -ForegroundColor Cyan

# Funciones
function Write-Error-Custom($message) {
    Write-Host "ERROR: $message" -ForegroundColor Red
    exit 1
}

function Write-Success($message) {
    Write-Host "OK: $message" -ForegroundColor Green
}

function Write-Warning-Custom($message) {
    Write-Host "WARNING: $message" -ForegroundColor Yellow
}

# 1. Verificar que estamos en una feature branch
Write-Host "Verificando branch actual..." -ForegroundColor Blue
$currentBranch = git branch --show-current
if (-not $currentBranch.StartsWith("feature/")) {
    Write-Error-Custom "No estas en una feature branch. Branch actual: $currentBranch"
}
Write-Success "Branch valida: $currentBranch"

# 2. Verificar naming convention
Write-Host "Verificando naming convention..." -ForegroundColor Blue
if ($currentBranch -match "^feature/\d{4}-\d{2}-\d{2}-.+") {
    Write-Success "Naming convention correcta"
} else {
    Write-Warning-Custom "Naming convention no sigue el patron: feature/YYYY-MM-DD-descripcion"
}

# 3. Verificar que hay commits
Write-Host "Verificando commits..." -ForegroundColor Blue
$commitCount = (git rev-list --count HEAD "^develop") -as [int]
if ($commitCount -eq 0) {
    Write-Error-Custom "No hay commits en esta feature branch"
}
Write-Success "Feature tiene $commitCount commits"

# 4. Ejecutar tests
Write-Host "Ejecutando tests..." -ForegroundColor Blue
if (Get-Command dotnet -ErrorAction SilentlyContinue) {
    $testResult = dotnet test --verbosity minimal --no-build
    if ($LASTEXITCODE -ne 0) {
        Write-Error-Custom "Tests fallaron"
    }
    Write-Success "Todos los tests pasaron"
} else {
    Write-Warning-Custom "dotnet no encontrado, saltando tests"
}

# 5. Verificar build
Write-Host "Verificando build..." -ForegroundColor Blue
if (Get-Command dotnet -ErrorAction SilentlyContinue) {
    $buildResult = dotnet build --verbosity minimal
    if ($LASTEXITCODE -ne 0) {
        Write-Error-Custom "Build fallo"
    }
    Write-Success "Build exitoso"
} else {
    Write-Warning-Custom "dotnet no encontrado, saltando build"
}

# 6. Verificar arquitectura (tests criticos)
Write-Host "Verificando arquitectura..." -ForegroundColor Blue
if (Test-Path "tests/PimFlow.Architecture.Tests/") {
    $archResult = dotnet test tests/PimFlow.Architecture.Tests/ --filter "Category=Critical" --verbosity minimal
    if ($LASTEXITCODE -ne 0) {
        Write-Error-Custom "Tests criticos de arquitectura fallaron"
    }
    Write-Success "Arquitectura validada"
} else {
    Write-Warning-Custom "Tests de arquitectura no encontrados, saltando"
}

# 6. Verificar cambios no commiteados
Write-Host "Verificando cambios pendientes..." -ForegroundColor Blue
$changes = git diff-index --quiet HEAD
if ($LASTEXITCODE -ne 0) {
    Write-Error-Custom "Hay cambios no commiteados"
}
Write-Success "No hay cambios pendientes"

Write-Host ""
Write-Host "==================================" -ForegroundColor Cyan
Write-Host "Validacion completada exitosamente!" -ForegroundColor Green
Write-Host "Feature lista para push" -ForegroundColor Green
Write-Host ""
Write-Host "Para hacer push:" -ForegroundColor Blue
Write-Host "git push -u origin $currentBranch" -ForegroundColor White
