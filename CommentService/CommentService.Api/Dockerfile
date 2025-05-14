FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["CommentService.Api/CommentService.Api.csproj", "CommentService.Api/"]
RUN dotnet restore "CommentService.Api/CommentService.Api.csproj"
COPY . .
WORKDIR "/src/CommentService.Api"
RUN dotnet build "CommentService.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CommentService.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CommentService.Api.dll"]
