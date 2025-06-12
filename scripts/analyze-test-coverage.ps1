#!/usr/bin/env pwsh

Write-Host "📊 Análisis de Cobertura de Tests vs Código Fuente" -ForegroundColor Cyan

# Función para contar líneas de código (excluyendo comentarios y líneas vacías)
function Count-LinesOfCode($path) {
    $files = Get-ChildItem $path -Recurse -Include "*.cs"
    $totalLines = 0
    $codeLines = 0
    
    foreach ($file in $files) {
        $content = Get-Content $file.FullName
        $totalLines += $content.Count
        
        # Contar líneas que no son comentarios ni vacías
        $codeLines += ($content | Where-Object { 
            $_.Trim() -ne "" -and 
            -not $_.Trim().StartsWith("//") -and 
            -not $_.Trim().StartsWith("/*") -and 
            -not $_.Trim().StartsWith("*") -and
            -not $_.Trim().StartsWith("using ") -and
            -not $_.Trim() -eq "{" -and
            -not $_.Trim() -eq "}"
        }).Count
    }
    
    return @{
        Files = $files.Count
        TotalLines = $totalLines
        CodeLines = $codeLines
    }
}

# Función para contar tests
function Count-Tests($path) {
    $files = Get-ChildItem $path -Recurse -Include "*.cs" -ErrorAction SilentlyContinue
    $testCount = 0
    
    foreach ($file in $files) {
        $content = Get-Content $file.FullName -Raw
        $testCount += ([regex]::Matches($content, '\[Fact\]|\[Theory\]')).Count
    }
    
    return @{
        Files = $files.Count
        Tests = $testCount
    }
}

Write-Host "`n🔍 ANÁLISIS POR PROYECTO:" -ForegroundColor Yellow

# Analizar cada proyecto de código fuente
$projects = @(
    @{ Name = "Domain"; Path = "src/PimFlow.Domain" },
    @{ Name = "Server"; Path = "src/PimFlow.Server" },
    @{ Name = "Client"; Path = "src/PimFlow.Client" },
    @{ Name = "Shared"; Path = "src/PimFlow.Shared" },
    @{ Name = "Contracts"; Path = "src/PimFlow.Contracts" }
)

$testProjects = @(
    @{ Name = "Domain.Tests"; Path = "tests/PimFlow.Domain.Tests" },
    @{ Name = "Server.Tests"; Path = "tests/PimFlow.Server.Tests" },
    @{ Name = "Shared.Tests"; Path = "tests/PimFlow.Shared.Tests" },
    @{ Name = "Architecture.Tests"; Path = "tests/PimFlow.Architecture.Tests" }
)

$totalSourceFiles = 0
$totalSourceLines = 0
$totalCodeLines = 0
$totalTestFiles = 0
$totalTests = 0

Write-Host "`n📁 CÓDIGO FUENTE:" -ForegroundColor Green

foreach ($project in $projects) {
    if (Test-Path $project.Path) {
        $stats = Count-LinesOfCode $project.Path
        $totalSourceFiles += $stats.Files
        $totalSourceLines += $stats.TotalLines
        $totalCodeLines += $stats.CodeLines
        
        Write-Host "  $($project.Name):" -ForegroundColor White
        Write-Host "    Archivos: $($stats.Files)" -ForegroundColor Gray
        Write-Host "    Líneas totales: $($stats.TotalLines)" -ForegroundColor Gray
        Write-Host "    Líneas de código: $($stats.CodeLines)" -ForegroundColor Gray
    }
}

Write-Host "`n🧪 TESTS:" -ForegroundColor Blue

foreach ($testProject in $testProjects) {
    if (Test-Path $testProject.Path) {
        $stats = Count-Tests $testProject.Path
        $totalTestFiles += $stats.Files
        $totalTests += $stats.Tests
        
        Write-Host "  $($testProject.Name):" -ForegroundColor White
        Write-Host "    Archivos: $($stats.Files)" -ForegroundColor Gray
        Write-Host "    Tests: $($stats.Tests)" -ForegroundColor Gray
    }
}

Write-Host "`n📊 RESUMEN GENERAL:" -ForegroundColor Cyan
Write-Host "  Archivos de código fuente: $totalSourceFiles" -ForegroundColor White
Write-Host "  Líneas totales de código: $totalSourceLines" -ForegroundColor White
Write-Host "  Líneas efectivas de código: $totalCodeLines" -ForegroundColor White
Write-Host "  Archivos de tests: $totalTestFiles" -ForegroundColor White
Write-Host "  Número total de tests: $totalTests" -ForegroundColor White

# Calcular ratios
$testToSourceRatio = [math]::Round($totalTests / $totalSourceFiles, 2)
$testToCodeLinesRatio = [math]::Round($totalTests / $totalCodeLines * 100, 2)
$testFilesToSourceRatio = [math]::Round($totalTestFiles / $totalSourceFiles, 2)

Write-Host "`n📈 MÉTRICAS DE COBERTURA:" -ForegroundColor Yellow
Write-Host "  Tests por archivo de código: $testToSourceRatio" -ForegroundColor White
Write-Host "  Tests por 100 líneas de código: $testToCodeLinesRatio" -ForegroundColor White
Write-Host "  Archivos de test por archivo de código: $testFilesToSourceRatio" -ForegroundColor White

Write-Host "`n🎯 EVALUACIÓN:" -ForegroundColor Magenta

# Benchmarks de la industria
if ($testToCodeLinesRatio -lt 5) {
    Write-Host "  📉 BAJA cobertura de tests (< 5 tests por 100 líneas)" -ForegroundColor Red
} elseif ($testToCodeLinesRatio -lt 15) {
    Write-Host "  📊 NORMAL cobertura de tests (5-15 tests por 100 líneas)" -ForegroundColor Green
} elseif ($testToCodeLinesRatio -lt 25) {
    Write-Host "  📈 ALTA cobertura de tests (15-25 tests por 100 líneas)" -ForegroundColor Yellow
} else {
    Write-Host "  🔥 MUY ALTA cobertura de tests (> 25 tests por 100 líneas)" -ForegroundColor Red
}

if ($testToSourceRatio -lt 2) {
    Write-Host "  📉 POCOS tests por archivo (< 2)" -ForegroundColor Red
} elseif ($testToSourceRatio -lt 5) {
    Write-Host "  📊 NORMAL cantidad de tests por archivo (2-5)" -ForegroundColor Green
} elseif ($testToSourceRatio -lt 8) {
    Write-Host "  📈 MUCHOS tests por archivo (5-8)" -ForegroundColor Yellow
} else {
    Write-Host "  🔥 EXCESIVOS tests por archivo (> 8)" -ForegroundColor Red
}

Write-Host "`n💡 RECOMENDACIONES:" -ForegroundColor Cyan
Write-Host "  • Proyectos pequeños-medianos: 3-7 tests por archivo" -ForegroundColor White
Write-Host "  • Proyectos enterprise: 5-15 tests por archivo" -ForegroundColor White
Write-Host "  • Cobertura óptima: 10-20 tests por 100 líneas de código" -ForegroundColor White
Write-Host "  • Foco en calidad sobre cantidad" -ForegroundColor White
