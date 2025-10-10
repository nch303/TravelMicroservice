# build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# copy đúng csproj của API layer (vì đây là entrypoint)
COPY AdvertisementService.API/AdvertisementService.API.csproj ./AdvertisementService.API/
RUN dotnet restore AdvertisementService.API/AdvertisementService.API.csproj

# copy toàn bộ source
COPY . ./
RUN dotnet publish AdvertisementService.API/AdvertisementService.API.csproj -c Release -o /app/publish

# runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "AdvertisementService.API.dll"]
