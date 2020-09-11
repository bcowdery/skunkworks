# Shipyard-web 
#
# Must build from the ./shipyard parent directory to ensure that
# all dependencies are included in the build context
#
#   $ cd ./shipyard
#   $ docker build -t shipyard-wb -f web\Dockerfile .

FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /app

# Copy, build and publish dotnet core app
COPY . ./
RUN dotnet restore
RUN dotnet publish ./web/src/Shipyard.Web/Shipyard.Web.csproj -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS runtime
WORKDIR /app
COPY --from=build /app/out ./
ENTRYPOINT ["dotnet", "Shipyard.Web.dll"]