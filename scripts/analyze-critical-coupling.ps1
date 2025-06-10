#!/usr/bin/env pwsh

# Script para analizar acoplamiento crítico vs. no crítico
# Identifica qué archivos realmente necesitan cambios para renombrar el proyecto

param(
    [string]$ProjectName = "PimFlow",
    [string]$ProjectRoot = "."
)

Write-Host "Analizando acoplamiento critico para: $ProjectName" -ForegroundColor Green

Set-Location $ProjectRoot

# Categorías de criticidad
$criticalFiles = @()      # Archivos que DEBEN cambiar para renombrar
$importantFiles = @()     # Archivos importantes pero no críticos
$implementationFiles = @() # Archivos de implementación (menos críticos)

# Obtener todos los archivos con el nombre del proyecto
$allFiles = Get-ChildItem -Recurse -File | Where-Object { 
    $_.FullName -notmatch "\\bin\\" -and 
    $_.FullName -notmatch "\\obj\\" -and 
    $_.FullName -notmatch "\\.git\\" -and
    $_.Extension -match "\.(cs|csproj|json|md|sln|ps1|sh|bat|yml|yaml|razor|txt|config)$"
}

foreach ($file in $allFiles) {
    try {
        $content = Get-Content $file.FullName -Raw -ErrorAction SilentlyContinue
        if ($content -and $content -match $ProjectName) {
            $relativePath = $file.FullName.Replace((Get-Location).Path, "").TrimStart('\', '/')
            $occurrences = ($content | Select-String -Pattern $ProjectName -AllMatches).Matches.Count
            
            $fileInfo = @{
                Path = $relativePath
                Extension = $file.Extension
                Occurrences = $occurrences
                Category = ""
            }
            
            # Clasificar por criticidad
            if ($file.Extension -eq ".sln" -or $file.Extension -eq ".csproj") {
                $fileInfo.Category = "CRITICAL"
                $criticalFiles += $fileInfo
            }
            elseif ($file.Name -match "(Program\.cs|Startup\.cs|appsettings|web\.config|docker|README)" -or
                    $file.Extension -match "\.(json|yml|yaml|md|config)$") {
                $fileInfo.Category = "IMPORTANT"
                $importantFiles += $fileInfo
            }
            elseif ($file.FullName -match "\\Controllers\\" -or 
                    $file.FullName -match "\\Services\\" -or
                    $file.FullName -match "\\Pages\\" -or
                    $file.Name -match "(DbContext|Repository)" -or
                    $content -match "namespace.*$ProjectName") {
                $fileInfo.Category = "IMPORTANT"
                $importantFiles += $fileInfo
            }
            else {
                $fileInfo.Category = "IMPLEMENTATION"
                $implementationFiles += $fileInfo
            }
        }
    }
    catch {
        # Ignorar archivos que no se pueden leer
    }
}

# Mostrar resultados
Write-Host "`nRESULTADOS DEL ANALISIS CRITICO" -ForegroundColor Green
Write-Host "=" * 50

Write-Host "`nARCHIVOS CRITICOS (deben cambiar): $($criticalFiles.Count)" -ForegroundColor Red
foreach ($file in $criticalFiles | Sort-Object Path) {
    Write-Host "   • $($file.Path) ($($file.Occurrences) ocurrencias)" -ForegroundColor Red
}

Write-Host "`nARCHIVOS IMPORTANTES (recomendado cambiar): $($importantFiles.Count)" -ForegroundColor Yellow
foreach ($file in $importantFiles | Sort-Object Path) {
    Write-Host "   • $($file.Path) ($($file.Occurrences) ocurrencias)" -ForegroundColor Yellow
}

Write-Host "`nARCHIVOS DE IMPLEMENTACION (opcional): $($implementationFiles.Count)" -ForegroundColor Gray
$implementationFiles | Sort-Object Path | Select-Object -First 10 | ForEach-Object {
    Write-Host "   • $($_.Path) ($($_.Occurrences) ocurrencias)" -ForegroundColor Gray
}
if ($implementationFiles.Count -gt 10) {
    Write-Host "   ... y $($implementationFiles.Count - 10) archivos mas" -ForegroundColor Gray
}

# Calcular objetivos realistas
$totalFiles = $criticalFiles.Count + $importantFiles.Count + $implementationFiles.Count
$realisticTarget = $criticalFiles.Count + $importantFiles.Count

Write-Host "`nOBJETIVOS REALISTAS:" -ForegroundColor Cyan
Write-Host "- Total actual: $totalFiles archivos" -ForegroundColor White
Write-Host "- Criticos + Importantes: $realisticTarget archivos" -ForegroundColor White
Write-Host "- Solo criticos: $($criticalFiles.Count) archivos" -ForegroundColor White

if ($realisticTarget -le 50) {
    Write-Host "- OBJETIVO REALISTA: $realisticTarget archivos (ALCANZABLE)" -ForegroundColor Green
} elseif ($realisticTarget -le 80) {
    Write-Host "- OBJETIVO REALISTA: $realisticTarget archivos (DESAFIANTE)" -ForegroundColor Yellow
} else {
    Write-Host "- OBJETIVO REALISTA: $realisticTarget archivos (DIFICIL)" -ForegroundColor Red
}

Write-Host "`nRECOMENDACIONES:" -ForegroundColor Green
Write-Host "1. FASE 1: Configurar archivos criticos ($($criticalFiles.Count) archivos)"
Write-Host "2. FASE 2: Configurar archivos importantes ($($importantFiles.Count) archivos)"
Write-Host "3. FASE 3: Considerar archivos de implementacion ($($implementationFiles.Count) archivos)"

return @{
    TotalFiles = $totalFiles
    CriticalFiles = $criticalFiles.Count
    ImportantFiles = $importantFiles.Count
    ImplementationFiles = $implementationFiles.Count
    RealisticTarget = $realisticTarget
    IsRealistic = ($realisticTarget -le 50)
}
