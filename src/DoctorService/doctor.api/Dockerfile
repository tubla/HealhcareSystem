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
COPY ["src/DoctorService/doctor.api/doctor.api.csproj", "src/DoctorService/doctor.api/"]
COPY ["src/DoctorService/doctor.services/doctor.services.csproj", "src/DoctorService/doctor.services/"]
COPY ["src/SharedModules/shared.csproj", "src/SharedModules/"]
COPY ["src/DoctorService/doctor.repositories/doctor.repositories.csproj", "src/DoctorService/doctor.repositories/"]
COPY ["src/DoctorService/doctor.models/doctor.models.csproj", "src/DoctorService/doctor.models/"]
RUN dotnet restore "./src/DoctorService/doctor.api/doctor.api.csproj"
COPY . .
WORKDIR "/src/src/DoctorService/doctor.api"
RUN dotnet build "./doctor.api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./doctor.api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "doctor.api.dll"]