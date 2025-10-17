# build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# copy đúng csproj của API layer (vì đây là entrypoint)
COPY PaymentService.API/PaymentService.API.csproj ./PaymentService.API/
RUN dotnet restore PaymentService.API/PaymentService.API.csproj

# copy toàn bộ source
COPY . ./
RUN dotnet publish PaymentService.API/PaymentService.API.csproj -c Release -o /app/publish

# runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "PaymentService.API.dll"]
