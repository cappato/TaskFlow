# Script de Validacion de Feature - PowerShell
# Valida que la feature este lista para push

Write-Host "Iniciando validacion de feature..." -ForegroundColor Cyan
Write-Host "==================================" -ForegroundColor Cyan

# Funcion para mostrar errores
function Write-Error-Custom($message) {
    Write-Host "ERROR: $message" -ForegroundColor Red
    exit 1
}

# Funcion para mostrar exito
function Write-Success($message) {
    Write-Host "OK: $message" -ForegroundColor Green
}

# Funcion para mostrar advertencias
function Write-Warning-Custom($message) {
    Write-Host "WARNING: $message" -ForegroundColor Yellow
}

# 1. Verificar que estamos en una feature branch
Write-Host "Verificando branch actual..." -ForegroundColor Blue
$currentBranch = git branch --show-current
if (-not $currentBranch.StartsWith("feature/")) {
    Write-Error-Custom "No estás en una feature branch. Branch actual: $currentBranch"
}
Write-Success "Branch válida: $currentBranch"

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
Write-Host "🧪 Ejecutando tests..." -ForegroundColor Blue
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
Write-Host "🔧 Verificando build..." -ForegroundColor Blue
if (Get-Command dotnet -ErrorAction SilentlyContinue) {
    $buildResult = dotnet build --verbosity minimal
    if ($LASTEXITCODE -ne 0) {
        Write-Error-Custom "Build falló"
    }
    Write-Success "Build exitoso"
} else {
    Write-Warning-Custom "dotnet no encontrado, saltando build"
}

# 6. Verificar commits convencionales (básico)
Write-Host "💬 Verificando commits convencionales..." -ForegroundColor Blue
$commits = git log --oneline develop..HEAD
$nonConventional = 0
foreach ($commit in $commits) {
    if ($commit -notmatch "^[a-f0-9]+ (feat|fix|docs|test|refactor|style|chore)(\(.+\))?: .+") {
        $nonConventional++
    }
}
if ($nonConventional -gt 0) {
    Write-Warning-Custom "$nonConventional commits no siguen conventional commits"
} else {
    Write-Success "Todos los commits siguen conventional commits"
}

# 7. Verificar archivos no trackeados
Write-Host "📁 Verificando archivos no trackeados..." -ForegroundColor Blue
$untracked = (git ls-files --others --exclude-standard).Count
if ($untracked -gt 0) {
    Write-Warning-Custom "Hay $untracked archivos no trackeados"
}

# 8. Verificar cambios no commiteados
Write-Host "📝 Verificando cambios pendientes..." -ForegroundColor Blue
$changes = git diff-index --quiet HEAD
if ($LASTEXITCODE -ne 0) {
    Write-Error-Custom "Hay cambios no commiteados"
}
Write-Success "No hay cambios pendientes"

Write-Host ""
Write-Host "==================================" -ForegroundColor Cyan
Write-Host "🎉 Validación completada exitosamente!" -ForegroundColor Green
Write-Host "✅ Feature lista para push" -ForegroundColor Green
Write-Host ""
Write-Host "Para hacer push:" -ForegroundColor Blue
Write-Host "git push -u origin $currentBranch" -ForegroundColor White
