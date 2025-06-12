#!/usr/bin/env pwsh

<#
.SYNOPSIS
    Actualiza los namespaces del dominio después de la refactorización por features
.DESCRIPTION
    Este script actualiza automáticamente todas las referencias a los namespaces antiguos
    del dominio (PimFlow.Domain.Entities, etc.) por los nuevos namespaces por feature
.PARAMETER DryRun
    Si se especifica, solo muestra qué cambios se harían sin aplicarlos
#>

param(
    [switch]$DryRun
)

# Configuración de colores
function Write-Info($message) { Write-Host $message -ForegroundColor Cyan }
function Write-Success($message) { Write-Host $message -ForegroundColor Green }
function Write-Warning($message) { Write-Host $message -ForegroundColor Yellow }
function Write-Error($message) { Write-Host $message -ForegroundColor Red }

Write-Info "🔄 Actualizando namespaces del dominio después de refactorización por features"
if ($DryRun) {
    Write-Warning "⚠️  MODO DRY RUN - Solo se mostrarán los cambios sin aplicarlos"
}

# Mapeo de namespaces antiguos a nuevos
$namespaceMapping = @{
    'using PimFlow\.Domain\.Entities;' = @(
        'using PimFlow.Domain.Article;',
        'using PimFlow.Domain.Category;', 
        'using PimFlow.Domain.User;',
        'using PimFlow.Domain.CustomAttribute;'
    )
    'using PimFlow\.Domain\.Enums;' = @(
        'using PimFlow.Domain.Article.Enums;',
        'using PimFlow.Domain.CustomAttribute.Enums;'
    )
    'using PimFlow\.Domain\.ValueObjects;' = @(
        'using PimFlow.Domain.Article.ValueObjects;',
        'using PimFlow.Domain.Category.ValueObjects;',
        'using PimFlow.Domain.User.ValueObjects;'
    )
    'using PimFlow\.Domain\.Events;' = 'using PimFlow.Domain.Common;'
    'using PimFlow\.Domain\.Specifications;' = 'using PimFlow.Domain.Common;'
}

# Archivos a procesar
$filesToProcess = Get-ChildItem -Path "src/PimFlow.Server" -Recurse -Include "*.cs" | Where-Object {
    $_.FullName -notmatch "\\bin\\" -and 
    $_.FullName -notmatch "\\obj\\"
}

$processedFiles = 0
$totalReplacements = 0

foreach ($file in $filesToProcess) {
    try {
        $content = Get-Content $file.FullName -Raw -ErrorAction SilentlyContinue
        if (-not $content) { continue }
        
        $originalContent = $content
        $fileReplacements = 0
        $hasChanges = $false
        
        # Verificar si el archivo necesita actualizaciones
        $needsUpdate = $false
        foreach ($oldPattern in $namespaceMapping.Keys) {
            if ($content -match $oldPattern) {
                $needsUpdate = $true
                break
            }
        }
        
        if (-not $needsUpdate) { continue }
        
        # Aplicar reemplazos
        foreach ($oldPattern in $namespaceMapping.Keys) {
            $newNamespaces = $namespaceMapping[$oldPattern]
            
            if ($content -match $oldPattern) {
                if ($newNamespaces -is [array]) {
                    # Múltiples namespaces de reemplazo
                    $replacement = $newNamespaces -join "`n"
                } else {
                    # Un solo namespace de reemplazo
                    $replacement = $newNamespaces
                }
                
                $content = $content -replace $oldPattern, $replacement
                $fileReplacements++
                $hasChanges = $true
            }
        }
        
        # Limpiar imports duplicados
        if ($hasChanges) {
            $lines = $content -split "`n"
            $usingLines = @()
            $otherLines = @()
            $inUsings = $true
            
            foreach ($line in $lines) {
                if ($line -match "^using " -and $inUsings) {
                    if ($usingLines -notcontains $line.Trim()) {
                        $usingLines += $line.Trim()
                    }
                } else {
                    if ($line.Trim() -eq "" -and $inUsings -and $usingLines.Count -gt 0) {
                        $inUsings = $false
                        $otherLines += ""
                    } elseif (-not $inUsings -or $line.Trim() -ne "") {
                        $inUsings = $false
                        $otherLines += $line
                    }
                }
            }
            
            # Reconstruir el archivo
            $content = ($usingLines + "" + $otherLines) -join "`n"
        }
        
        if ($hasChanges) {
            $relativePath = $file.FullName.Replace((Get-Location).Path, "").TrimStart('\', '/')
            
            if ($DryRun) {
                Write-Warning "SERIA MODIFICADO: $relativePath ($fileReplacements reemplazos)"
            } else {
                Set-Content $file.FullName -Value $content -NoNewline
                Write-Success "MODIFICADO: $relativePath ($fileReplacements reemplazos)"
            }
            
            $processedFiles++
            $totalReplacements += $fileReplacements
        }
    }
    catch {
        Write-Error "Error procesando $($file.FullName): $($_.Exception.Message)"
    }
}

# Resumen
Write-Info "`n📊 Resumen de la actualización:"
Write-Host "  Archivos procesados: $processedFiles" -ForegroundColor White
Write-Host "  Total de reemplazos: $totalReplacements" -ForegroundColor White

if ($DryRun) {
    Write-Warning "`n⚠️  Para aplicar los cambios, ejecuta el script sin el parámetro -DryRun"
} else {
    Write-Success "`n✅ Actualización de namespaces completada"
}
