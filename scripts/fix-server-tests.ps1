#!/usr/bin/env pwsh

Write-Host "🔧 Corrigiendo imports obsoletos en tests del servidor..." -ForegroundColor Cyan

# Mapeo de namespaces obsoletos a nuevos
$namespaceMapping = @{
    "using PimFlow.Domain.Entities;" = "using PimFlow.Domain.Article;" + [Environment]::NewLine + "using PimFlow.Domain.Category;" + [Environment]::NewLine + "using PimFlow.Domain.CustomAttribute;" + [Environment]::NewLine + "using PimFlow.Domain.User;"
    "using PimFlow.Domain.Events;" = "using PimFlow.Domain.Article;" + [Environment]::NewLine + "using PimFlow.Domain.Category;" + [Environment]::NewLine + "using PimFlow.Domain.Common;"
    "using PimFlow.Domain.Enums;" = "using PimFlow.Domain.Article.Enums;" + [Environment]::NewLine + "using PimFlow.Domain.CustomAttribute.Enums;"
    "using PimFlow.Domain.Specifications;" = "using PimFlow.Domain.Article;" + [Environment]::NewLine + "using PimFlow.Domain.Category;" + [Environment]::NewLine + "using PimFlow.Domain.Common;"
    "using PimFlow.Domain.ValueObjects;" = "using PimFlow.Domain.Article.ValueObjects;" + [Environment]::NewLine + "using PimFlow.Domain.Category.ValueObjects;" + [Environment]::NewLine + "using PimFlow.Domain.User.ValueObjects;"
}

# Referencias específicas que necesitan ser actualizadas
$typeMapping = @{
    "PimFlow.Domain.Entities.Article" = "PimFlow.Domain.Article.Article"
    "PimFlow.Domain.Entities.Category" = "PimFlow.Domain.Category.Category"
    "PimFlow.Domain.Entities.CustomAttribute" = "PimFlow.Domain.CustomAttribute.CustomAttribute"
    "PimFlow.Domain.Entities.User" = "PimFlow.Domain.User.User"
    "PimFlow.Domain.Enums.ArticleType" = "PimFlow.Domain.Article.Enums.ArticleType"
    "PimFlow.Domain.Enums.AttributeType" = "PimFlow.Domain.CustomAttribute.Enums.AttributeType"
    "Specification<" = "ISpecification<"
}

# Obtener todos los archivos .cs en tests del servidor
$testFiles = Get-ChildItem "tests/PimFlow.Server.Tests" -Recurse -Include "*.cs"

$totalFiles = $testFiles.Count
$processedFiles = 0
$modifiedFiles = 0

Write-Host "📁 Procesando $totalFiles archivos..." -ForegroundColor Yellow

foreach ($file in $testFiles) {
    try {
        $content = Get-Content $file.FullName -Raw
        $originalContent = $content
        $hasChanges = $false
        
        # Aplicar mapeo de namespaces
        foreach ($oldNamespace in $namespaceMapping.Keys) {
            $newNamespace = $namespaceMapping[$oldNamespace]
            if ($content -match [regex]::Escape($oldNamespace)) {
                $content = $content -replace [regex]::Escape($oldNamespace), $newNamespace
                $hasChanges = $true
            }
        }
        
        # Aplicar mapeo de tipos específicos
        foreach ($oldType in $typeMapping.Keys) {
            $newType = $typeMapping[$oldType]
            if ($content -match [regex]::Escape($oldType)) {
                $content = $content -replace [regex]::Escape($oldType), $newType
                $hasChanges = $true
            }
        }
        
        # Limpiar duplicados de using
        $lines = $content -split "`n"
        $cleanedLines = @()
        $usingLines = @()
        $inUsingSection = $true
        
        foreach ($line in $lines) {
            if ($line.Trim().StartsWith("using ") -and $inUsingSection) {
                if ($usingLines -notcontains $line.Trim()) {
                    $usingLines += $line.Trim()
                }
            } else {
                if ($inUsingSection -and $line.Trim() -ne "") {
                    $inUsingSection = $false
                    # Agregar usings únicos
                    $cleanedLines += $usingLines | Sort-Object
                    $cleanedLines += ""
                }
                $cleanedLines += $line
            }
        }
        
        if ($hasChanges) {
            $content = $cleanedLines -join "`n"
            Set-Content $file.FullName -Value $content -NoNewline
            
            $relativePath = $file.FullName.Replace((Get-Location).Path, "").TrimStart('\', '/')
            Write-Host "✅ CORREGIDO: $relativePath" -ForegroundColor Green
            $modifiedFiles++
        }
        
        $processedFiles++
        
        # Mostrar progreso cada 10 archivos
        if ($processedFiles % 10 -eq 0) {
            Write-Host "📊 Progreso: $processedFiles/$totalFiles archivos procesados" -ForegroundColor Blue
        }
        
    } catch {
        $relativePath = $file.FullName.Replace((Get-Location).Path, "").TrimStart('\', '/')
        Write-Host "❌ ERROR en $relativePath : $($_.Exception.Message)" -ForegroundColor Red
    }
}

Write-Host "`n📊 Resumen:" -ForegroundColor Cyan
Write-Host "  Archivos procesados: $processedFiles" -ForegroundColor White
Write-Host "  Archivos modificados: $modifiedFiles" -ForegroundColor White

# Intentar compilar
Write-Host "`n🔧 Verificando compilación..." -ForegroundColor Cyan
try {
    $buildOutput = & dotnet build tests/PimFlow.Server.Tests/PimFlow.Server.Tests.csproj --verbosity quiet 2>&1
    if ($LASTEXITCODE -eq 0) {
        Write-Host "✅ ¡Compilación exitosa!" -ForegroundColor Green
    } else {
        Write-Host "⚠️  Aún hay errores de compilación" -ForegroundColor Yellow
        # Mostrar solo los primeros errores
        $errors = $buildOutput | Where-Object { $_ -match "error CS" } | Select-Object -First 5
        foreach ($error in $errors) {
            Write-Host "  $error" -ForegroundColor Red
        }
    }
} catch {
    Write-Host "❌ Error al verificar compilación" -ForegroundColor Red
}

Write-Host "`n🎯 Próximos pasos:" -ForegroundColor Cyan
Write-Host "  1. Revisar errores de compilación restantes" -ForegroundColor White
Write-Host "  2. Corregir tests de Shared" -ForegroundColor White
Write-Host "  3. Ejecutar: dotnet test tests/PimFlow.Server.Tests/" -ForegroundColor White
