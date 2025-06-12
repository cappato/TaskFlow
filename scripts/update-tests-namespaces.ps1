#!/usr/bin/env pwsh

<#
.SYNOPSIS
    Actualiza los namespaces de los tests después de la refactorización por features
.DESCRIPTION
    Este script actualiza automáticamente todas las referencias a los namespaces antiguos
    en los archivos de tests y los reorganiza por features
#>

Write-Host "🧪 Actualizando namespaces de tests después de refactorización por features..." -ForegroundColor Cyan

# Mapeo de namespaces antiguos a nuevos para tests
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

# Reemplazos específicos en código
$codeReplacements = @{
    'Domain\.Enums\.ArticleType' = 'PimFlow.Domain.Article.Enums.ArticleType'
    'Domain\.Enums\.AttributeType' = 'PimFlow.Domain.CustomAttribute.Enums.AttributeType'
}

# Archivos de tests a procesar
$testFiles = Get-ChildItem -Path "tests" -Recurse -Include "*.cs" | Where-Object {
    $_.FullName -notmatch "\\bin\\" -and 
    $_.FullName -notmatch "\\obj\\"
}

$totalFixed = 0
$errors = @()

Write-Host "📁 Procesando $($testFiles.Count) archivos de tests..." -ForegroundColor Yellow

foreach ($file in $testFiles) {
    try {
        $content = Get-Content $file.FullName -Raw -ErrorAction SilentlyContinue
        if (-not $content) { continue }
        
        $originalContent = $content
        $hasChanges = $false
        
        # Aplicar reemplazos de namespaces
        foreach ($pattern in $namespaceMapping.Keys) {
            if ($content -match $pattern) {
                $newNamespaces = $namespaceMapping[$pattern]
                
                if ($newNamespaces -is [array]) {
                    $replacement = $newNamespaces -join "`n"
                } else {
                    $replacement = $newNamespaces
                }
                
                $content = $content -replace $pattern, $replacement
                $hasChanges = $true
            }
        }
        
        # Aplicar reemplazos de código específicos
        foreach ($pattern in $codeReplacements.Keys) {
            if ($content -match $pattern) {
                $content = $content -replace $pattern, $codeReplacements[$pattern]
                $hasChanges = $true
            }
        }
        
        if ($hasChanges) {
            # Limpiar duplicados de using
            $lines = $content -split "`n"
            $usingLines = @()
            $otherLines = @()
            $inUsings = $true
            
            foreach ($line in $lines) {
                if ($line -match "^using " -and $inUsings) {
                    $cleanLine = $line.Trim()
                    if ($usingLines -notcontains $cleanLine -and $cleanLine -ne "") {
                        $usingLines += $cleanLine
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
            
            # Reconstruir archivo
            $newContent = ($usingLines + "" + $otherLines) -join "`n"
            Set-Content $file.FullName -Value $newContent -NoNewline
            
            $relativePath = $file.FullName.Replace((Get-Location).Path, "").TrimStart('\', '/')
            Write-Host "✅ ACTUALIZADO: $relativePath" -ForegroundColor Green
            $totalFixed++
        }
    }
    catch {
        $relativePath = $file.FullName.Replace((Get-Location).Path, "").TrimStart('\', '/')
        $errors += "❌ ERROR en $relativePath : $($_.Exception.Message)"
        Write-Host "❌ ERROR en $relativePath : $($_.Exception.Message)" -ForegroundColor Red
    }
}

# Mostrar resumen
Write-Host "`n📊 Resumen de actualización de tests:" -ForegroundColor Cyan
Write-Host "  Archivos actualizados: $totalFixed" -ForegroundColor White
Write-Host "  Errores: $($errors.Count)" -ForegroundColor White

if ($errors.Count -gt 0) {
    Write-Host "`n❌ Errores encontrados:" -ForegroundColor Red
    foreach ($error in $errors) {
        Write-Host "  $error" -ForegroundColor Red
    }
}

# Intentar compilar tests para verificar
Write-Host "`n🔧 Verificando compilación de tests..." -ForegroundColor Cyan
try {
    $buildOutput = & dotnet build tests/PimFlow.Domain.Tests/PimFlow.Domain.Tests.csproj --verbosity quiet 2>&1
    if ($LASTEXITCODE -eq 0) {
        Write-Host "✅ Tests de Domain compilan correctamente!" -ForegroundColor Green
    } else {
        Write-Host "⚠️  Aún hay errores en tests de Domain" -ForegroundColor Yellow
    }
    
    $buildOutput = & dotnet build tests/PimFlow.Server.Tests/PimFlow.Server.Tests.csproj --verbosity quiet 2>&1
    if ($LASTEXITCODE -eq 0) {
        Write-Host "✅ Tests de Server compilan correctamente!" -ForegroundColor Green
    } else {
        Write-Host "⚠️  Aún hay errores en tests de Server" -ForegroundColor Yellow
    }
} catch {
    Write-Host "❌ Error al verificar compilación de tests" -ForegroundColor Red
}

Write-Host "`n🎯 Próximos pasos recomendados:" -ForegroundColor Cyan
Write-Host "  1. Reorganizar tests por features (Article/, Category/, etc.)" -ForegroundColor White
Write-Host "  2. Crear tests para nuevos validadores" -ForegroundColor White
Write-Host "  3. Crear tests para nuevas specifications" -ForegroundColor White
Write-Host "  4. Ejecutar: dotnet test para verificar funcionalidad" -ForegroundColor White
