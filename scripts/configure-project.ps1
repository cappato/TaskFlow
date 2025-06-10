#!/usr/bin/env pwsh

# Script para configurar el nombre del proyecto en archivos de configuración
# Reemplaza placeholders #{APPLICATION_NAME}# con el nombre real del proyecto

param(
    [Parameter(Mandatory=$true)]
    [string]$ApplicationName,
    
    [string]$ProjectRoot = "."
)

Write-Host "Configurando proyecto con nombre: $ApplicationName" -ForegroundColor Green
Write-Host "Directorio raiz: $ProjectRoot" -ForegroundColor Gray

# Cambiar al directorio del proyecto
Set-Location $ProjectRoot

# Archivos que contienen placeholders
$configFiles = @(
    "src\PimFlow.Server\appsettings.json",
    "src\PimFlow.Server\appsettings.Development.json", 
    "src\PimFlow.Server\appsettings.Production.json",
    "src\PimFlow.Server\web.config"
)

$placeholder = "#{APPLICATION_NAME}#"
$replacedFiles = 0

foreach ($file in $configFiles) {
    if (Test-Path $file) {
        try {
            $content = Get-Content $file -Raw
            if ($content -match [regex]::Escape($placeholder)) {
                $newContent = $content -replace [regex]::Escape($placeholder), $ApplicationName
                Set-Content $file -Value $newContent -NoNewline
                Write-Host "Configurado: $file" -ForegroundColor Yellow
                $replacedFiles++
            }
        }
        catch {
            Write-Warning "Error procesando $file : $($_.Exception.Message)"
        }
    }
    else {
        Write-Warning "Archivo no encontrado: $file"
    }
}

Write-Host "`nConfiguracion completada:" -ForegroundColor Green
Write-Host "- Archivos procesados: $replacedFiles" -ForegroundColor Cyan
Write-Host "- Nombre de aplicacion: $ApplicationName" -ForegroundColor Cyan

# Verificar que no queden placeholders
Write-Host "`nVerificando configuracion..." -ForegroundColor Yellow

$remainingPlaceholders = @()
foreach ($file in $configFiles) {
    if (Test-Path $file) {
        $content = Get-Content $file -Raw
        if ($content -match [regex]::Escape($placeholder)) {
            $remainingPlaceholders += $file
        }
    }
}

if ($remainingPlaceholders.Count -eq 0) {
    Write-Host "Verificacion exitosa: No quedan placeholders" -ForegroundColor Green
} else {
    Write-Warning "Placeholders restantes en: $($remainingPlaceholders -join ', ')"
}

return @{
    ApplicationName = $ApplicationName
    ProcessedFiles = $replacedFiles
    RemainingPlaceholders = $remainingPlaceholders.Count
    Success = ($remainingPlaceholders.Count -eq 0)
}
