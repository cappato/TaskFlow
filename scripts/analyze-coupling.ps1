#!/usr/bin/env pwsh

# Script para analizar el acoplamiento de nombres en el proyecto
# Identifica archivos que contienen "PimFlow" y los categoriza

param(
    [string]$ProjectName = "PimFlow",
    [string]$ProjectRoot = "."
)

Write-Host "Analizando acoplamiento de nombres para: $ProjectName" -ForegroundColor Cyan
Write-Host "Directorio raiz: $ProjectRoot" -ForegroundColor Gray

# Cambiar al directorio del proyecto
Set-Location $ProjectRoot

# Obtener todos los archivos relevantes
$allFiles = Get-ChildItem -Recurse -File | Where-Object { 
    $_.FullName -notmatch "\\bin\\" -and 
    $_.FullName -notmatch "\\obj\\" -and 
    $_.FullName -notmatch "\\.git\\" -and
    $_.FullName -notmatch "\\node_modules\\" -and
    $_.Extension -match "\.(cs|csproj|json|md|sln|ps1|sh|bat|yml|yaml|razor|txt|config)$"
}

Write-Host "Total de archivos a analizar: $($allFiles.Count)" -ForegroundColor Yellow

# Categorías de archivos
$categories = @{
    "Essential" = @()      # Archivos que DEBEN contener el nombre del proyecto
    "Configuration" = @()  # Archivos de configuración que podrían usar variables
    "Documentation" = @()  # Documentación que podría usar variables
    "Code" = @()          # Código que usa namespaces
    "Build" = @()         # Archivos de build/deployment
    "Tests" = @()         # Archivos de tests
}

$totalFilesWithProjectName = 0

foreach ($file in $allFiles) {
    try {
        $content = Get-Content $file.FullName -Raw -ErrorAction SilentlyContinue
        if ($content -and $content -match $ProjectName) {
            $totalFilesWithProjectName++
            $relativePath = $file.FullName.Replace((Get-Location).Path, "").TrimStart('\', '/')
            
            # Categorizar archivo
            $category = "Code"  # Default
            
            if ($file.Extension -eq ".sln" -or $file.Name -eq "PimFlow.sln") {
                $category = "Essential"
            }
            elseif ($file.Extension -eq ".csproj") {
                $category = "Essential"
            }
            elseif ($file.Extension -match "\.(json|config)$") {
                $category = "Configuration"
            }
            elseif ($file.Extension -match "\.(md|txt)$") {
                $category = "Documentation"
            }
            elseif ($file.Extension -match "\.(ps1|sh|bat|yml|yaml)$") {
                $category = "Build"
            }
            elseif ($file.FullName -match "\\tests\\") {
                $category = "Tests"
            }
            
            $categories[$category] += @{
                Path = $relativePath
                Extension = $file.Extension
                Size = $file.Length
                Occurrences = ($content | Select-String -Pattern $ProjectName -AllMatches).Matches.Count
            }
        }
    }
    catch {
        # Ignorar archivos que no se pueden leer
    }
}

Write-Host "`nRESULTADOS DEL ANALISIS" -ForegroundColor Green
Write-Host "=" * 50

Write-Host "Total de archivos con '$ProjectName': $totalFilesWithProjectName" -ForegroundColor Yellow

foreach ($categoryName in $categories.Keys) {
    $files = $categories[$categoryName]
    if ($files.Count -gt 0) {
        Write-Host "`n$categoryName ($($files.Count) archivos):" -ForegroundColor Cyan
        
        foreach ($file in $files | Sort-Object Path) {
            $occurrences = $file.Occurrences
            Write-Host "   • $($file.Path) ($occurrences ocurrencias)" -ForegroundColor Gray
        }
    }
}

# Análisis de reducción potencial
Write-Host "`nANALISIS DE REDUCCION POTENCIAL" -ForegroundColor Green
Write-Host "=" * 50

$essential = $categories["Essential"].Count
$configurable = $categories["Configuration"].Count + $categories["Documentation"].Count + $categories["Build"].Count
$codeFiles = $categories["Code"].Count + $categories["Tests"].Count

Write-Host "Archivos esenciales (deben mantener el nombre): $essential" -ForegroundColor Green
Write-Host "Archivos configurables (pueden usar variables): $configurable" -ForegroundColor Yellow
Write-Host "Archivos de codigo (namespaces): $codeFiles" -ForegroundColor Blue

$potentialReduction = $configurable
$targetFiles = $essential + ($codeFiles * 0.3)  # Asumiendo que podemos reducir 70% del código

Write-Host "`nPROYECCION:" -ForegroundColor Magenta
Write-Host "   Actual: $totalFilesWithProjectName archivos"
Write-Host "   Objetivo: ≤ 30 archivos"
Write-Host "   Reducción potencial: $potentialReduction archivos"
Write-Host "   Archivos objetivo estimado: $([math]::Round($targetFiles))"

if ($targetFiles -le 30) {
    Write-Host "   OBJETIVO ALCANZABLE" -ForegroundColor Green
} else {
    Write-Host "   REQUIERE TRABAJO ADICIONAL" -ForegroundColor Yellow
}

# Generar recomendaciones
Write-Host "`nRECOMENDACIONES:" -ForegroundColor Green
Write-Host "1. Usar variables de configuración en archivos .json"
Write-Host "2. Usar variables de entorno en scripts de build"
Write-Host "3. Usar placeholders en documentación"
Write-Host "4. Considerar usar Assembly Attributes para metadatos"
Write-Host "5. Centralizar configuración de nombres en un solo lugar"

return @{
    TotalFiles = $totalFilesWithProjectName
    Categories = $categories
    PotentialReduction = $potentialReduction
    TargetAchievable = ($targetFiles -le 30)
}
