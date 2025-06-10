#!/usr/bin/env pwsh

# Script maestro para renombrar el proyecto completo
# Cambia el nombre en todos los archivos críticos e importantes

param(
    [Parameter(Mandatory=$true)]
    [string]$NewProjectName,
    
    [string]$OldProjectName = "PimFlow",
    [string]$ProjectRoot = ".",
    [switch]$DryRun = $false
)

Write-Host "RENOMBRANDO PROYECTO" -ForegroundColor Green
Write-Host "De: $OldProjectName" -ForegroundColor Red
Write-Host "A: $NewProjectName" -ForegroundColor Green
Write-Host "Modo: $(if ($DryRun) { 'DRY RUN' } else { 'APLICAR CAMBIOS' })" -ForegroundColor Yellow

Set-Location $ProjectRoot

# FASE 1: Archivos críticos (.sln, .csproj)
Write-Host "`nFASE 1: Renombrando archivos criticos..." -ForegroundColor Cyan

$criticalFiles = @(
    "PimFlow.sln",
    "src\PimFlow.Domain\PimFlow.Domain.csproj",
    "src\PimFlow.Shared\PimFlow.Shared.csproj", 
    "src\PimFlow.Client\PimFlow.Client.csproj",
    "src\PimFlow.Server\PimFlow.Server.csproj",
    "tests\PimFlow.Domain.Tests\PimFlow.Domain.Tests.csproj",
    "tests\PimFlow.Shared.Tests\PimFlow.Shared.Tests.csproj",
    "tests\PimFlow.Server.Tests\PimFlow.Server.Tests.csproj",
    "tests\PimFlow.Architecture.Tests\PimFlow.Architecture.Tests.csproj"
)

foreach ($file in $criticalFiles) {
    if (Test-Path $file) {
        $content = Get-Content $file -Raw
        $newContent = $content -replace $OldProjectName, $NewProjectName
        
        if ($DryRun) {
            Write-Host "SERIA RENOMBRADO: $file" -ForegroundColor Yellow
        } else {
            Set-Content $file -Value $newContent -NoNewline
            Write-Host "RENOMBRADO: $file" -ForegroundColor Green
        }
    }
}

# FASE 2: Configuración y documentación
Write-Host "`nFASE 2: Actualizando configuracion..." -ForegroundColor Cyan

$configFiles = @(
    "README.md",
    "docker-compose.yml",
    "src\PimFlow.Server\web.config",
    "src\PimFlow.Server\Configuration\ApplicationSettings.cs"
)

foreach ($file in $configFiles) {
    if (Test-Path $file) {
        $content = Get-Content $file -Raw
        $newContent = $content -replace $OldProjectName, $NewProjectName
        
        if ($DryRun) {
            Write-Host "SERIA ACTUALIZADO: $file" -ForegroundColor Yellow
        } else {
            Set-Content $file -Value $newContent -NoNewline
            Write-Host "ACTUALIZADO: $file" -ForegroundColor Green
        }
    }
}

# FASE 3: Renombrar directorios
Write-Host "`nFASE 3: Renombrando directorios..." -ForegroundColor Cyan

$directoriesToRename = @(
    @{ Old = "src\PimFlow.Domain"; New = "src\$NewProjectName.Domain" },
    @{ Old = "src\PimFlow.Shared"; New = "src\$NewProjectName.Shared" },
    @{ Old = "src\PimFlow.Client"; New = "src\$NewProjectName.Client" },
    @{ Old = "src\PimFlow.Server"; New = "src\$NewProjectName.Server" },
    @{ Old = "src\PimFlow.Contracts"; New = "src\$NewProjectName.Contracts" },
    @{ Old = "tests\PimFlow.Domain.Tests"; New = "tests\$NewProjectName.Domain.Tests" },
    @{ Old = "tests\PimFlow.Shared.Tests"; New = "tests\$NewProjectName.Shared.Tests" },
    @{ Old = "tests\PimFlow.Server.Tests"; New = "tests\$NewProjectName.Server.Tests" },
    @{ Old = "tests\PimFlow.Architecture.Tests"; New = "tests\$NewProjectName.Architecture.Tests" }
)

foreach ($dir in $directoriesToRename) {
    if (Test-Path $dir.Old) {
        if ($DryRun) {
            Write-Host "SERIA RENOMBRADO: $($dir.Old) -> $($dir.New)" -ForegroundColor Yellow
        } else {
            Rename-Item $dir.Old $dir.New
            Write-Host "RENOMBRADO: $($dir.Old) -> $($dir.New)" -ForegroundColor Green
        }
    }
}

# FASE 4: Actualizar referencias en archivos importantes
Write-Host "`nFASE 4: Actualizando referencias en archivos importantes..." -ForegroundColor Cyan

$importantFiles = Get-ChildItem -Recurse -File | Where-Object { 
    $_.Extension -match "\.(cs|razor)$" -and
    $_.FullName -notmatch "\\bin\\" -and 
    $_.FullName -notmatch "\\obj\\" -and
    ($_.FullName -match "\\Controllers\\" -or 
     $_.FullName -match "\\Services\\" -or
     $_.FullName -match "\\Pages\\" -or
     $_.Name -match "(Program\.cs|DbContext)")
}

$updatedFiles = 0
foreach ($file in $importantFiles) {
    try {
        $content = Get-Content $file.FullName -Raw -ErrorAction SilentlyContinue
        if ($content -and $content -match $OldProjectName) {
            $newContent = $content -replace $OldProjectName, $NewProjectName
            
            if ($DryRun) {
                $updatedFiles++
            } else {
                Set-Content $file.FullName -Value $newContent -NoNewline
                $updatedFiles++
            }
        }
    }
    catch {
        Write-Warning "Error procesando $($file.FullName)"
    }
}

Write-Host "Archivos importantes actualizados: $updatedFiles" -ForegroundColor Cyan

# FASE 5: Renombrar archivo .sln
Write-Host "`nFASE 5: Renombrando archivo de solucion..." -ForegroundColor Cyan

if (Test-Path "$OldProjectName.sln") {
    if ($DryRun) {
        Write-Host "SERIA RENOMBRADO: $OldProjectName.sln -> $NewProjectName.sln" -ForegroundColor Yellow
    } else {
        Rename-Item "$OldProjectName.sln" "$NewProjectName.sln"
        Write-Host "RENOMBRADO: $OldProjectName.sln -> $NewProjectName.sln" -ForegroundColor Green
    }
}

Write-Host "`nRENOMBRADO COMPLETADO!" -ForegroundColor Green
Write-Host "Proyecto renombrado de '$OldProjectName' a '$NewProjectName'" -ForegroundColor Cyan

if ($DryRun) {
    Write-Host "`nEjecuta sin -DryRun para aplicar los cambios" -ForegroundColor Yellow
} else {
    Write-Host "`nRECOMENDACIONES POST-RENOMBRADO:" -ForegroundColor Yellow
    Write-Host "1. Ejecutar: dotnet clean && dotnet build"
    Write-Host "2. Actualizar referencias en IDE/Visual Studio"
    Write-Host "3. Verificar que todos los tests pasen"
    Write-Host "4. Actualizar repositorio Git si es necesario"
}

return @{
    OldName = $OldProjectName
    NewName = $NewProjectName
    Success = $true
    DryRun = $DryRun
}
