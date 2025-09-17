# 1. Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy file csproj và restore dependencies
COPY ["UserService.csproj", "./"]
RUN dotnet restore

# Copy toàn bộ code và build
COPY . .
RUN dotnet publish -c Release -o /app

# 2. Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Copy kết quả build từ stage trước
COPY --from=build /app .

# Mở port mặc định 80
EXPOSE 80

# Chạy app
ENTRYPOINT ["dotnet", "UserService.dll"]
