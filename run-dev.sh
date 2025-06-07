#!/bin/bash

echo "Starting TaskFlow Development Environment..."

echo ""
echo "Checking .NET installation..."
if ! command -v dotnet &> /dev/null; then
    echo "ERROR: .NET 8 SDK is not installed or not in PATH"
    echo "Please install .NET 8 SDK from https://dotnet.microsoft.com/download"
    exit 1
fi

dotnet --version

echo ""
echo "Restoring NuGet packages..."
if ! dotnet restore; then
    echo "ERROR: Failed to restore packages"
    exit 1
fi

echo ""
echo "Building solution..."
if ! dotnet build --configuration Debug; then
    echo "ERROR: Build failed"
    exit 1
fi

echo ""
echo "Starting API Server (TaskFlow.Server)..."
cd src/TaskFlow.Server
dotnet run &
API_PID=$!
cd ../..

echo "Waiting for API to start..."
sleep 5

echo ""
echo "Starting Blazor Client (TaskFlow.Client)..."
cd src/TaskFlow.Client
dotnet run &
CLIENT_PID=$!
cd ../..

echo ""
echo "========================================"
echo "TaskFlow Development Environment Started!"
echo "========================================"
echo ""
echo "API Server: https://localhost:7000"
echo "Swagger UI: https://localhost:7000/swagger"
echo "Blazor Client: https://localhost:7001"
echo ""
echo "Press Ctrl+C to stop all services..."

# Function to cleanup on exit
cleanup() {
    echo ""
    echo "Stopping services..."
    kill $API_PID 2>/dev/null
    kill $CLIENT_PID 2>/dev/null
    echo "Services stopped."
    exit 0
}

# Set trap to cleanup on script exit
trap cleanup SIGINT SIGTERM

# Wait for user to stop
wait
