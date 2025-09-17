FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["src/gateway/ApiGateway/ApiGateway.csproj", "src/gateway/ApiGateway/"]
RUN dotnet restore "src/gateway/ApiGateway/ApiGateway.csproj"

COPY . .
WORKDIR "/src/src/gateway/ApiGateway"
RUN dotnet publish "ApiGateway.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "ApiGateway.dll"]
