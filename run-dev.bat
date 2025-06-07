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
echo Starting API Server (TaskFlow.Server)...
start "TaskFlow API" cmd /k "cd src\TaskFlow.Server && dotnet run"

echo Waiting for API to start...
timeout /t 5 /nobreak > nul

echo.
echo Starting Blazor Client (TaskFlow.Client)...
start "TaskFlow Client" cmd /k "cd src\TaskFlow.Client && dotnet run"

echo.
echo ========================================
echo TaskFlow Development Environment Started!
echo ========================================
echo.
echo API Server: https://localhost:7000
echo Swagger UI: https://localhost:7000/swagger
echo Blazor Client: https://localhost:7001
echo.
echo Press any key to stop all services...
pause > nul

echo.
echo Stopping services...
taskkill /f /im dotnet.exe 2>nul
echo Services stopped.
pause
