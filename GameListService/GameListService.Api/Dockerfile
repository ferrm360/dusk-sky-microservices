FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

COPY ["GameListService.Api.csproj", "./"]
RUN dotnet restore "./GameListService.Api.csproj"

COPY . .
RUN dotnet build "GameListService.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "GameListService.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GameListService.Api.dll"]
