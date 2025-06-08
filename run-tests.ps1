#!/usr/bin/env pwsh

Write-Host "🧪 Ejecutando Tests del PIM TaskFlow" -ForegroundColor Green
Write-Host "=====================================" -ForegroundColor Green

# Verificar que dotnet esté disponible
if (-not (Get-Command dotnet -ErrorAction SilentlyContinue)) {
    Write-Host "❌ .NET SDK no está instalado o no está en el PATH" -ForegroundColor Red
    Write-Host "Por favor instala .NET 8 SDK desde: https://dotnet.microsoft.com/download" -ForegroundColor Yellow
    exit 1
}

# Mostrar versión de .NET
Write-Host "📋 Versión de .NET:" -ForegroundColor Cyan
dotnet --version

Write-Host ""
Write-Host "🔨 Compilando proyecto..." -ForegroundColor Yellow

# Compilar el proyecto
$buildResult = dotnet build tests/TaskFlow.Server.Tests/TaskFlow.Server.Tests.csproj --configuration Release --verbosity quiet
if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ Error en la compilación" -ForegroundColor Red
    exit 1
}

Write-Host "✅ Compilación exitosa" -ForegroundColor Green
Write-Host ""

Write-Host "🧪 Ejecutando tests..." -ForegroundColor Yellow
Write-Host ""

# Ejecutar tests con reporte detallado
dotnet test tests/TaskFlow.Server.Tests/TaskFlow.Server.Tests.csproj `
    --configuration Release `
    --logger "console;verbosity=detailed" `
    --collect:"XPlat Code Coverage"

if ($LASTEXITCODE -eq 0) {
    Write-Host ""
    Write-Host "🎉 ¡Todos los tests pasaron exitosamente!" -ForegroundColor Green
    Write-Host ""
    Write-Host "Resumen de tests ejecutados:" -ForegroundColor Cyan
    Write-Host "  - Tests de Repositorios (ArticleRepository, CustomAttributeRepository)" -ForegroundColor White
    Write-Host "  - Tests de Servicios (ArticleService, CustomAttributeService)" -ForegroundColor White
    Write-Host "  - Tests de Controladores (ArticlesController)" -ForegroundColor White
    Write-Host "  - Tests de Integracion (Workflow completo del PIM)" -ForegroundColor White
    Write-Host ""
    Write-Host "✅ El sistema PIM está funcionando correctamente" -ForegroundColor Green
}
else {
    Write-Host ""
    Write-Host "❌ Algunos tests fallaron" -ForegroundColor Red
    Write-Host "Revisa los errores arriba para mas detalles" -ForegroundColor Yellow
    exit 1
}
