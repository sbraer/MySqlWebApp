FROM mcr.microsoft.com/dotnet/core/sdk:3.1-alpine AS build
WORKDIR /app

# copy all files
COPY ./ ./
RUN dotnet restore

# Publish
WORKDIR /app
RUN dotnet publish -c Release -o out

# Create final repository
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-alpine AS runtime
WORKDIR /app
LABEL maintainer="az"
ENV ASPNETCORE_Environment=Production
ENV ASPNETCORE_URLS http://+:5000
EXPOSE 5000/tcp
RUN apk --no-cache add curl
COPY --from=build /app/out ./
HEALTHCHECK --start-period=5s --interval=10s --timeout=3s --retries=5 CMD curl -f http://localhost:5000/ || exit 1
ENTRYPOINT ["dotnet", "WebApp.dll"]

# https://docs.docker.com/engine/examples/dotnetcore/