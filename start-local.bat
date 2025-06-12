@echo off
echo ========================================
echo 🚀 PimFlow - Iniciando en Local
echo ========================================

echo.
echo 🔍 Verificando .NET...
dotnet --version
if %ERRORLEVEL% neq 0 (
    echo ❌ ERROR: .NET 8 SDK no está instalado
    echo 📥 Instalar desde: https://dotnet.microsoft.com/download
    pause
    exit /b 1
)

echo.
echo 📦 Restaurando paquetes...
dotnet restore
if %ERRORLEVEL% neq 0 (
    echo ❌ ERROR: Falló la restauración de paquetes
    pause
    exit /b 1
)

echo.
echo 🔨 Compilando proyecto...
dotnet build --configuration Debug
if %ERRORLEVEL% neq 0 (
    echo ❌ ERROR: Falló la compilación
    pause
    exit /b 1
)

echo.
echo 🧪 Ejecutando tests rápidos...
dotnet test tests/PimFlow.Domain.Tests/ --verbosity minimal
if %ERRORLEVEL% neq 0 (
    echo ⚠️  ADVERTENCIA: Algunos tests fallaron, pero continuando...
)

echo.
echo 🌐 Iniciando servidor en puerto 5002...
echo.
echo ========================================
echo ✅ PimFlow está ejecutándose!
echo ========================================
echo.
echo 🌐 Aplicación: http://localhost:5002
echo 📚 Swagger API: http://localhost:5002/swagger
echo 🔧 Entorno: Development
echo.
echo Presiona Ctrl+C para detener el servidor
echo ========================================

cd src\PimFlow.Server
dotnet run --urls http://localhost:5002
