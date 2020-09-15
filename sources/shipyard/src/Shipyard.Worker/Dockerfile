FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS base
WORKDIR /app

# Restore nuget packages
FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /src
COPY ./src .
RUN dotnet restore /src/Shipyard.Data.Migration
RUN dotnet restore /src/Shipyard.Worker

# Build and publish applications
FROM build as publish
WORKDIR /out
RUN dotnet publish /src/Shipyard.Data.Migration -c Release -o /out/migration
RUN dotnet publish /src/Shipyard.Worker -c Release -o /out/app

# Build runtime image
FROM base as final
WORKDIR /tools
COPY --from=publish /out/migration /tools/migration
WORKDIR /app
COPY --from=publish /out/app /app/
ENTRYPOINT [ "dotnet", "Shipyard.Worker.dll" ]