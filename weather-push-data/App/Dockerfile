FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env2
WORKDIR /app

# Restore as distinct layers
COPY *.csproj ./
RUN dotnet restore

# Copy everything and build
COPY . ./
RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app/out
COPY --from=build-env2 /app/out .
ADD /DB_Files /app/out/DB_Files
CMD ["dotnet", "DotNet.DockerServicePushData.dll"]