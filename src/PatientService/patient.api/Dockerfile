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
COPY ["src/PatientService/patient.api/patient.api.csproj", "src/PatientService/patient.api/"]
COPY ["src/PatientService/patient.services/patient.services.csproj", "src/PatientService/patient.services/"]
COPY ["src/SharedModules/shared.csproj", "src/SharedModules/"]
COPY ["src/PatientService/patient.repositories/patient.repositories.csproj", "src/PatientService/patient.repositories/"]
COPY ["src/PatientService/patient.models/patient.models.csproj", "src/PatientService/patient.models/"]
RUN dotnet restore "./src/PatientService/patient.api/patient.api.csproj"
COPY . .
WORKDIR "/src/src/PatientService/patient.api"
RUN dotnet build "./patient.api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./patient.api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "patient.api.dll"]