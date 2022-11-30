FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
WORKDIR /app

# Restore as distinct layers
COPY *.csproj ./
RUN dotnet restore

# Copy everything and build
WORKDIR /app
COPY . ./
RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/sdk:6.0
WORKDIR /app/out
COPY --from=build-env /app/out .
CMD ["dotnet", "test", "ApiService.Tests.dll"]