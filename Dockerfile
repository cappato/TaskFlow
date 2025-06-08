# Use the official .NET 8 SDK image for building
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy solution file and restore dependencies
COPY *.sln ./
COPY src/TaskFlow.Server/*.csproj ./src/TaskFlow.Server/
COPY src/TaskFlow.Client/*.csproj ./src/TaskFlow.Client/
COPY src/TaskFlow.Shared/*.csproj ./src/TaskFlow.Shared/
COPY tests/TaskFlow.Server.Tests/*.csproj ./tests/TaskFlow.Server.Tests/

RUN dotnet restore

# Copy all source code
COPY . .

# Build the application
WORKDIR /app/src/TaskFlow.Server
RUN dotnet publish -c Release -o /app/publish

# Use the official .NET 8 runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Install SQLite (if needed)
RUN apt-get update && apt-get install -y sqlite3 && rm -rf /var/lib/apt/lists/*

# Copy the published application
COPY --from=build /app/publish .

# Expose the port that Railway will use
EXPOSE 8080

# Set environment variables
ENV ASPNETCORE_URLS=http://0.0.0.0:8080
ENV ASPNETCORE_ENVIRONMENT=Production

# Run the application
ENTRYPOINT ["dotnet", "TaskFlow.Server.dll"]
