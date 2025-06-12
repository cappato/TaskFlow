#!/usr/bin/env pwsh

Write-Host "🔄 Corrigiendo namespaces restantes en PimFlow.Server..." -ForegroundColor Cyan

# Lista de archivos específicos que necesitan corrección
$filesToFix = @(
    "src/PimFlow.Server/Services/CustomAttributeCommandService.cs",
    "src/PimFlow.Server/Services/CustomAttributeQueryService.cs", 
    "src/PimFlow.Server/Services/DatabaseInitializationService.cs",
    "src/PimFlow.Server/Services/DomainEventService.cs",
    "src/PimFlow.Server/Services/IArticleQueryService.cs",
    "src/PimFlow.Server/Services/IArticleService.cs",
    "src/PimFlow.Server/Mapping/ArticleMappingProfile.cs"
)

# Reemplazos específicos
$replacements = @{
    "using PimFlow\.Domain\.Entities;" = "using PimFlow.Domain.Article;`nusing PimFlow.Domain.Category;`nusing PimFlow.Domain.User;`nusing PimFlow.Domain.CustomAttribute;"
    "using PimFlow\.Domain\.Enums;" = "using PimFlow.Domain.Article.Enums;`nusing PimFlow.Domain.CustomAttribute.Enums;"
    "using PimFlow\.Domain\.Events;" = "using PimFlow.Domain.Common;"
    "using PimFlow\.Domain\.ValueObjects;" = "using PimFlow.Domain.Article.ValueObjects;`nusing PimFlow.Domain.Category.ValueObjects;`nusing PimFlow.Domain.User.ValueObjects;"
}

$totalFixed = 0

foreach ($file in $filesToFix) {
    if (Test-Path $file) {
        try {
            $content = Get-Content $file -Raw
            $originalContent = $content
            $hasChanges = $false
            
            foreach ($pattern in $replacements.Keys) {
                if ($content -match $pattern) {
                    $content = $content -replace $pattern, $replacements[$pattern]
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
                Set-Content $file -Value $newContent -NoNewline
                
                $relativePath = $file.Replace((Get-Location).Path, "").TrimStart('\', '/')
                Write-Host "✅ CORREGIDO: $relativePath" -ForegroundColor Green
                $totalFixed++
            }
        }
        catch {
            Write-Host "❌ ERROR en $file : $($_.Exception.Message)" -ForegroundColor Red
        }
    } else {
        Write-Host "⚠️  ARCHIVO NO ENCONTRADO: $file" -ForegroundColor Yellow
    }
}

Write-Host "`n📊 Resumen:" -ForegroundColor Cyan
Write-Host "  Archivos corregidos: $totalFixed" -ForegroundColor White

# Intentar compilar para verificar
Write-Host "`n🔧 Verificando compilación..." -ForegroundColor Cyan
try {
    $buildOutput = & dotnet build src/PimFlow.Server/PimFlow.Server.csproj --verbosity quiet 2>&1
    if ($LASTEXITCODE -eq 0) {
        Write-Host "✅ ¡Compilación exitosa!" -ForegroundColor Green
    } else {
        Write-Host "⚠️  Aún hay errores de compilación. Revisando..." -ForegroundColor Yellow
        # Mostrar solo los primeros 10 errores para no saturar
        $errors = $buildOutput | Where-Object { $_ -match "error CS" } | Select-Object -First 10
        foreach ($error in $errors) {
            Write-Host "  $error" -ForegroundColor Red
        }
        if ($errors.Count -eq 10) {
            Write-Host "  ... y más errores" -ForegroundColor Red
        }
    }
} catch {
    Write-Host "❌ Error al ejecutar compilación" -ForegroundColor Red
}

Write-Host "`n🎯 Próximos pasos si aún hay errores:" -ForegroundColor Cyan
Write-Host "  1. Revisar archivos con 'ArticleAttributeValue' → usar 'PimFlow.Domain.CustomAttribute.ArticleAttributeValue'" -ForegroundColor White
Write-Host "  2. Revisar archivos con 'CustomAttribute' → usar 'PimFlow.Domain.CustomAttribute.CustomAttribute'" -ForegroundColor White
Write-Host "  3. Revisar archivos con eventos → usar namespaces de features específicos" -ForegroundColor White
