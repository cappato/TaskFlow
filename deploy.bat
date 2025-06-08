@echo off
echo 🚀 TaskFlow Deployment Script
echo ==============================

echo.
echo 📋 Running tests...
dotnet test --configuration Release
if %ERRORLEVEL% neq 0 (
    echo ❌ Tests failed! Deployment aborted.
    exit /b 1
)

echo.
echo ✅ Tests passed! Building for production...
dotnet build --configuration Release
if %ERRORLEVEL% neq 0 (
    echo ❌ Build failed! Deployment aborted.
    exit /b 1
)

echo.
echo 📦 Publishing application...
dotnet publish src/TaskFlow.Server/TaskFlow.Server.csproj --configuration Release --output ./publish

echo.
echo 🌐 Ready for deployment!
echo.
echo Next steps:
echo 1. Push to GitHub (triggers automatic Railway deployment)
echo 2. Or manually deploy using Railway CLI
echo.
echo Railway CLI commands:
echo   railway login
echo   railway link [your-project-id]
echo   railway up
echo.
echo ✅ Deployment preparation complete!
pause
