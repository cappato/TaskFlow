# Use the official .NET 8 runtime as base image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Use the official .NET 8 SDK for building
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project files
COPY ["src/TaskFlow.Server/TaskFlow.Server.csproj", "src/TaskFlow.Server/"]
COPY ["src/TaskFlow.Shared/TaskFlow.Shared.csproj", "src/TaskFlow.Shared/"]

# Restore dependencies
RUN dotnet restore "src/TaskFlow.Server/TaskFlow.Server.csproj"

# Copy all source code
COPY . .

# Build the application
WORKDIR "/src/src/TaskFlow.Server"
RUN dotnet build "TaskFlow.Server.csproj" -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish "TaskFlow.Server.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Final stage
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Create a non-root user
RUN adduser --disabled-password --gecos '' appuser && chown -R appuser /app
USER appuser

ENTRYPOINT ["dotnet", "TaskFlow.Server.dll"]
