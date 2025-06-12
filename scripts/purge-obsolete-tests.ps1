#!/usr/bin/env pwsh

<#
.SYNOPSIS
    Purga tests obsoletos después de la reorganización por features
.DESCRIPTION
    Este script elimina los tests obsoletos que han sido migrados a la nueva estructura por features
.PARAMETER DryRun
    Si se especifica, solo muestra qué archivos se eliminarían sin hacerlo realmente
#>

param(
    [switch]$DryRun
)

# Configuración de colores
function Write-Info($message) { Write-Host $message -ForegroundColor Cyan }
function Write-Success($message) { Write-Host $message -ForegroundColor Green }
function Write-Warning($message) { Write-Host $message -ForegroundColor Yellow }
function Write-Error($message) { Write-Host $message -ForegroundColor Red }

Write-Info "🧹 Purgando tests obsoletos después de reorganización por features"
if ($DryRun) {
    Write-Warning "⚠️  MODO DRY RUN - Solo se mostrarán los archivos que se eliminarían"
}

# Archivos y carpetas obsoletos para eliminar
$obsoleteItems = @(
    # Tests obsoletos con estructura técnica
    "tests/PimFlow.Domain.Tests/Entities/CategoryDomainLogicTests.cs",
    "tests/PimFlow.Domain.Tests/Events/DomainEventsTests.cs",
    "tests/PimFlow.Domain.Tests/Specifications/ArticleSpecificationsTests.cs",
    "tests/PimFlow.Domain.Tests/ValueObjects/SKUTests.cs",
    "tests/PimFlow.Domain.Tests/ValueObjects/ProductNameTests.cs",
    "tests/PimFlow.Domain.Tests/ValueObjects/DeletionInfoTests.cs",
    "tests/PimFlow.Domain.Tests/ValueObjects/EmailTests.cs",
    
    # Carpetas obsoletas (se eliminarán si están vacías)
    "tests/PimFlow.Domain.Tests/Entities",
    "tests/PimFlow.Domain.Tests/Events", 
    "tests/PimFlow.Domain.Tests/Specifications",
    "tests/PimFlow.Domain.Tests/ValueObjects"
)

$deletedFiles = 0
$deletedFolders = 0
$errors = @()

Write-Info "📁 Procesando $($obsoleteItems.Count) elementos obsoletos..."

foreach ($item in $obsoleteItems) {
    try {
        if (Test-Path $item) {
            $isDirectory = (Get-Item $item).PSIsContainer
            
            if ($isDirectory) {
                # Es una carpeta - verificar si está vacía
                $contents = Get-ChildItem $item -Force
                if ($contents.Count -eq 0) {
                    if ($DryRun) {
                        Write-Warning "SERIA ELIMINADA (carpeta vacía): $item"
                    } else {
                        Remove-Item $item -Force
                        Write-Success "ELIMINADA (carpeta vacía): $item"
                        $deletedFolders++
                    }
                } else {
                    Write-Warning "CARPETA NO VACÍA (no eliminada): $item - Contiene $($contents.Count) elementos"
                }
            } else {
                # Es un archivo
                if ($DryRun) {
                    Write-Warning "SERIA ELIMINADO: $item"
                } else {
                    Remove-Item $item -Force
                    Write-Success "ELIMINADO: $item"
                    $deletedFiles++
                }
            }
        } else {
            Write-Info "YA NO EXISTE: $item"
        }
    }
    catch {
        $errorMsg = "Error procesando $item : $($_.Exception.Message)"
        $errors += $errorMsg
        Write-Error $errorMsg
    }
}

# Intentar eliminar carpetas padre si están vacías
$parentFolders = @(
    "tests/PimFlow.Domain.Tests/Entities",
    "tests/PimFlow.Domain.Tests/Events",
    "tests/PimFlow.Domain.Tests/Specifications", 
    "tests/PimFlow.Domain.Tests/ValueObjects"
)

foreach ($folder in $parentFolders) {
    try {
        if (Test-Path $folder) {
            $contents = Get-ChildItem $folder -Force
            if ($contents.Count -eq 0) {
                if ($DryRun) {
                    Write-Warning "SERIA ELIMINADA (carpeta vacía): $folder"
                } else {
                    Remove-Item $folder -Force
                    Write-Success "ELIMINADA (carpeta vacía): $folder"
                    $deletedFolders++
                }
            }
        }
    }
    catch {
        Write-Warning "No se pudo eliminar carpeta $folder : $($_.Exception.Message)"
    }
}

# Mostrar resumen
Write-Info "`n📊 Resumen de purga de tests:"
Write-Host "  Archivos eliminados: $deletedFiles" -ForegroundColor White
Write-Host "  Carpetas eliminadas: $deletedFolders" -ForegroundColor White
Write-Host "  Errores: $($errors.Count)" -ForegroundColor White

if ($errors.Count -gt 0) {
    Write-Error "`n❌ Errores encontrados:"
    foreach ($error in $errors) {
        Write-Host "  $error" -ForegroundColor Red
    }
}

if ($DryRun) {
    Write-Warning "`n⚠️  Para aplicar los cambios, ejecuta el script sin el parámetro -DryRun"
} else {
    Write-Success "`n✅ Purga de tests obsoletos completada"
    
    # Verificar estructura resultante
    Write-Info "`n📁 Estructura resultante de tests:"
    if (Test-Path "tests/PimFlow.Domain.Tests") {
        Get-ChildItem "tests/PimFlow.Domain.Tests" -Directory | ForEach-Object {
            Write-Host "  📁 $($_.Name)" -ForegroundColor Green
            Get-ChildItem $_.FullName -Recurse -File -Include "*.cs" | ForEach-Object {
                $relativePath = $_.FullName.Replace((Get-Item "tests/PimFlow.Domain.Tests").FullName, "").TrimStart('\')
                Write-Host "    📄 $relativePath" -ForegroundColor White
            }
        }
    }
}

Write-Info "`n🎯 Próximos pasos:"
Write-Host "  1. Verificar que la compilación funciona: dotnet build tests/PimFlow.Domain.Tests/" -ForegroundColor White
Write-Host "  2. Ejecutar tests nuevos: dotnet test tests/PimFlow.Domain.Tests/ --filter 'Category=Validation'" -ForegroundColor White
Write-Host "  3. Actualizar tests de Server si es necesario" -ForegroundColor White
