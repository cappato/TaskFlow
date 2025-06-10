# Use the official .NET 8 SDK for building
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project files
COPY ["src/TaskFlow.Client/TaskFlow.Client.csproj", "src/TaskFlow.Client/"]
COPY ["src/TaskFlow.Shared/TaskFlow.Shared.csproj", "src/TaskFlow.Shared/"]

# Restore dependencies
RUN dotnet restore "src/TaskFlow.Client/TaskFlow.Client.csproj"

# Copy all source code
COPY . .

# Build and publish the application
WORKDIR "/src/src/TaskFlow.Client"
RUN dotnet publish "TaskFlow.Client.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Use nginx to serve the static files
FROM nginx:alpine AS final
WORKDIR /usr/share/nginx/html

# Copy the published output
COPY --from=build /app/publish/wwwroot .

# Copy custom nginx configuration
COPY src/TaskFlow.Client/nginx.conf /etc/nginx/nginx.conf

EXPOSE 80
EXPOSE 443
