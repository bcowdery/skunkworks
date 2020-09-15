FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /work

# Copy, build and publish dotnet core app
COPY . ./
RUN dotnet restore
RUN dotnet publish src/Shipyard.App -c Release -o app
RUN dotnet publish src/Shipyard.Data.Migration -c Release -o migration

# Build runtime image
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS runtime
WORKDIR /tools
COPY --from=build /work/migration ./migration
WORKDIR /app
COPY --from=build /work/app ./
ENTRYPOINT [ "dotnet", "Shipyard.App.dll" ]