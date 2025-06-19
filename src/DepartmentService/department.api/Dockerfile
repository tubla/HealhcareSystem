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
COPY ["src/DepartmentService/department.api/department.api.csproj", "src/DepartmentService/department.api/"]
COPY ["src/DepartmentService/department.services/department.services.csproj", "src/DepartmentService/department.services/"]
COPY ["src/SharedModules/shared.csproj", "src/SharedModules/"]
COPY ["src/DepartmentService/department.repositories/department.repositories.csproj", "src/DepartmentService/department.repositories/"]
COPY ["src/DepartmentService/department.models/department.models.csproj", "src/DepartmentService/department.models/"]
RUN dotnet restore "./src/DepartmentService/department.api/department.api.csproj"
COPY . .
WORKDIR "/src/src/DepartmentService/department.api"
RUN dotnet build "./department.api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./department.api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "department.api.dll"]