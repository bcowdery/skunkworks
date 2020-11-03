FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS base
ENV ASPNETCORE_URLS=http://+:80
EXPOSE 80

# Restore nuget packages
FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /sln
COPY . .
RUN dotnet restore /sln/src/Shipyard.Web

# Build and publish application
FROM build AS publish
WORKDIR /out
RUN dotnet publish /sln/src/Shipyard.Web -c Release -o /out

# Build runtime image
FROM base as final
WORKDIR /app
COPY --from=publish /out /app/
ENTRYPOINT [ "dotnet", "Shipyard.Web.dll" ]