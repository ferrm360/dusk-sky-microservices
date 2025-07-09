FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

COPY ["ModerationService.Api.csproj", "./"]
RUN dotnet restore "./ModerationService.Api.csproj"

COPY . .
RUN dotnet build "ModerationService.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ModerationService.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "ModerationService.Api.dll"]
