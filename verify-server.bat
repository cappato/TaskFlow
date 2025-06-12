@echo off
echo ========================================
echo 🔍 Verificando servidor PimFlow
echo ========================================

echo.
echo 🌐 Probando endpoint de test...
curl -s http://localhost:5014/api/test
if %ERRORLEVEL% equ 0 (
    echo.
    echo ✅ Endpoint de test funcionando
) else (
    echo.
    echo ❌ Endpoint de test no responde
)

echo.
echo 🏥 Probando health check...
curl -s http://localhost:5014/health
if %ERRORLEVEL% equ 0 (
    echo.
    echo ✅ Health check funcionando
) else (
    echo.
    echo ❌ Health check no responde
)

echo.
echo 📚 Probando Swagger JSON...
curl -s http://localhost:5014/swagger/v1/swagger.json | findstr "openapi"
if %ERRORLEVEL% equ 0 (
    echo ✅ Swagger JSON funcionando
) else (
    echo ❌ Swagger JSON no responde
)

echo.
echo ========================================
echo 🌐 URLs disponibles:
echo   Aplicación: http://localhost:5014
echo   Swagger: http://localhost:5014/swagger
echo   Test API: http://localhost:5014/api/test
echo   Health: http://localhost:5014/health
echo ========================================
