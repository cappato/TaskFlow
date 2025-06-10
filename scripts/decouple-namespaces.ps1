#!/usr/bin/env pwsh

# Script para desacoplar namespaces del nombre del proyecto
# Reemplaza referencias hardcodeadas con configuración centralizada

param(
    [string]$ProjectRoot = ".",
    [switch]$DryRun = $false
)

Write-Host "Desacoplando namespaces del proyecto" -ForegroundColor Green
Write-Host "Directorio raiz: $ProjectRoot" -ForegroundColor Gray
Write-Host "Modo: $(if ($DryRun) { 'DRY RUN (solo mostrar cambios)' } else { 'APLICAR CAMBIOS' })" -ForegroundColor Yellow

Set-Location $ProjectRoot

# Patrones a reemplazar
$patterns = @{
    # Configuración hardcodeada
    'Application__Name=PimFlow' = 'Application__Name=${APPLICATION_NAME:-PimFlow}'
    'Name.*=.*"PimFlow"' = 'Name = ApplicationInfo.Name'
    
    # Referencias en comentarios y documentación
    'PimFlow Logo' = '#{APPLICATION_NAME}# Logo'
    'PimFlow team' = '#{APPLICATION_NAME}# team'
    'PimFlow/' = '#{APPLICATION_NAME}#/'
    
    # Referencias en URLs y paths
    'github.com/cappato/PimFlow' = 'github.com/cappato/#{APPLICATION_NAME}#'
    'PimFlow.workflows' = '#{APPLICATION_NAME}#.workflows'
}

# Archivos a procesar (excluyendo archivos esenciales)
$filesToProcess = Get-ChildItem -Recurse -File | Where-Object { 
    $_.FullName -notmatch "\\bin\\" -and 
    $_.FullName -notmatch "\\obj\\" -and 
    $_.FullName -notmatch "\\.git\\" -and
    $_.FullName -notmatch "\\node_modules\\" -and
    $_.Extension -match "\.(md|json|yml|yaml|config|txt)$" -and
    $_.Name -notmatch "\.(csproj|sln)$"  # Excluir archivos esenciales
}

$processedFiles = 0
$totalReplacements = 0

foreach ($file in $filesToProcess) {
    try {
        $content = Get-Content $file.FullName -Raw -ErrorAction SilentlyContinue
        if (-not $content) { continue }
        
        $originalContent = $content
        $fileReplacements = 0
        
        foreach ($pattern in $patterns.Keys) {
            $replacement = $patterns[$pattern]
            $matches = [regex]::Matches($content, $pattern)
            
            if ($matches.Count -gt 0) {
                $content = $content -replace $pattern, $replacement
                $fileReplacements += $matches.Count
            }
        }
        
        if ($fileReplacements -gt 0) {
            $relativePath = $file.FullName.Replace((Get-Location).Path, "").TrimStart('\', '/')
            
            if ($DryRun) {
                Write-Host "SERIA MODIFICADO: $relativePath ($fileReplacements reemplazos)" -ForegroundColor Yellow
            } else {
                Set-Content $file.FullName -Value $content -NoNewline
                Write-Host "MODIFICADO: $relativePath ($fileReplacements reemplazos)" -ForegroundColor Green
            }
            
            $processedFiles++
            $totalReplacements += $fileReplacements
        }
    }
    catch {
        Write-Warning "Error procesando $($file.FullName): $($_.Exception.Message)"
    }
}

Write-Host "`nRESUMEN:" -ForegroundColor Cyan
Write-Host "- Archivos procesados: $processedFiles" -ForegroundColor White
Write-Host "- Total de reemplazos: $totalReplacements" -ForegroundColor White

if ($DryRun) {
    Write-Host "`nEjecuta sin -DryRun para aplicar los cambios" -ForegroundColor Yellow
} else {
    Write-Host "`nDesacoplamiento completado!" -ForegroundColor Green
}

# Verificar progreso
Write-Host "`nVerificando progreso..." -ForegroundColor Yellow
$remainingFiles = Get-ChildItem -Recurse -File | Where-Object { 
    $_.FullName -notmatch "\\bin\\" -and 
    $_.FullName -notmatch "\\obj\\" -and 
    $_.FullName -notmatch "\\.git\\" -and
    $_.Extension -match "\.(md|json|yml|yaml|config|txt)$"
} | ForEach-Object {
    $content = Get-Content $_.FullName -Raw -ErrorAction SilentlyContinue
    if ($content -and $content -match "PimFlow" -and $_.Name -notmatch "\.(csproj|sln)$") {
        $_.FullName.Replace((Get-Location).Path, "").TrimStart('\', '/')
    }
} | Where-Object { $_ }

Write-Host "Archivos de configuracion restantes con 'PimFlow': $($remainingFiles.Count)" -ForegroundColor Cyan

return @{
    ProcessedFiles = $processedFiles
    TotalReplacements = $totalReplacements
    RemainingConfigFiles = $remainingFiles.Count
    Success = ($processedFiles -gt 0)
}
