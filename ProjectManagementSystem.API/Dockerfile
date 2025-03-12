# Use the .NET SDK image as the build environment
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build-env

WORKDIR /app

# Copy the solution file
COPY ../*.sln ./

# Copy the project files
COPY ../ProjectManagementSystem.Common/*.csproj ./ProjectManagementSystem.Common/
COPY ../ProjectManagementSystem.Infrastructure/*.csproj ./ProjectManagementSystem.Infrastructure/
COPY ../ProjectManagementSystem.API/*.csproj ./ProjectManagementSystem.API/
COPY ../ProjectManagementSystem.Test/*.csproj ./ProjectManagementSystem.Test/

# Restore dependencies
RUN dotnet restore

# Copy all the source files
COPY ../ProjectManagementSystem.Common/ ./ProjectManagementSystem.Common/
COPY ../ProjectManagementSystem.Infrastructure/ ./ProjectManagementSystem.Infrastructure/
COPY ../ProjectManagementSystem.API/ ./ProjectManagementSystem.API/
COPY ../ProjectManagementSystem.Test/ ./ProjectManagementSystem.Test/

# Build the project
RUN dotnet publish ProjectManagementSystem.API -c Release -o out

# Use runtime image
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build-env /app/out .

# Define entry point
ENTRYPOINT ["dotnet", "ProjectManagementSystem.API.dll"]
