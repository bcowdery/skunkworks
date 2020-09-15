FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /work

# Copy, build and publish dotnet core app
COPY . ./
RUN dotnet restore
RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS runtime
WORKDIR /app
COPY --from=build /work/out ./
ENTRYPOINT [ "dotnet", "Shipyard.Web.dll" ]