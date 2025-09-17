# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj và restore
COPY ApiGateway/ApiGateway.csproj ApiGateway/
RUN dotnet restore ApiGateway/ApiGateway.csproj

# Copy toàn bộ source code và publish
COPY ApiGateway/. ./ApiGateway/
WORKDIR /src/ApiGateway
RUN dotnet publish -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Copy publish từ build stage
COPY --from=build /app/publish .

# EXPOSE port trong container (phải khớp docker-compose)
EXPOSE 80
EXPOSE 443

# Run ApiGateway
ENTRYPOINT ["dotnet", "ApiGateway.dll"]
