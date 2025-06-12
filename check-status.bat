@echo off
echo ========================================
echo 🔍 Verificando Estado de PimFlow
echo ========================================

echo.
echo 🔍 Verificando procesos dotnet...
tasklist /fi "imagename eq dotnet.exe" /fo table | findstr dotnet
if %ERRORLEVEL% equ 0 (
    echo ✅ Proceso dotnet ejecutándose
) else (
    echo ❌ No hay procesos dotnet ejecutándose
)

echo.
echo 🌐 Verificando conectividad...
echo Probando http://localhost:5020...

powershell -Command "try { $response = Invoke-WebRequest -Uri 'http://localhost:5020/health' -UseBasicParsing -TimeoutSec 5; if ($response.StatusCode -eq 200) { Write-Host '✅ Servidor respondiendo correctamente' } else { Write-Host '⚠️ Servidor responde pero con código:' $response.StatusCode } } catch { Write-Host '❌ Servidor no responde o no está ejecutándose' }"

echo.
echo ========================================
echo 📋 URLs para verificar manualmente:
echo ========================================
echo 🌐 Aplicación principal:
echo    http://localhost:5020
echo.
echo 📚 Swagger API (Documentación):
echo    http://localhost:5020/swagger
echo.
echo 🧪 Endpoint de prueba:
echo    http://localhost:5020/api/test
echo.
echo 🏥 Health Check:
echo    http://localhost:5020/health
echo.
echo 📊 Endpoints principales de la API:
echo    GET  http://localhost:5020/api/articles
echo    POST http://localhost:5020/api/articles
echo    GET  http://localhost:5020/api/categories
echo    GET  http://localhost:5020/api/customattributes
echo ========================================

pause
