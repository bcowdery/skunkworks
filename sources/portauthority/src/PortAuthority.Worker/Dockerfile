FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS base
WORKDIR /app

# Restore nuget packages
FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /src
COPY ./src .
RUN dotnet restore /src/PortAuthority.Worker

# Build and publish applications
FROM build as publish
WORKDIR /out
RUN dotnet publish /src/PortAuthority.Worker -c Release -o /out

# Build runtime image
FROM base as final
WORKDIR /app
COPY --from=publish /out /app/
ENTRYPOINT [ "dotnet", "PortAuthority.Worker.dll" ]