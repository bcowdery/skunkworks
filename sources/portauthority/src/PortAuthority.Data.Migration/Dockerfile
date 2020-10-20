# Restore nuget packages
FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /src
COPY ./src .
RUN dotnet restore /src/PortAuthority.Data.Migration

# Build and publish applications
FROM build as publish
WORKDIR /out
RUN dotnet publish /src/PortAuthority.Data.Migration -c Release -o /out

# Build runtime image
FROM build as final
WORKDIR /app
COPY --from=publish /out /app/

RUN apt-get update && apt-get install -y wget
ENV DOCKERIZE_VERSION v0.6.1
RUN wget https://github.com/jwilder/dockerize/releases/download/$DOCKERIZE_VERSION/dockerize-linux-amd64-$DOCKERIZE_VERSION.tar.gz \
    && tar -C /usr/local/bin -xzvf dockerize-linux-amd64-$DOCKERIZE_VERSION.tar.gz \
    && rm dockerize-linux-amd64-$DOCKERIZE_VERSION.tar.gz

CMD ["dotnet", "PortAuthority.Data.Migration.dll"]