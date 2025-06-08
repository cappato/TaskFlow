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
echo 1. Push to GitHub (triggers automatic Azure deployment)
echo 2. Or deploy from Visual Studio:
echo    - Right-click TaskFlow.Server → Publish
echo    - Azure → App Service → Deploy
echo.
echo Azure CLI commands (optional):
echo   az login
echo   az webapp up --name your-app-name --resource-group your-rg
echo.
echo ✅ Deployment preparation complete!
pause
