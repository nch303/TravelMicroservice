# build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# copy đúng csproj của API layer (vì đây là entrypoint)
COPY AuthService.API/UserService.API.csproj ./UserService.API/
RUN dotnet restore UserService.API/UserService.API.csproj

# copy toàn bộ source
COPY . ./
RUN dotnet publish UserService.API/UserService.API.csproj -c Release -o /app/publish

# runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "UserService.API.dll"]
