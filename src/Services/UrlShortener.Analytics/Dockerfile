# См. статью по ссылке https://aka.ms/customizecontainer, чтобы узнать как настроить контейнер отладки и как Visual Studio использует этот Dockerfile для создания образов для ускорения отладки.

# Этот этап используется при запуске из VS в быстром режиме (по умолчанию для конфигурации отладки)
FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
USER $APP_UID
WORKDIR /app


# Этот этап используется для сборки проекта службы
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Services/UrlShortener.Analytics/UrlShortener.Analytics.csproj", "src/Services/UrlShortener.Analytics/"]
COPY ["src/Persistence/UrlShortener.Persistence/UrlShortener.Persistence.csproj", "src/Persistence/UrlShortener.Persistence/"]
COPY ["src/Shared/UrlShortener.Shared/UrlShortener.Shared.csproj", "src/Shared/UrlShortener.Shared/"]
RUN dotnet restore "./src/Services/UrlShortener.Analytics/UrlShortener.Analytics.csproj"
COPY . .
WORKDIR "/src/src/Services/UrlShortener.Analytics"
RUN dotnet build "./UrlShortener.Analytics.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Этот этап используется для публикации проекта службы, который будет скопирован на последний этап
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./UrlShortener.Analytics.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Этот этап используется в рабочей среде или при запуске из VS в обычном режиме (по умолчанию, когда конфигурация отладки не используется)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "UrlShortener.Analytics.dll"]