@echo off
echo ========================================
echo 🚀 PimFlow - Ejecución Forzada con Logs
echo ========================================

echo Matando procesos dotnet existentes...
taskkill /f /im dotnet.exe >nul 2>&1

echo.
echo Cambiando al directorio del servidor...
cd src\PimFlow.Server

echo.
echo Configurando variables de entorno...
set ASPNETCORE_ENVIRONMENT=Development
set ASPNETCORE_URLS=http://localhost:5023
set Logging__LogLevel__Default=Information
set Logging__LogLevel__Microsoft=Warning
set Logging__LogLevel__Microsoft.AspNetCore=Information

echo.
echo ========================================
echo 🌐 Iniciando servidor en puerto 5023...
echo ========================================
echo.

dotnet run --verbosity normal
