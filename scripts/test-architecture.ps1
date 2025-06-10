# Script de Tests de Arquitectura por Categorias - PowerShell
# Ejecuta diferentes tipos de tests arquitectonicos segun el contexto

param(
    [Parameter(Mandatory=$false)]
    [ValidateSet("Critical", "Aspirational", "Monitoring", "All")]
    [string]$Category = "Critical",
    
    [Parameter(Mandatory=$false)]
    [switch]$FailOnAspirationErrors = $false
)

Write-Host "Ejecutando tests de arquitectura - Categoria: $Category" -ForegroundColor Cyan
Write-Host "=======================================================" -ForegroundColor Cyan

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

function Write-Info($message) {
    Write-Host "INFO: $message" -ForegroundColor Blue
}

# Verificar estructura del proyecto
if (-not (Test-Path "PimFlow.sln")) {
    Write-Error-Custom "No se encontro PimFlow.sln. Ejecuta desde la raiz del proyecto."
}

# Compilar proyecto
Write-Info "Compilando proyecto..."
$buildResult = dotnet build --verbosity minimal
if ($LASTEXITCODE -ne 0) {
    Write-Error-Custom "Error al compilar el proyecto"
}

# Ejecutar tests segun categoria
switch ($Category) {
    "Critical" {
        Write-Info "Ejecutando tests CRITICOS de arquitectura..."
        Write-Host "Estos tests DEBEN pasar siempre" -ForegroundColor Yellow
        
        $testResult = dotnet test tests/PimFlow.Architecture.Tests/ --filter "Category=Critical" --verbosity normal
        if ($LASTEXITCODE -ne 0) {
            Write-Error-Custom "Tests criticos de arquitectura fallaron. Esto bloquea el desarrollo."
        }
        Write-Success "Todos los tests criticos pasaron"
    }
    
    "Aspirational" {
        Write-Info "Ejecutando tests ASPIRACIONALES de arquitectura..."
        Write-Host "Estos tests representan objetivos arquitectonicos" -ForegroundColor Yellow
        
        $testResult = dotnet test tests/PimFlow.Architecture.Tests/ --filter "Category=Aspirational" --verbosity normal
        if ($LASTEXITCODE -ne 0) {
            if ($FailOnAspirationErrors) {
                Write-Error-Custom "Tests aspiracionales fallaron y FailOnAspirationErrors esta habilitado"
            } else {
                Write-Warning-Custom "Tests aspiracionales fallaron - esto indica areas de mejora"
                Write-Host "Usa -FailOnAspirationErrors para hacer estos tests obligatorios" -ForegroundColor Gray
            }
        } else {
            Write-Success "Todos los tests aspiracionales pasaron - excelente arquitectura!"
        }
    }
    
    "Monitoring" {
        Write-Info "Ejecutando tests de MONITOREO de arquitectura..."
        Write-Host "Estos tests miden metricas arquitectonicas" -ForegroundColor Yellow
        
        $testResult = dotnet test tests/PimFlow.Architecture.Tests/ --filter "Category=Monitoring" --verbosity normal
        if ($LASTEXITCODE -ne 0) {
            Write-Warning-Custom "Metricas arquitectonicas fuera de rango - considera mejoras"
        } else {
            Write-Success "Metricas arquitectonicas dentro de rangos aceptables"
        }
    }
    
    "All" {
        Write-Info "Ejecutando TODOS los tests de arquitectura..."
        
        # Criticos (deben pasar)
        Write-Host "`n=== TESTS CRITICOS ===" -ForegroundColor Red
        $criticalResult = dotnet test tests/PimFlow.Architecture.Tests/ --filter "Category=Critical" --verbosity normal
        $criticalPassed = ($LASTEXITCODE -eq 0)
        
        # Aspiracionales (pueden fallar)
        Write-Host "`n=== TESTS ASPIRACIONALES ===" -ForegroundColor Yellow
        $aspirationalResult = dotnet test tests/PimFlow.Architecture.Tests/ --filter "Category=Aspirational" --verbosity normal
        $aspirationalPassed = ($LASTEXITCODE -eq 0)
        
        # Monitoreo (informativos)
        Write-Host "`n=== TESTS DE MONITOREO ===" -ForegroundColor Blue
        $monitoringResult = dotnet test tests/PimFlow.Architecture.Tests/ --filter "Category=Monitoring" --verbosity normal
        $monitoringPassed = ($LASTEXITCODE -eq 0)
        
        # Resumen
        Write-Host "`n=== RESUMEN ===" -ForegroundColor Cyan
        if ($criticalPassed) {
            Write-Success "Tests criticos: PASARON"
        } else {
            Write-Host "Tests criticos: FALLARON" -ForegroundColor Red
        }
        
        if ($aspirationalPassed) {
            Write-Success "Tests aspiracionales: PASARON"
        } else {
            Write-Warning-Custom "Tests aspiracionales: FALLARON (areas de mejora)"
        }
        
        if ($monitoringPassed) {
            Write-Success "Tests de monitoreo: PASARON"
        } else {
            Write-Warning-Custom "Tests de monitoreo: FALLARON (metricas fuera de rango)"
        }
        
        # Solo fallar si los criticos fallan
        if (-not $criticalPassed) {
            Write-Error-Custom "Tests criticos fallaron - desarrollo bloqueado"
        }
    }
}

Write-Host ""
Write-Host "=======================================================" -ForegroundColor Cyan
Write-Host "Tests de arquitectura completados" -ForegroundColor Cyan
Write-Host ""

# Mostrar guia de uso
Write-Host "GUIA DE USO:" -ForegroundColor Yellow
Write-Host "  Critical:      ./scripts/test-architecture.ps1 -Category Critical" -ForegroundColor White
Write-Host "  Aspirational:  ./scripts/test-architecture.ps1 -Category Aspirational" -ForegroundColor White
Write-Host "  Monitoring:    ./scripts/test-architecture.ps1 -Category Monitoring" -ForegroundColor White
Write-Host "  All:           ./scripts/test-architecture.ps1 -Category All" -ForegroundColor White
Write-Host ""
Write-Host "INTEGRACION EN GIT FLOW:" -ForegroundColor Yellow
Write-Host "  Pre-push:      Tests Critical (obligatorios)" -ForegroundColor White
Write-Host "  Pre-merge:     Tests Critical + Aspirational" -ForegroundColor White
Write-Host "  CI/CD:         Tests All (con reportes)" -ForegroundColor White
