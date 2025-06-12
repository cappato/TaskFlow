@echo off
echo ========================================
echo 🚀 PimFlow - Inicio Simple
echo ========================================

cd src\PimFlow.Server

echo.
echo 🔨 Compilando...
dotnet build --configuration Debug --verbosity quiet
if %ERRORLEVEL% neq 0 (
    echo ❌ Error de compilación
    pause
    exit /b 1
)

echo ✅ Compilación exitosa
echo.
echo 🌐 Iniciando servidor en puerto 5016...
echo.
echo ========================================
echo ✅ URLs disponibles:
echo   🌐 Aplicación: http://localhost:5016
echo   📚 Swagger: http://localhost:5016/swagger
echo   🧪 Test: http://localhost:5016/api/test
echo   🏥 Health: http://localhost:5016/health
echo ========================================
echo.
echo Presiona Ctrl+C para detener
echo.

dotnet run --urls http://localhost:5016
