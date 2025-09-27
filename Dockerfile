# Базовый образ для runtime (ASP.NET Core)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Образ для сборки
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Phoenix_The_Fall_Api.csproj", "."]
RUN dotnet restore "Phoenix The Fall Api.csproj"
COPY . .
WORKDIR "/src"
RUN dotnet build "Phoenix The Fall Api.csproj" -c Release -o /app/build

# Образ для публикации
FROM build AS publish
RUN dotnet publish "Phoenix The Fall Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Финальный образ
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Phoenix The Fall Api.dll"]