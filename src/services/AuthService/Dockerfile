# build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# copy đúng csproj của API layer (vì đây là entrypoint)
COPY AuthService.API/AuthService.API.csproj ./AuthService.API/
RUN dotnet restore AuthService.API/AuthService.API.csproj

# copy toàn bộ source
COPY . ./
RUN dotnet publish AuthService.API/AuthService.API.csproj -c Release -o /app/publish

# runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "AuthService.API.dll"]
