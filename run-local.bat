@echo off
cls
echo ========================================
echo 🚀 PimFlow - Ejecutar en Local
echo ========================================

echo.
echo 🔍 Verificando .NET SDK...
dotnet --version
if %ERRORLEVEL% neq 0 (
    echo ❌ ERROR: .NET 8 SDK no encontrado
    echo 📥 Descargar desde: https://dotnet.microsoft.com/download
    pause
    exit /b 1
)

echo ✅ .NET SDK encontrado
echo.

echo 🧹 Limpiando builds anteriores...
dotnet clean --verbosity quiet
echo ✅ Limpieza completada

echo.
echo 📦 Restaurando paquetes...
dotnet restore --verbosity quiet
if %ERRORLEVEL% neq 0 (
    echo ❌ ERROR: Falló la restauración de paquetes
    pause
    exit /b 1
)
echo ✅ Paquetes restaurados

echo.
echo 🔨 Compilando proyecto...
dotnet build --configuration Debug --verbosity quiet
if %ERRORLEVEL% neq 0 (
    echo ❌ ERROR: Falló la compilación
    echo.
    echo 🔧 Intentando compilación con detalles...
    dotnet build --configuration Debug
    pause
    exit /b 1
)
echo ✅ Compilación exitosa

echo.
echo 🧪 Ejecutando tests rápidos del dominio...
dotnet test tests/PimFlow.Domain.Tests/ --verbosity quiet --no-build
if %ERRORLEVEL% neq 0 (
    echo ⚠️  ADVERTENCIA: Algunos tests del dominio fallaron
) else (
    echo ✅ Tests del dominio OK
)

echo.
echo 🌐 Iniciando servidor web...
echo.
echo ========================================
echo ✅ PimFlow está iniciando...
echo ========================================
echo.
echo 🌐 URLs disponibles:
echo   • Aplicación: http://localhost:5020
echo   • Swagger API: http://localhost:5020/swagger
echo   • Health Check: http://localhost:5020/health
echo   • Test Endpoint: http://localhost:5020/api/test
echo.
echo 🔧 Entorno: Development
echo 💾 Base de datos: SQLite (App_Data/application-dev.db)
echo.
echo ⚡ Presiona Ctrl+C para detener el servidor
echo ========================================
echo.

cd src\PimFlow.Server
set ASPNETCORE_ENVIRONMENT=Development
set ASPNETCORE_URLS=http://localhost:5020
dotnet run --no-build
