# Stage 1: Build and Publish
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and csproj files first for optimal Docker layer caching
COPY ["DndBackend.sln", "./"]
COPY ["Directory.Build.props", "./"]
COPY ["src/Dnd.Domain/Dnd.Domain.csproj", "src/Dnd.Domain/"]
COPY ["src/Dnd.Application/Dnd.Application.csproj", "src/Dnd.Application/"]
COPY ["src/Dnd.Infrastructure/Dnd.Infrastructure.csproj", "src/Dnd.Infrastructure/"]
COPY ["src/Dnd.Api/Dnd.Api.csproj", "src/Dnd.Api/"]

# Restore dependencies
RUN dotnet restore "src/Dnd.Api/Dnd.Api.csproj"

# Copy the rest of the application source code
COPY src/ src/

# Build and publish the release artifact
WORKDIR "/src/src/Dnd.Api"
RUN dotnet publish "Dnd.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 2: Production Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Run as non-root user (built into .NET 8 container images)
USER $APP_UID

# Copy published binaries from the build stage
COPY --from=build /app/publish .

# Expose standard ASP.NET Core 8 container port
EXPOSE 8080
ENV ASPNETCORE_HTTP_PORTS=8080

ENTRYPOINT ["dotnet", "Dnd.Api.dll"]

