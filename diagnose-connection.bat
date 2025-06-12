@echo off
echo ========================================
echo 🔍 Diagnóstico de Conexión - PimFlow
echo ========================================

echo.
echo 🔍 1. Verificando procesos dotnet...
tasklist /fi "imagename eq dotnet.exe" /fo table 2>nul | findstr dotnet
if %ERRORLEVEL% equ 0 (
    echo ✅ Procesos dotnet encontrados
) else (
    echo ❌ No hay procesos dotnet ejecutándose
    echo.
    echo 🚀 Iniciando servidor...
    start /b cmd /c "cd src\PimFlow.Server && dotnet run --urls http://localhost:5022"
    timeout /t 5 /nobreak >nul
)

echo.
echo 🌐 2. Verificando puertos en uso...
netstat -an | findstr :5021
netstat -an | findstr :5022
netstat -an | findstr :5020

echo.
echo 🔗 3. Probando conectividad con curl...
where curl >nul 2>&1
if %ERRORLEVEL% equ 0 (
    echo Probando http://localhost:5021...
    curl -s -o nul -w "Status: %%{http_code}\n" http://localhost:5021 --connect-timeout 5
    echo Probando http://localhost:5022...
    curl -s -o nul -w "Status: %%{http_code}\n" http://localhost:5022 --connect-timeout 5
) else (
    echo curl no disponible, usando PowerShell...
    powershell -Command "try { $r = Invoke-WebRequest -Uri 'http://localhost:5021' -UseBasicParsing -TimeoutSec 5; Write-Host 'Puerto 5021: OK -' $r.StatusCode } catch { Write-Host 'Puerto 5021: ERROR -' $_.Exception.Message }"
    powershell -Command "try { $r = Invoke-WebRequest -Uri 'http://localhost:5022' -UseBasicParsing -TimeoutSec 5; Write-Host 'Puerto 5022: OK -' $r.StatusCode } catch { Write-Host 'Puerto 5022: ERROR -' $_.Exception.Message }"
)

echo.
echo 🔥 4. Verificando firewall...
powershell -Command "Get-NetFirewallRule -DisplayName '*dotnet*' -ErrorAction SilentlyContinue | Select-Object DisplayName, Enabled, Direction"

echo.
echo 📋 5. Información del sistema...
echo Puerto predeterminado: 5021
echo Entorno: Development
echo Base de datos: SQLite

echo.
echo ========================================
echo 🌐 URLs para probar manualmente:
echo   http://localhost:5021
echo   http://localhost:5021/swagger
echo   http://localhost:5021/health
echo   http://localhost:5022
echo   http://localhost:5022/swagger
echo ========================================

pause
