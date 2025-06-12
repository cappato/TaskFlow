@echo off
echo ========================================
echo 🚀 PimFlow - Ejecución Mínima
echo ========================================

taskkill /f /im dotnet.exe >nul 2>&1

cd src\PimFlow.Server

echo.
echo Ejecutando con configuración mínima...
echo Puerto: 5024
echo.

dotnet run --urls "http://0.0.0.0:5024" --environment Development
