FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS base
ENV ASPNETCORE_URLS=http://+:80
EXPOSE 80

# Restore nuget packages
FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /src
COPY ./src .
RUN dotnet restore /src/MissionControl.Web

# Build and publish application
WORKDIR /out
FROM build AS publish
RUN dotnet publish /src/MissionControl.Web -c Release -o /out/app

# Build runtime image
FROM base as final
WORKDIR /app
COPY --from=publish /out/app ./
ENTRYPOINT [ "dotnet", "MissionControl.Web.dll" ]