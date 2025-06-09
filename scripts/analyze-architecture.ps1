# Script de Analisis de Arquitectura - PowerShell
# Ejecuta tests de acoplamiento y genera reporte

Write-Host "Analizando arquitectura del proyecto PimFlow..." -ForegroundColor Cyan
Write-Host "=============================================" -ForegroundColor Cyan

# Funcion para mostrar errores
function Write-Error-Custom($message) {
    Write-Host "ERROR: $message" -ForegroundColor Red
    exit 1
}

# Funcion para mostrar exito
function Write-Success($message) {
    Write-Host "OK: $message" -ForegroundColor Green
}

# Funcion para mostrar informacion
function Write-Info($message) {
    Write-Host "INFO: $message" -ForegroundColor Blue
}

# 1. Verificar que estamos en el directorio correcto
Write-Info "Verificando estructura del proyecto..."
if (-not (Test-Path "PimFlow.sln")) {
    Write-Error-Custom "No se encontro PimFlow.sln. Ejecuta desde la raiz del proyecto."
}
Write-Success "Estructura del proyecto correcta"

# 2. Compilar el proyecto
Write-Info "Compilando proyecto..."
$buildResult = dotnet build --verbosity minimal
if ($LASTEXITCODE -ne 0) {
    Write-Error-Custom "Error al compilar el proyecto"
}
Write-Success "Proyecto compilado exitosamente"

# 3. Ejecutar tests de arquitectura
Write-Info "Ejecutando tests de arquitectura..."
$testResult = dotnet test tests/PimFlow.Architecture.Tests/ --verbosity normal --logger "console;verbosity=detailed"
if ($LASTEXITCODE -ne 0) {
    Write-Host "ADVERTENCIA: Algunos tests de arquitectura fallaron" -ForegroundColor Yellow
    Write-Host "Esto indica posibles problemas de acoplamiento" -ForegroundColor Yellow
} else {
    Write-Success "Todos los tests de arquitectura pasaron"
}

# 4. Analizar dependencias entre proyectos
Write-Info "Analizando dependencias entre proyectos..."

$projects = @(
    "src/PimFlow.Domain/PimFlow.Domain.csproj",
    "src/PimFlow.Shared/PimFlow.Shared.csproj", 
    "src/PimFlow.Server/PimFlow.Server.csproj",
    "src/PimFlow.Client/PimFlow.Client.csproj"
)

foreach ($project in $projects) {
    if (Test-Path $project) {
        Write-Host "Proyecto: $project" -ForegroundColor White
        $content = Get-Content $project
        $references = $content | Select-String "ProjectReference Include"
        if ($references) {
            foreach ($ref in $references) {
                Write-Host "  -> $($ref.Line.Trim())" -ForegroundColor Gray
            }
        } else {
            Write-Host "  -> Sin referencias a otros proyectos" -ForegroundColor Gray
        }
    }
}

# 5. Contar archivos que contienen el nombre del proyecto
Write-Info "Analizando impacto potencial de rename..."
$projectName = "PimFlow"
$allFiles = Get-ChildItem -Recurse -File | Where-Object { 
    $_.FullName -notmatch "\\bin\\" -and 
    $_.FullName -notmatch "\\obj\\" -and 
    $_.FullName -notmatch "\\.git\\" -and
    $_.Extension -match "\.(cs|csproj|json|md|sln)$"
}

$filesWithProjectName = @()
foreach ($file in $allFiles) {
    try {
        $content = Get-Content $file.FullName -Raw -ErrorAction SilentlyContinue
        if ($content -and $content -match $projectName) {
            $filesWithProjectName += $file.FullName
        }
    }
    catch {
        # Ignorar archivos que no se pueden leer
    }
}

Write-Host ""
Write-Host "ANALISIS DE IMPACTO DE RENAME:" -ForegroundColor Yellow
Write-Host "Archivos que contienen '$projectName': $($filesWithProjectName.Count)" -ForegroundColor White

if ($filesWithProjectName.Count -gt 30) {
    Write-Host "ADVERTENCIA: Demasiados archivos afectados por rename" -ForegroundColor Red
    Write-Host "Considera implementar las mejoras arquitectonicas propuestas" -ForegroundColor Red
} else {
    Write-Success "Impacto de rename dentro de limites aceptables"
}

# Mostrar algunos archivos afectados
Write-Host ""
Write-Host "Primeros 10 archivos que serian afectados:" -ForegroundColor White
$filesWithProjectName | Select-Object -First 10 | ForEach-Object {
    $relativePath = Resolve-Path $_ -Relative
    Write-Host "  $relativePath" -ForegroundColor Gray
}

# 6. Analizar servicios y sus responsabilidades
Write-Info "Analizando responsabilidades de servicios..."
$serviceFiles = Get-ChildItem -Path "src/PimFlow.Server/Services" -Filter "*Service.cs" -Recurse

foreach ($serviceFile in $serviceFiles) {
    $content = Get-Content $serviceFile.FullName
    $publicMethods = $content | Select-String "public.*async.*Task.*\(" | Measure-Object
    $className = $serviceFile.BaseName
    
    Write-Host "Servicio: $className" -ForegroundColor White
    Write-Host "  Metodos publicos: $($publicMethods.Count)" -ForegroundColor Gray
    
    if ($publicMethods.Count -gt 10) {
        Write-Host "  ADVERTENCIA: Demasiadas responsabilidades" -ForegroundColor Yellow
    }
}

Write-Host ""
Write-Host "=============================================" -ForegroundColor Cyan
Write-Host "Analisis de arquitectura completado" -ForegroundColor Cyan
Write-Host ""
Write-Host "RECOMENDACIONES:" -ForegroundColor Yellow
Write-Host "1. Ejecutar tests de arquitectura regularmente" -ForegroundColor White
Write-Host "2. Mantener bajo el numero de archivos afectados por rename" -ForegroundColor White
Write-Host "3. Implementar AutoMapper para reducir acoplamiento" -ForegroundColor White
Write-Host "4. Considerar CQRS para servicios con muchas responsabilidades" -ForegroundColor White
Write-Host ""
Write-Host "Para mas detalles, ver: docs/architecture-improvements.md" -ForegroundColor Blue
