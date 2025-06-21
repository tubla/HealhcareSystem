# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/MediaService/media.api/media.api.csproj", "src/MediaService/media.api/"]
COPY ["src/MediaService/media.services/media.services.csproj", "src/MediaService/media.services/"]
COPY ["src/SharedModules/shared.csproj", "src/SharedModules/"]
COPY ["src/MediaService/media.repositories/media.repositories.csproj", "src/MediaService/media.repositories/"]
COPY ["src/MediaService/media.models/media.models.csproj", "src/MediaService/media.models/"]
RUN dotnet restore "./src/MediaService/media.api/media.api.csproj"
COPY . .
WORKDIR "/src/src/MediaService/media.api"
RUN dotnet build "./media.api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./media.api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "media.api.dll"]