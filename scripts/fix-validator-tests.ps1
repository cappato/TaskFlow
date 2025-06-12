#!/usr/bin/env pwsh

Write-Host "🔧 Corrigiendo tests de validadores para usar métodos estáticos..." -ForegroundColor Cyan

# Archivos de tests de validadores
$validatorTestFiles = @(
    "tests/PimFlow.Domain.Tests/Article/ArticleValidatorTests.cs",
    "tests/PimFlow.Domain.Tests/Category/CategoryValidatorTests.cs",
    "tests/PimFlow.Domain.Tests/User/UserValidatorTests.cs",
    "tests/PimFlow.Domain.Tests/CustomAttribute/CustomAttributeValidatorTests.cs"
)

# Reemplazos para cada tipo de validador
$replacements = @{
    "ArticleValidatorTests.cs" = @{
        "private readonly ArticleValidator _validator = new();" = ""
        "_validator.Validate" = "ArticleValidator.Validate"
    }
    "CategoryValidatorTests.cs" = @{
        "private readonly CategoryValidator _validator = new();" = ""
        "_validator.Validate" = "CategoryValidator.Validate"
    }
    "UserValidatorTests.cs" = @{
        "private readonly UserValidator _validator = new();" = ""
        "_validator.Validate" = "UserValidator.Validate"
    }
    "CustomAttributeValidatorTests.cs" = @{
        "private readonly CustomAttributeValidator _validator = new();" = ""
        "_validator.Validate" = "CustomAttributeValidator.Validate"
    }
}

$totalFixed = 0

foreach ($file in $validatorTestFiles) {
    if (Test-Path $file) {
        try {
            $content = Get-Content $file -Raw
            $originalContent = $content
            $hasChanges = $false
            
            # Obtener el nombre del archivo para buscar reemplazos específicos
            $fileName = Split-Path $file -Leaf
            
            if ($replacements.ContainsKey($fileName)) {
                $fileReplacements = $replacements[$fileName]
                
                foreach ($pattern in $fileReplacements.Keys) {
                    $replacement = $fileReplacements[$pattern]
                    
                    if ($content -match [regex]::Escape($pattern)) {
                        $content = $content -replace [regex]::Escape($pattern), $replacement
                        $hasChanges = $true
                    }
                }
            }
            
            if ($hasChanges) {
                # Limpiar líneas vacías duplicadas
                $lines = $content -split "`n"
                $cleanedLines = @()
                $previousLineEmpty = $false
                
                foreach ($line in $lines) {
                    $isCurrentLineEmpty = [string]::IsNullOrWhiteSpace($line)
                    
                    if (-not ($isCurrentLineEmpty -and $previousLineEmpty)) {
                        $cleanedLines += $line
                    }
                    
                    $previousLineEmpty = $isCurrentLineEmpty
                }
                
                $content = $cleanedLines -join "`n"
                Set-Content $file -Value $content -NoNewline
                
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
    $buildOutput = & dotnet build tests/PimFlow.Domain.Tests/PimFlow.Domain.Tests.csproj --verbosity quiet 2>&1
    if ($LASTEXITCODE -eq 0) {
        Write-Host "✅ ¡Compilación exitosa!" -ForegroundColor Green
        
        # Intentar ejecutar tests de validación
        Write-Host "`n🧪 Ejecutando tests de validación..." -ForegroundColor Cyan
        $testOutput = & dotnet test tests/PimFlow.Domain.Tests/ --filter "Category=Validation" --verbosity quiet 2>&1
        if ($LASTEXITCODE -eq 0) {
            Write-Host "✅ ¡Tests de validación ejecutados exitosamente!" -ForegroundColor Green
        } else {
            Write-Host "⚠️  Tests de validación tienen problemas" -ForegroundColor Yellow
        }
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
Write-Host "  1. Ejecutar: dotnet test tests/PimFlow.Domain.Tests/ --filter 'Category=Validation'" -ForegroundColor White
Write-Host "  2. Ejecutar: dotnet test tests/PimFlow.Domain.Tests/ --filter 'Category=ValueObjects'" -ForegroundColor White
Write-Host "  3. Verificar que todos los tests nuevos funcionan correctamente" -ForegroundColor White
