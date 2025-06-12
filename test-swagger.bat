@echo off
echo ========================================
echo 🔧 Probando Swagger con versión simplificada
echo ========================================

cd src\PimFlow.Server

echo.
echo 📋 Respaldando Program.cs original...
copy Program.cs Program.Original.cs

echo.
echo 🔄 Usando versión simplificada...
copy Program.Simple.cs Program.cs

echo.
echo 🚀 Iniciando servidor simplificado...
echo.
echo 🌐 Swagger: http://localhost:5005/swagger
echo 🔧 Test: http://localhost:5005/api/test
echo 🏥 Health: http://localhost:5005/health
echo.

dotnet run --urls http://localhost:5005
