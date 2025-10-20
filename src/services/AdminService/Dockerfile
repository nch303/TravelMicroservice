# build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# copy đúng csproj của API layer (vì đây là entrypoint)
COPY AdminService.API/AdminService.API.csproj ./AdminService.API/
RUN dotnet restore AdminService.API/AdminService.API.csproj

# copy toàn bộ source
COPY . ./
RUN dotnet publish AdminService.API/AdminService.API.csproj -c Release -o /app/publish

# runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "AdminService.API.dll"]
