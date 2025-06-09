@echo off
echo Starting TaskFlow Development Environment...

echo.
echo Checking .NET installation...
dotnet --version
if %ERRORLEVEL% neq 0 (
    echo ERROR: .NET 8 SDK is not installed or not in PATH
    echo Please install .NET 8 SDK from https://dotnet.microsoft.com/download
    pause
    exit /b 1
)

echo.
echo Restoring NuGet packages...
dotnet restore
if %ERRORLEVEL% neq 0 (
    echo ERROR: Failed to restore packages
    pause
    exit /b 1
)

echo.
echo Building solution...
dotnet build --configuration Debug
if %ERRORLEVEL% neq 0 (
    echo ERROR: Build failed
    pause
    exit /b 1
)

echo.
echo Starting PimFlow Hosted Server...
start "PimFlow Hosted" cmd /k "cd src\PimFlow.Server && dotnet run"

echo Waiting for server to start...
timeout /t 8 /nobreak > nul

echo.
echo ========================================
echo PimFlow Hosted Development Environment Started!
echo ========================================
echo.
echo Application: http://localhost:5001
echo Swagger UI: http://localhost:5001/swagger
echo (Client served from same port - Hosted Architecture)
echo.
echo Press any key to stop all services...
pause > nul

echo.
echo Stopping services...
taskkill /f /im dotnet.exe 2>nul
echo Services stopped.
pause
